// ------------------------------------------
// ElectricLeverEnvironment.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2012/11/23
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.States;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;
using PS.Utilities.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Environments
{
    [Serializable]
    public class ElectricLeverEnvironment : LeverEnvironment
    {
        #region Public Fields

        public const string ELECTRIC_SIGN_ID = "electricity";
        public const string SHOCK_ID = "shock";

        #endregion Public Fields

        #region Protected Fields

        protected const uint MAX_NO_ELECTRIC_STEPS = 1;
        protected const double MILD_SHOCK_REWARD = 0;
        protected const double SHOCK_REWARD = -1;

        //-.05;
        protected readonly List<CellElement> electricSigns = new List<CellElement>();

        protected readonly List<CellElement> shockElements = new List<CellElement>();
        protected uint curNoElectricSteps;

        #endregion Protected Fields

        #region Public Properties

        public bool Electrified { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void CreateCells(uint rows, uint cols, string configFile)
        {
            base.CreateCells(rows, cols, configFile);

            //gathers information about possible lever cells and creates electricity signs
            foreach (var cell in this.Cells)
                if (this.IsWalkable(cell))
                    this.electricSigns.Add(new CellElement
                    {
                        IdToken = ELECTRIC_SIGN_ID,
                        Description = ELECTRIC_SIGN_ID,
                        //Reward = SHOCK_REWARD,
                        Visible = true,
                        Walkable = true,
                        ImagePath = "../../../../bin/resources/lab/electricity.png",
                        Color = System.Drawing.Color.FromArgb(255, 255, 247, 0).ToColorInfo(),
                        Cell = cell
                    });

            //creates a shock sensation element for each agent
            foreach (var agent in this.Agents.Cast<IFoodSharingAgent>())
                this.shockElements.Add(new CellElement
                {
                    IdToken = SHOCK_ID,
                    Description = SHOCK_ID,
                    Visible = false,
                    Walkable = true,
                    ImagePath = "../../../../bin/resources/lab/shock.png",
                    Color = System.Drawing.Color.FromArgb(255, 27, 102, 255).ToColorInfo(),
                    Cell = agent.Cell
                });
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var electricSign in this.electricSigns)
                electricSign.Dispose();
            this.electricSigns.Clear();
            foreach (var element in this.shockElements)
                element.Dispose();
            this.shockElements.Clear();
        }

        public override double GetAgentReward(IAgent agent, IState state, IAction action)
        {
            //checks whether some agent ate food while electricity was on
            var reward = 0d;
            if (((IStimuliState)state).Sensations.Contains(SHOCK_ID))
                reward = SHOCK_REWARD;

            //checks if agent ate food and electricity status
            else if (this.AteFood(agent, state, action))
                reward = FOOD_REWARD;

            //checks whether floor is electrified (mild shock)
            if (((IStimuliState)state).Sensations.Contains(ELECTRIC_SIGN_ID))
                reward += MILD_SHOCK_REWARD;

            //checks if agent is hungry (negative reward) or not hungry, not full (normal reward)
            reward += this.IsHungry(agent, state, action) ? this.Scenario.HungryReward : 0;

            return reward;
        }

        public override void Init()
        {
            base.Init();

            //environment starts electrified
            this.Electrified = true;

            //changes lever image
            this.Lever.ImagePath = "../../../../bin/resources/lab/electric-switch.png";

            //changes food visibility
            foreach (var foodResource in this.FoodResources)
            {
                foodResource.Visible = true;
                foodResource.ForceRepaint = true;
            }
        }

        public override void Update()
        {
            //check agents ate and resets after task completion
            base.Update();

            //verifies agents in switch
            this.VerifyElectricSwitch();

            //checks shock to agents
            this.CheckElectricShock();

            //food is always visible
            foreach (var foodResource in this.FoodResources)
            {
                foodResource.Visible = true;
                foodResource.ForceRepaint = false;
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void CheckAgentsAte()
        {
            //only updates eating variables if electric floor is off
            if (!this.Electrified)
                base.CheckAgentsAte();
            else
                //update hunger status anyway
                foreach (IFoodSharingAgent agent in this.Agents)
                    agent.MotivationManager.Hunger.Value =
                        (uint)(++this.stepsWithoutEating[agent] >= this.Scenario.MaxStepsWithoutEating ? 1 : 0);
        }

        protected virtual void CheckElectricShock()
        {
            //checks whether to provide electric shock, ie, iff electric floor is on and someone is trying to eat
            var electricShock =
                this.Electrified &&
                this.Agents.Cast<IFoodSharingAgent>().Any(
                agent => this.FoodResources.Any(resource => agent.Cell.Equals(resource.Cell)));

            //just put the shock sensation in the agents' cell
            for (var i = 0; i < this.Agents.Count; i++)
            {
                this.shockElements[i].Cell = ((IFoodSharingAgent)this.Agents[i]).Cell;
                this.shockElements[i].Visible = electricShock;
                this.shockElements[i].ForceRepaint = true;
            }
        }

        protected virtual void VerifyElectricSwitch()
        {
            var wasElectrified = this.Electrified;

            //checks whether some agent is/isn't on the lever or electricity recently off (disables/enables electricity)
            this.Electrified = !this.Agents.Cast<IFoodSharingAgent>().Any(agent => Equals(agent.Cell, this.Lever.Cell)) &&
                (wasElectrified || (this.curNoElectricSteps++ >= MAX_NO_ELECTRIC_STEPS));

            //resets no electricity counter
            if (this.Electrified) this.curNoElectricSteps = 0;

            if (!(wasElectrified ^ this.Electrified)) return;

            //just for repaint purposes
            foreach (var electricSign in this.electricSigns)
            {
                electricSign.Visible = this.Electrified;
                electricSign.ForceRepaint = true;
            }
        }

        #endregion Protected Methods
    }
}