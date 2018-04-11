// ------------------------------------------
// FoodCyclesPerceptionManager.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/3
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System.Globalization;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Managers
{
    public class FoodCyclesPerceptionManager : AltruismPerceptionManager
    {
        public const string LAST_TO_EAT_SENSATION = "last-to-eat";
        public const string WITHOUT_EATING_SENSATION = "cycles-withtout-eating";

        public FoodCyclesPerceptionManager(IFoodSharingAgent agent) : base(agent)
        {
        }

        protected override void AddInternalSensations()
        {
            //agent "knows" how many cycles have passed without eating
            var cyclesWithoutEating = this.Agent.Environment.CyclesWithoutEating[this.Agent];
            var cyclesWithoutEatingLimit = this.Agent.Environment.Agents.Count - 1;

            var lastToEatSensation = (cyclesWithoutEating == 0)
                ? LAST_TO_EAT_SENSATION
                : (((cyclesWithoutEating >= cyclesWithoutEatingLimit)
                    ? cyclesWithoutEatingLimit + "+"
                    : cyclesWithoutEating.ToString(CultureInfo.InvariantCulture)) +
                   WITHOUT_EATING_SENSATION);

            this.CurrentSensations.Add(lastToEatSensation);

            //adds satiation status
            this.CurrentSensations.Add(this.Agent.MotivationManager.Hunger.Value == 1
                ? HUNGRY_SENSATION
                : SATISFIED_SENSATION);
        }
    }
}