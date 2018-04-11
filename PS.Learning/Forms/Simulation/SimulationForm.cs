 // ------------------------------------------
// <copyright file="SimulationForm.cs" company="Pedro Sequeira">
// 
//     Copyright (c) 2018 Pedro Sequeira
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// </copyright>
// <summary>
//    Project: Learning.Forms

//    Last updated: 6/20/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Social.Testing;
using PS.Learning.Core.Testing.Simulations;
using PS.Learning.Core.Testing.SingleTests;

namespace PS.Learning.Forms.Simulation
{
    public class SimulationForm : Form, ISimulationForm
    {
        protected readonly object locker = new object();
        private IAgent _controlledAgent;
        private Keys _lastPressedKey;
        private Timer _updateTimer;
        protected AgentInfoForm agentInfoForm;
        protected int numSimulation;
        protected SimulationControlForm simControlForm;
        protected FitnessSimulation simulation;
        protected FitnessTest test;

        protected SimulationForm()
        {
            //for design purposes
        }

        protected SimulationForm(FitnessTest test)
        {
            this.test = test;
            this.simControlForm = new SimulationControlForm(this);
            this.InitTimer();
        }

        protected virtual bool DisplayEnabled { set; get; }

        #region ISimulationForm Members

        public int CellSize { get; set; }

        public int UpdateInterval
        {
            set { lock (this.locker) this._updateTimer.Interval = value; }
        }

        public virtual void SetDebugMode(bool value)
        {
        }

        public IAgent ControlledAgent
        {
            set
            {
                lock (this.locker)
                {
                    if ((value != null) && (value.Equals(this._controlledAgent))) return;

                    //sets controlled agent
                    var controlable = true;
                    if (this._controlledAgent != null)
                    {
                        controlable = this.Controlable;
                        this.Controlable = false;
                    }

                    this._controlledAgent = value;
                    this.Controlable = controlable;
                }
            }
            protected get { return this._controlledAgent; }
        }

        public bool Controlable
        {
            set
            {
                if (this.ControlledAgent != null)
                    this.ControlledAgent.BehaviorManager.Controlable = value;
            }
            get { return this.ControlledAgent != null && this.ControlledAgent.BehaviorManager.Controlable; }
        }

        public virtual void Init()
        {
            this.simControlForm.Show();
            this.test.StartTest();
            this.StartNewSimulation();
        }

        public virtual void PauseResumeSimulation()
        {
            lock (this.locker)
            {
                this.Controlable = !this.simControlForm.StepBtn.Enabled;
                this._updateTimer.Enabled = !this._updateTimer.Enabled;
                this.simControlForm.StepBtn.Enabled = !this._updateTimer.Enabled;
                this.simControlForm.PauseBtn.Text = this._updateTimer.Enabled ? @"Pause" : @"Resume";
            }
        }

        public void StepSimulation()
        {
            lock (this.locker)
            {
                this.Controlable = false;
                if (this.Step())
                    this.RefreshDisplay();
                this.Controlable = true;
            }
        }

        public virtual void ResetSimulation()
        {
            lock (this.locker)
            {
                var enabled = this._updateTimer.Enabled;
                this._updateTimer.Enabled = false;
                this.Reset();
                this._updateTimer.Enabled = enabled;
            }
        }

        public virtual void PrintEnvironment()
        {
        }

        public virtual void AdvanceSimulation(ulong toTimeStep)
        {
            lock (this.locker)
            {
                var wasRunning = this._updateTimer.Enabled;
                this._updateTimer.Enabled = false;

                this.Controlable = false;
                this.simControlForm.Enabled = this.agentInfoForm.Enabled = false;
                this.DisplayEnabled = false;

                var ltm = this.ControlledAgent.LongTermMemory;
                var numTimeSteps = this.simulation.Scenario.TestsConfig.NumTimeSteps;
                var timeInt = DateTime.Now;
                while ((ltm.TimeStep < numTimeSteps) && (ltm.TimeStep < toTimeStep))
                {
                    this.Step();

                    //updates interface each second
                    if (!(DateTime.Now.Subtract(timeInt).TotalSeconds >= 1)) continue;
                    timeInt = DateTime.Now;
                    Application.DoEvents();
                }

                this.simControlForm.Enabled = this.agentInfoForm.Enabled = true;
                this._updateTimer.Enabled = wasRunning;
                this.DisplayEnabled = true;
                this.Controlable = true;
            }
        }

        public virtual void Reset()
        {
            lock (this.locker)
            {
                this.StopSimulation();
                this.StartNewSimulation();
            }
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.DisplayEnabled = true;
            this.RefreshDisplay();
        }

        protected void InitTimer()
        {
            this._updateTimer = new Timer();
            this._updateTimer.Tick += this.UpdateTimerTick;
            this._updateTimer.Enabled = true;
            this._updateTimer.Interval = 300;
        }

        protected virtual void RefreshDisplay()
        {
        }

        protected virtual void PrintResults()
        {
            this.test.PrintResults();
        }

        protected virtual bool Step()
        {
            lock (this.locker)
            {
                //updates keyboard handling
                this.UpdateKeyControls();

                //updates simulation
                this.simulation.Update();

                //updates selected agent info if not "advancing"
                if (this.simControlForm.ShowAgInfoCBox.Checked && this.DisplayEnabled)
                    this.agentInfoForm.UpdateVariables();

                //tests simulation finished
                if (this.simulation.SimulationFinished())
                {
                    this.Reset();
                    return false;
                }

                return true;
            }
        }

