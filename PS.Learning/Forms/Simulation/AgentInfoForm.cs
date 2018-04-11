// ------------------------------------------
// <copyright file="AgentInfoForm.cs" company="Pedro Sequeira">
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Managers.Behavior;
using PS.Learning.Core.Domain.Memories;

namespace PS.Learning.Forms.Simulation
{
    public partial class AgentInfoForm : Form
    {
        protected IAgent agent;

        public AgentInfoForm(IEnumerable<IAgent> agents)
        {
            this.InitializeComponent();

            this.agentsComboBox.Items.AddRange(agents.ToArray());
            this.agentsComboBox.SelectedIndex = 0;
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Text = string.Format("Agent Information ({0} available)", this.agentsComboBox.Items.Count);
        }

        public virtual void UpdateVariables()
        {
            //updates all info related to the selected agent
            var stm = this.agent.ShortTermMemory;
            var ltm = this.agent.LongTermMemory;
            var prevState = stm.PreviousState;
            var curAction = stm.CurrentAction;
            if ((prevState == null) || (curAction == null)) return;

            this.prevStateTxtBox.Text = prevState.ToString();
            this.curStateTxtBox.Text = stm.CurrentState.ToString();
            this.actionTxtBox.Text = curAction.ToString();
            this.rewardTxtBox.Text = stm.CurrentReward.Value.ToString("#,0.####");
            this.goalDistTxtBox.Text = this.agent is ICellAgent
                ? ((CellAgentLTM) ltm).GetDistanceToOptimalState().ToString("#,0.##")
                : "NA";
            this.actionValueListBox.Items.Clear();
            for (var actionID = 0u; actionID < ltm.NumActions; actionID++)
            {
                var actionValue = ltm.GetStateActionValue(stm.CurrentState.ID, actionID);
                var actionRwdAvg = ltm.GetStateActionReward(stm.CurrentState.ID, actionID);
                this.actionValueListBox.Items.Add(
                    string.Format("{0}: {1:#,0.##}/{2:#,0.##}",
                        ltm.GetAction(actionID), actionValue, actionRwdAvg));
            }

            this.fitnessTxtBox.Text = this.agent.Fitness.Value.ToString("#,0.##");
            this.numTasksTxtBox.Text = ltm.NumTasks.Value.ToString(CultureInfo.InvariantCulture);
            this.numStatesTxtBox.Text = ltm.NumStates.ToString(CultureInfo.InvariantCulture);
            this.alphaTrackBar.Value = (int) (this.agent.LearningManager.LearningRate.Value*10);

            var behaviorManager = this.agent.BehaviorManager;
            if (behaviorManager is EpsilonGreedyBehaviorManager)
                this.epsilonTrackBar.Value =
                    (int) System.Math.Min(
                        ((EpsilonGreedyBehaviorManager) behaviorManager).Epsilon.Value*10, 10);
        }

        private void AgentsComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.agent = (IAgent) this.agentsComboBox.SelectedItem;
            this.UpdateVariables();
        }
    }
}