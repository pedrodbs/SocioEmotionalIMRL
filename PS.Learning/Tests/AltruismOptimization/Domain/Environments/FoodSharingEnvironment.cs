// ------------------------------------------
// FoodSharingEnvironment.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/3/26
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.States;
using PS.Learning.IMRL.Domain;
using PS.Learning.Social.Domain;
using PS.Learning.Social.Domain.Environments;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;
using PS.Learning.Tests.AltruismOptimization.Domain.Managers;
using PS.Learning.Tests.AltruismOptimization.Testing;
using PS.Utilities.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Environments
{
    [Serializable]
    public class FoodSharingEnvironment : SocialEnvironment, IFoodSharingEnvironment
    {
        #region Public Fields

        public const string FOOD_ID = "food";

        #endregion Public Fields

        #region Protected Fields

        protected const string FOOD_CELL_ID_TOKEN = "food-location";
        protected const double FOOD_REWARD = 1d;
        protected const string POSITION_CELL_ID_TOKEN = "start-position";
        protected readonly List<Cell> possibleFoodResourceCells = new List<Cell>();
        protected readonly List<Cell> possibleStartPositionCells = new List<Cell>();

        protected readonly Dictionary<IFoodSharingAgent, int> stepsWithoutEating =
            new Dictionary<IFoodSharingAgent, int>();

        #endregion Protected Fields

        #region Public Constructors

        public FoodSharingEnvironment()
        {
            //auto-eat true by default
            this.AutoEat = true;
        }

        #endregion Public Constructors

        #region Public Properties

        public bool AutoEat { get; set; }

        public Dictionary<IFoodSharingAgent, int> CyclesWithoutEating { get; private set; }

        public List<CellElement> FoodResources { get; protected set; }

        public new IFoodSharingScenario Scenario
        {
            get { return base.Scenario as IFoodSharingScenario; }
        }

        #endregion Public Properties

        #region Public Methods

        public override bool AgentFinishedTask(IAgent agent, IState state, IAction action)
        {
            return this.AteFood(agent, state, action);
        }

        public virtual bool AteFood(IAgent agent, IState state, IAction action)
        {
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
                                          (otherAgent != agent) &&
                                          (otherAgent.ShortTermMemory.CurrentAction is Eat)));

            var isStrongerAgent = foodSharingAgent.IsStronger;

            //cooperative eat only "works" when there is food and no one else is eating at the same location and time
            //if both agents try to eat at the same time, no food is eaten by anyone
            return containsFood && (!containsOtherAgent || (!isOtherAgentEating || isStrongerAgent));
        }

        public bool AteFood(IFoodSharingAgent agent)
        {
            return this.CyclesWithoutEating[agent] < 1;
        }

        public override void CreateCells(uint rows, uint cols, string configFile)
        {
            base.CreateCells(rows, cols, configFile);

            //gathers information about start position and food resource cells
            foreach (var cell in this.Cells)
            {
                if (cell.Description.Contains(POSITION_CELL_ID_TOKEN))
                    this.possibleStartPositionCells.Add(cell);
                if (cell.Description.Contains(FOOD_CELL_ID_TOKEN))
                    this.possibleFoodResourceCells.Add(cell);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            this.FoodResources.Clear();
            this.CyclesWithoutEating.Clear();
            this.possibleStartPositionCells.Clear();
            this.possibleFoodResourceCells.Clear();
        }

        public override double GetAgentReward(IAgent agent, IState state, IAction action)
        {
            //checks if agent ate food
            if (this.AteFood(agent, state, action))
                return FOOD_REWARD;

            //checks if agent is hungry (negative reward) or not hungry, not full (normal reward)
            return this.IsHungry(agent, state, action) ? this.Scenario.HungryReward : 0;
        }

        public override void Init()
        {
            //creates food resources list
            this.FoodResources = new List<CellElement>();

            for (var i = 0; i < this.Scenario.NumFoodResources; i++)
            {
                //creates food resource and adds it to list
                this.FoodResources.Add(new CellElement
                {
                    IdToken = FOOD_ID + i,
                    Description = FOOD_ID,
                    Reward = FOOD_REWARD,
                    Visible = true,
                    Walkable = true,
                    ImagePath = "../../../../bin/resources/lab/cheese.png",
                    Color = System.Drawing.Color.FromArgb(255, 255, 140, 0).ToColorInfo(),
                });
            }
        }

        public override void Reset()
        {
            this.PlaceFoodResources();
            this.PlaceAgents();
        }

        public override void SetAgents(IList<ISocialAgent> agents)
        {
            base.SetAgents(agents);

            //sets initial cycles without eating for each agent based on their order
            this.CyclesWithoutEating = new Dictionary<IFoodSharingAgent, int>();
            for (var i = 0; i < agents.Count; i++)
            {
                this.CyclesWithoutEating[(IFoodSharingAgent)agents[i]] = i;
                this.stepsWithoutEating[(IFoodSharingAgent)agents[i]] = 0;
            }
        }

        public override void Update()
        {
            this.CheckAgentsAte();
            base.Update();
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void CheckAgentsAte()
        {
            var someoneAte = false;
            foreach (IFoodSharingAgent agent in this.Agents)
            {
                //checks if agent ate food
                var ateFood = this.AteFood(
                    agent, agent.ShortTermMemory.PreviousState, agent.ShortTermMemory.CurrentAction);
                someoneAte |= ateFood;

                if (ateFood)
                {
                    //atributes -1 to agents that just ate and resets steps without eating
                    this.CyclesWithoutEating[agent] = -1;
                    this.stepsWithoutEating[agent] = -1;
                }

                //update hunger status
                agent.MotivationManager.Hunger.Value =
                    (uint)(++this.stepsWithoutEating[agent] >= this.Scenario.MaxStepsWithoutEating ? 1 : 0);
            }

            // if some agent just ate, update cycles
            if (someoneAte)
                foreach (IFoodSharingAgent agent in this.Agents)
                    this.CyclesWithoutEating[agent]++;
        }

        protected virtual IList<Cell> GetStartPositionCellsClosestToFood()
        {
            var startPositionsDistanceToClosestFoodResource = new Dictionary<Cell, double>();
            var closestToFoodCells = new List<Cell>();

            foreach (var startPositionCell in this.possibleStartPositionCells)
            {
                //determines minimum distance to a food resource cell from this start position cell
                var cell = startPositionCell;
                var minDistance =
                    this.FoodResources.Min(
                        foodResource => this.GetDistanceBetweenCells(cell, foodResource.Cell));

                //stores information
                startPositionsDistanceToClosestFoodResource.Add(startPositionCell, minDistance);

                //checks first cell
                if (closestToFoodCells.Count == 0)
                    closestToFoodCells.Add(startPositionCell);
                else
                    //stores start position cell in correct position on the list according to its distance to closest food resource
                    for (var i = 0; i <= closestToFoodCells.Count; i++)
                    {
                        if ((i != closestToFoodCells.Count) &&
                            (minDistance >= startPositionsDistanceToClosestFoodResource[closestToFoodCells[i]]))
                            continue;
                        closestToFoodCells.Insert(i, startPositionCell);
                        break;
                    }
            }
            startPositionsDistanceToClosestFoodResource.Clear();

            return closestToFoodCells;
        }

        protected bool IsHungry(IAgent agent, IState state, IAction action)
        {
            return (state is IStimuliState) &&
                   ((IStimuliState)state).Sensations.Contains(FoodCyclesPerceptionManager.HUNGRY_SENSATION);
        }

        protected virtual void PlaceAgents()
        {
            if ((this.Agents == null) || (this.Agents.Count == 0)) return;

            //last agents to eat always start from positions closer to food
            var startPositionCellsClosestToFood = this.GetStartPositionCellsClosestToFood();
            var curClosestCellIndex = 0;
            var curFartherCellIndex = startPositionCellsClosestToFood.Count - 1;

            //positions every agent according to its "last-to-eat" status
            foreach (IFoodSharingAgent socialAgent in this.Agents)
            {
                socialAgent.Environment = this;
                if (this.CyclesWithoutEating[socialAgent] == 0)
                {
                    //if agent just ate, try to place it in a cell close to food
                    if (curClosestCellIndex <= curFartherCellIndex)
                    {
                        socialAgent.Cell = startPositionCellsClosestToFood[curClosestCellIndex++];
                        continue;
                    }
                }
                else
                {
                    //if agent didn't eat, try to place it in a cell far from food
                    if (curFartherCellIndex >= curClosestCellIndex)
                    {
                        socialAgent.Cell = startPositionCellsClosestToFood[curFartherCellIndex--];
                        continue;
                    }
                }

                //or else just put the agent in a fixed or random cell if not available
                socialAgent.Cell = (startPositionCellsClosestToFood.Count > 0)
                    ? startPositionCellsClosestToFood[0]
                    : this.GetRandomCell();
            }

            startPositionCellsClosestToFood.Clear();
        }

        protected virtual void PlaceFoodResources()
        {
            //places food resources according to possible cell locations
            for (var i = 0; i < this.FoodResources.Count; i++)
                this.FoodResources[i].Cell = i < this.possibleFoodResourceCells.Count
                    ? this.possibleFoodResourceCells[i]
                    : this.GetRandomCell();
        }

        #endregion Protected Methods
    }
}