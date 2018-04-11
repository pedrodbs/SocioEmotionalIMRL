// ------------------------------------------
// <copyright file="SimulationControlForm.cs" company="Pedro Sequeira">
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

//    Last updated: 12/9/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using System.Windows.Forms;
using PS.Learning.Core.Domain.Agents;

namespace PS.Learning.Forms.Simulation
{
    public partial class SimulationControlForm : Form
    {
        protected readonly ISimulationForm simulationForm;

        public SimulationControlForm(ISimulationForm simulationForm)
        {
            this.simulationForm = simulationForm;
            this.InitializeComponent();
        }

        public CheckBox ShowAgInfoCBox
        {
            get { return showAgInfoCBox; }
        }

        public Button StepBtn
        {
            get { return stepBtn; }
        }

        public Button PauseBtn
        {
            get { return pauseBtn; }
        }

        public Button ResetBtn
        {
            get { return resetBtn; }
        }

        public CheckBox PrintResCheckBox
        {
            get { return printResCheckBox; }
        }

        public NumericUpDown AdvanceNumUd
        {
            get { return advanceNumUD; }
        }

        public TrackBar TimerTrackBar
        {
            get { return timerTrackBar; }
        }

        public ComboBox ControlAgComboBox
        {
            get { return controlAgComboBox; }
        }

        protected virtual void PauseBtnClick(object sender, EventArgs e)
        {
            this.simulationForm.PauseResumeSimulation();
        }

        private void StepBtnClick(object sender, EventArgs e)
        {
            this.simulationForm.StepSimulation();
        }

        private void TimerTrackBarScroll(object sender, EventArgs e)
        {
            this.simulationForm.UpdateInterval = this.timerTrackBar.Value;
        }

        private void ResetBtnClick(object sender, EventArgs e)
        {
            this.simulationForm.ResetSimulation();
        }

        private void AdvanceBtnClick(object sender, EventArgs e)
        {
            this.advanceBtn.Enabled = false;
            this.simulationForm.AdvanceSimulation((ulong) this.advanceNumUD.Value);
            this.advanceBtn.Enabled = true;
        }

        private void PrintWorldBtnClick(object sender, EventArgs e)
        {
            this.simulationForm.PrintEnvironment();
        }

        private void DebugCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            this.simulationForm.SetDebugMode(this.debugCheckBox.Checked);
        }

        private void ControlAgListBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.simulationForm.ControlledAgent = (IAgent) this.controlAgComboBox.SelectedItem;
            this.simulationForm.Select();
        }
    }
}