        protected void StartNewSimulation()
        {
            lock (this.locker)
                if (this.SetupSimulation())
                    this.InitForm();
        }

        protected virtual void StopSimulation()
        {
            lock (this.locker)
            {
                this.simulation.FinishSimulation();
                this.test.FinishSimulation(this.simulation);
                this.agentInfoForm.Dispose();
            }
        }

        protected virtual bool SetupSimulation()
        {
            lock (this.locker)
            {
                //set up new simulation
                var testsConfig = this.test.Scenario.TestsConfig;
                if (this.numSimulation++ >= testsConfig.NumSimulations)
                {
                    this._updateTimer.Enabled = false;
                    this.Close();
                    return false;
                }
				this.simulation = (FitnessSimulation) this.test.CreateAndSetupSimulation((uint)(this.numSimulation-1));
                this.simulation.StartSimulation();

                //inits agent info windows
                this.InitAgentInfoForms();

                //sets controllable agent list
                this.SetAgentControlList();

                //sets default advance value 
                var numTimeSteps = testsConfig.NumTimeSteps;
                this.simControlForm.AdvanceNumUd.Maximum = numTimeSteps;
                this.simControlForm.AdvanceNumUd.Value = numTimeSteps - (numTimeSteps*0.05m);
                //(testsConfig.NumTimeSteps/100m);

                this.Controlable = true;
                this.OnLocationChanged(EventArgs.Empty);
            }

            return true;
        }

        protected virtual void UpdateKeyControls()
        {
            if (!this.Controlable) return;

            //checks last pressed key
            var action = this.GetActionFromKey();

            //ignores null actions
            this.ControlledAgent.BehaviorManager.NextAction = action;

            //reset key detected
            this._lastPressedKey = new Keys();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            this.UpdateLocations();
            base.OnLocationChanged(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            this.UpdateLocations();
            base.OnSizeChanged(e);
        }

        protected void UpdateLocations()
        {
            if (this.simControlForm != null)
                this.simControlForm.Location = this.Location - new Size(this.simControlForm.Width, 0);
            if (this.agentInfoForm != null)
                this.agentInfoForm.Location = this.Location + new Size(this.Width, 0);
            this.BringAllToFront();
        }

        protected void BringAllToFront()
        {
            if (this.simControlForm != null)
                this.simControlForm.BringToFront();
            if (this.agentInfoForm != null)
                this.agentInfoForm.BringToFront();
            this.BringToFront();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            lock (this.locker)
            {
                this.StopSimulation();

                if (this.simControlForm.PrintResCheckBox.Checked)
                    this.PrintResults();

                this.simControlForm.Close();
                this.simControlForm.Dispose();

                this.agentInfoForm.Dispose();

                this.test.FinishTest();
                base.OnClosing(e);
            }
        }

        protected virtual void UpdateTimerTick(object sender, EventArgs e)
        {
            lock (this.locker)
                if (this.Step())
                    this.RefreshDisplay();
        }

        protected void EnableInput(Control control)
        {
            //enables key preview for all controls
            foreach (Control childControl in control.Controls)
            {
                this.EnableInput(childControl);
                childControl.KeyDown += this.ControlPreviewKeyDown;
            }
        }

        protected void ControlPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!char.IsDigit((char) e.KeyCode))
                e.SuppressKeyPress = e.Handled = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //stepping is controlled by the user
            lock (this.locker)
            {
                this._lastPressedKey = keyData;
                if (this.Step())
                    this.RefreshDisplay();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected IAction GetActionFromKey()
        {
            var actionID = this.GetActionIDFromKey(this._lastPressedKey);

            if (actionID == null) return null;
            return this.ControlledAgent.Actions.ContainsKey(actionID)
                ? this.ControlledAgent.Actions[actionID]
                : null;
        }

        protected virtual string GetActionIDFromKey(Keys keys)
        {
            return null;
        }

        protected void SetAgentControlList()
        {
            lock (this.locker)
            {
                //forces change of controlled agent
                this._controlledAgent = null;

                //adds agent to controlable agent list
                this.simControlForm.ControlAgComboBox.Items.Clear();
                this.simControlForm.ControlAgComboBox.Items.AddRange(this.GetSimulationAgents().ToArray());
                this.simControlForm.ControlAgComboBox.SelectedIndex = 0;
            }
        }

        protected void InitAgentInfoForms()
        {
            lock (this.locker)
            {
                var agents = this.GetSimulationAgents();

                //creates and shows an info form to show agents' info
                this.agentInfoForm = new AgentInfoForm(agents);
                agentInfoForm.Show();
            }
        }

        protected IEnumerable<IAgent> GetSimulationAgents()
        {
            return (this.simulation is SocialSimulation)
                ? (IEnumerable<IAgent>) ((SocialSimulation) this.simulation).Population
                : new List<IAgent> {this.simulation.Agent};
        }

        protected virtual void InitForm()
        {
            lock (this.locker)
            {
                this.FormatForm();
                this._updateTimer.Interval = this.simControlForm.TimerTrackBar.Value;
                //this.FormatFitnessLabels();
                this.EnableInput(this);
            }
        }

        protected virtual void FormatForm()
        {
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SimulationForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(284, 259);
            this.Font = new System.Drawing.Font("Candara", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "SimulationForm";
            this.ResumeLayout(false);

        }
    }
}