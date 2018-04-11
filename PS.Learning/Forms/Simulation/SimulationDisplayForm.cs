// ------------------------------------------
// <copyright file="SimulationDisplayForm.cs" company="Pedro Sequeira">
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

//    Last updated: 12/09/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Environments;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Learning.Core.Testing.SingleTests;

namespace PS.Learning.Forms.Simulation
{
    public partial class SimulationDisplayForm : SimulationForm
    {
        public const int LARGE_CELL_SIZE = 250;
        public const bool CELL_VIEW = false;
        protected readonly bool limitedUser;
        protected EnvironmentControl environmentControl;

        protected SimulationDisplayForm()
        {
            //for design purposes
            this.InitializeComponent();
        }

        public SimulationDisplayForm(FitnessTest test) :
            this(test, false)
        {
        }

        public SimulationDisplayForm(FitnessTest test, bool limitedUser)
            : base(test)
        {
            //allow for keys detection when not recording
            this.KeyPreview = true;
            this.limitedUser = limitedUser;

            this.InitializeComponent();
        }

        protected override bool DisplayEnabled
        {
            set { this.simulationPanel.Enabled = value; }
            get { return this.environmentControl.VisibleCells; }
        }

        public override void SetDebugMode(bool value)
        {
            lock (this.locker)
                this.environmentControl.Environment.DebugMode = value;
        }

        public override void PrintEnvironment()
        {
            lock (this.locker)
                this.environmentControl.SaveToImage(Path.GetFullPath("./environment.png"));
        }

        public override void AdvanceSimulation(ulong toTimeStep)
        {
            var debugMode = this.environmentControl.Environment.DebugMode;
            this.environmentControl.Environment.DebugMode = false;
            this.environmentControl.VisibleCells = false;
            this.environmentControl.Control.Invalidate(true);

            base.AdvanceSimulation(toTimeStep);

            this.environmentControl.Environment.DebugMode = debugMode;
            this.environmentControl.VisibleCells = true;
        }

        protected override void RefreshDisplay()
        {
            if (this.DisplayEnabled)
                this.simulationPanel.Invalidate();
        }

        protected override bool SetupSimulation()
        {
            var result = base.SetupSimulation();
            lock (this.locker)
                this.environmentControl = this.CreateEnvironmentControl(((ICellAgent) this.simulation.Agent).Environment);
            return result;
        }

        protected override void FormatForm()
        {
            this.Height = //System.Math.Max(this.Height, 
                ((this.CellSize + 1)*(int) this.environmentControl.Environment.Rows) + 34;
            this.Width = Math.Max(
                this.Width, ((this.CellSize + 1)*(int) this.environmentControl.Environment.Cols)) + 11;
        }

        protected void FormatFitnessLabels()
        {
            this.fitnessLabel.Visible = this.numStepsLabel.Visible = true;
            this.fitnessLabel.Location += new Size(-130, -35);
            this.numStepsLabel.Location += new Size(20, -35);
            this.fitnessLabel.ForeColor = this.numStepsLabel.ForeColor = Color.DarkBlue;
            this.fitnessLabel.BorderStyle = this.numStepsLabel.BorderStyle = BorderStyle.None;
            this.fitnessLabel.Font = this.numStepsLabel.Font =
                new Font("Candara", 15F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
        }

        protected virtual EnvironmentControl CreateEnvironmentControl(IEnvironment environment)
        {
            return new EnvironmentControl(environment, this.simulationPanel, CELL_VIEW ? LARGE_CELL_SIZE : this.CellSize,
                this.limitedUser, CELL_VIEW);
        }

        protected override void StopSimulation()
        {
            base.StopSimulation();

            lock (this.locker)
            {
                if (this.environmentControl == null) return;
                this.environmentControl.Dispose();
                this.environmentControl = null;
            }
        }

        protected override bool Step()
        {
            //updates simulation info
            this.UpdateLabels();

            return base.Step();
        }

        protected void UpdateLabels()
        {
            lock (this.locker)
            {
                this.numStepsLabel.Text =
                    string.Format(CultureInfo.InvariantCulture, "Step: {0:#,0.##}",
                        this.ControlledAgent.LongTermMemory.TimeStep);
                this.numStepsLabel.Location = new Point(12, this.numStepsLabel.Location.Y);

                this.fitnessLabel.Text = //"Group Fitness: -1,215.62";
                    string.Format(CultureInfo.InvariantCulture, "{0}: {1:#,0.0}",
                        ((IFitnessScenario) this.test.Scenario).FitnessText, this.simulation.Score.Value);
                this.fitnessLabel.Location = new Point(
                    this.Width - 20 - this.fitnessLabel.Size.Width,
                    this.fitnessLabel.Location.Y);
            }
        }

        #region Keys handling

        protected override string GetActionIDFromKey(Keys keys)
        {
            switch (keys)
            {
                case Keys.Down:
                    return "MoveDown";
                case Keys.Up:
                    return "MoveUp";
                case Keys.Left:
                    return "MoveLeft";
                case Keys.Right:
                    return "MoveRight";
                case Keys.Space:
                    return "Eat";
                case Keys.P:
                    return "Pick Up";
                case Keys.D:
                    return "Drop Off";
                case Keys.O:
                    return "Open Lair";
                default:
                    return "DoNothing";
            }
        }

        #endregion
    }
}