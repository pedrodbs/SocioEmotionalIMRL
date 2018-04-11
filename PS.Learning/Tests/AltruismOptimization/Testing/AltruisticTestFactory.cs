using System.Collections.Generic;
using Learning.Domain.Agents;
using Learning.Social.Domain;
using Learning.Social.Testing;
using Learning.Tests.AltruismOptimization.Domain.Agents;
using Learning.Tests.AltruismOptimization.Domain.Environments;

namespace Learning.Tests.AltruismOptimization.Testing
{
    public class AltruisticTestFactory : SocialTestFactory
    {
        public AltruisticTestFactory(uint sampleSteps, uint maxFoodResources) : base(sampleSteps)
        {
            this.MaxFoodResources = maxFoodResources;
        }

        public uint MaxFoodResources { get; private set; }

        public override IAgent CreateAgent()
        {
            return new AltruisticAgent {StatisticsCollection = {SampleSteps = this.SampleSteps}};
        }

        public override SocialEnvironment CreateEnvironment(IList<ISocialAgent> agents)
        {
            return new AltruisticEnvironment(agents, this.MaxFoodResources);
        }
    }
}