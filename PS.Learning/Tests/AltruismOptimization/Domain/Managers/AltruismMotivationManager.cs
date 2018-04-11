// ------------------------------------------
// AltruismMotivationManager.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/4
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using MathNet.Numerics.LinearAlgebra.Double;
using PS.Learning.Core.Domain;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.IMRL.Domain.Managers.Motivation;
using PS.Learning.Social.Domain;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Managers
{
    public class AltruismMotivationManager : ArrayParamMotivationManager, IHungerMotivationManager
    {
        #region Constructor

        public AltruismMotivationManager(ISocialAgent agent) : base((ICellAgent) agent)
        {
            this.Hunger = new Need("hunger", 1, 0, 0, 0) {Value = 1};
        }

        #endregion

        #region Properties

        public new ISocialAgent Agent
        {
            get { return base.Agent as ISocialAgent; }
        }

        public Need Hunger { get; protected set; }

        #endregion

        public override DenseVector GetRewardFeatures(uint prevState, uint action, uint nextState)
        {
            var altruismManager = (FoodSharingSocialManager) this.Agent.SocialManager;
            return new DenseVector(new[]
                                       {
                                           altruismManager.GetExternalLegitimacySignal(prevState, action, nextState),
                                           altruismManager.GetInternalLegitimacySignal(prevState, action, nextState),
                                           this.GetExtrinsicReward(prevState, action)
                                       });
        }
    }
}