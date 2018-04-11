// ------------------------------------------
// HuntingEnvironment.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2014/2/24
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.States;
using PS.Learning.IMRL.Domain;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;
using PS.Learning.Tests.AltruismOptimization.Domain.Managers;
using PS.Utilities.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Environments
{
    [Serializable]
    public class HuntingEnvironment : FoodSharingEnvironment
    {
        #region Protected Fields

        protected Cell[][] agentCornersCells;
        protected List<Cell> cornerCells;
        protected MovingPrey prey;

        #endregion Protected Fields

        #region Public Methods

        public override bool AteFood(IAgent agent, IState state, IAction action)
        {
            //verifies elements
            if (!(agent is IFoodSharingAgent) || (state == null) || !(state is IStimuliState) ||
                (action == null) || (!this.AutoEat && !(action is Eat)))
                return false;

            var foodSharingAgent = (IFoodSharingAgent)agent;

            var containsOtherAgent =
                ((IDetectOthersPerceptionManager)foodSharingAgent.PerceptionManager).SeeOtherAgents(state);

            var containsFood = this.FoodResources.Any(
                foodResource => ((IStimuliState)state).Sensations.Contains(foodResource.IdToken));

            var isOtherAgentEating = containsOtherAgent &&
                                     (this.AutoEat ||
                                      this.Agents.Any(otherAgent =>
                                          !otherAgent.Equals(agent) &&
                                          (otherAgent.ShortTermMemory.CurrentAction is Eat)));

            //hunters captureprey if solo hunting or other agent is in same cell
            return containsFood && (this.Scenario.SoloHunting || isOtherAgentEating);
        }

        public override void Init()
        {
            //creates food resources (prey)
            this.prey = new MovingPrey(this)
            {
                IdToken = FOOD_ID,
                Description = FOOD_ID,
                Reward = FOOD_REWARD,
                Visible = true,
                Walkable = true,
                ImagePath = "../../../../bin/resources/rabbit.png",
                Color = System.Drawing.Color.FromArgb(255, 255, 140, 0).ToColorInfo(),
                MoveProb = this.Scenario.SoloHunting ? 0.9 : 0.1 //more dificult if solo hunting
            };
            this.FoodResources = new List<CellElement> { prey };
        }

        public override void Update()
        {
            base.Update();

            //updates preys' movements
            this.prey.Update();
        }

        #endregion Public Methods
    }
}