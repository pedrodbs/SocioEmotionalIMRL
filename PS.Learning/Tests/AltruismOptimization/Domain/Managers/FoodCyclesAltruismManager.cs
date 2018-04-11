// ------------------------------------------
// FoodCyclesAltruismManager.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/4
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using PS.Learning.Tests.AltruismOptimization.Domain.Agents;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Managers
{
    public class FoodCyclesAltruismManager : FoodSharingSocialManager
    {
        protected int cyclesWithoutEating;

        public FoodCyclesAltruismManager(IFoodSharingAgent agent) : base(agent)
        {
        }


        public override void Update()
        {
            base.Update();

            //updates eating cycles
            this.cyclesWithoutEating = this.Agent.Environment.CyclesWithoutEating[this.Agent];
        }

        public override double GetInternalLegitimacySignal(uint prevState, uint action, uint nextState)
        {
            var socialObs = this.GetSocialObservations(prevState, action);

            //internal signal punishes for selfish acts and rewards if the agents see food and do not eat
            return socialObs.seeFood ? this.GetSocialSignalValue(socialObs) : 0;
        }

        public override double GetExternalLegitimacySignal(uint prevState, uint action, uint nextState)
        {
            var socialObs = this.GetSocialObservations(prevState, action);

            //external legitimacy signal is 1 if the agents was the last to eat food and does not ea
            // and sees food and the other agent decides to eat
            return socialObs.seeFood && socialObs.isOtherAgentActionEat
                       ? this.GetSocialSignalValue(socialObs)
                       : 0;
        }

        protected int GetSocialSignalValue(SocialObservations socialObs)
        {
            //signal is modulated by the number of cycles without eating
            var maxCyclesWithoutEating = this.Agent.Environment.Agents.Count - 1;
            var truncatedCyclesWithoutEating = System.Math.Min(maxCyclesWithoutEating, this.cyclesWithoutEating);
            var value = (maxCyclesWithoutEating - truncatedCyclesWithoutEating)/maxCyclesWithoutEating;
            return (socialObs.isAgentActionEat ? -1 : 1)*value;
        }

        public override double GetSocialSignal(uint prevState, uint action, uint nextState)
        {
            var socialObs = this.GetSocialObservations(prevState, action);
            return socialObs.seeOtherAgents.Count > 0 ? 1 : 0;
        }
    }
}