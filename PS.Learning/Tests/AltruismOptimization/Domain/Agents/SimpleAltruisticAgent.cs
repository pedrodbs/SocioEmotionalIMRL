// ------------------------------------------
// SimpleAltruisticAgent.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/18
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
namespace PS.Learning.Tests.AltruismOptimization.Domain.Agents
{
    public class SimpleAltruisticAgent : AltruisticAgent
    {
        protected override void CreateActions()
        {
            base.CreateActions();

            //removes up and down actions
            this.Actions.Remove(MOVE_UP);
            this.Actions.Remove(MOVE_DOWN);
        }
    }
}