// ------------------------------------------
// AltruismPerceptionManager.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2014/2/20
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System.Linq;
using PS.Learning.Core.Domain.Managers.Perception;
using PS.Learning.Core.Domain.States;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;
using PS.Learning.Tests.AltruismOptimization.Domain.Environments;
using PS.Learning.Tests.AltruismOptimization.Testing;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Managers
{
    public class AltruismPerceptionManager : CellPerceptionManager, IDetectOthersPerceptionManager
    {
        public const string HUNGRY_SENSATION = "hungry";
        public const string SATISFIED_SENSATION = "satisfied";

        public AltruismPerceptionManager(IFoodSharingAgent agent)
            : base(agent)
        {
        }

        public new IFoodSharingAgent Agent
        {
            get { return base.Agent as IFoodSharingAgent; }
        }

        #region IDetectOthersPerceptionManager Members

        public virtual bool SeeOtherAgents(IState state)
        {
            return Agent.Environment.Agents.Any(otherAgent =>
                (otherAgent != Agent) &&
                ((IStimuliState) state).Sensations.Contains(otherAgent.IdToken));
        }

        #endregion

        protected override void AddExternalSensations()
        {
            base.AddExternalSensations();

            if (!((IFoodSharingScenario) this.Scenario).SeeFoodFromAfar) return;

            //adds locations of all resources
            foreach (var foodResource in this.Agent.Environment.FoodResources)
                //this.CurrentSensations.Add(string.Format("{0},{1}", foodResource.IdToken, foodResource.Cell));
                this.DetectInCorridor(foodResource.IdToken, FoodSharingEnvironment.FOOD_ID);
        }

        //public virtual List<Cell> GetFoodCellsFromState(IState state)
        //{
        //    var cells = new List<Cell>();
        //    if (!(state is StringState)) return cells;

        //    //this.XCoord + "*" + this.YCoord
        //    string[] sensations;
        //    foreach (var sensation in ((IStimuliState) state).Sensations)
        //        if (sensation.Contains("*") && ((sensations = sensation.Split('*')).Length == 3))
        //            cells.Add(this.Agent.Environment.Cells[int.Parse(sensations[1]), int.Parse(sensations[2])]);
        //    return cells;
    }
}