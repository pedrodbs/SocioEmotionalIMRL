// ------------------------------------------
// EchoMotivationManager.cs, Learning.Tests.AltruismGPOptimization
//
// Created by Pedro Sequeira, 2014/2/20
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using MathNet.Numerics.LinearAlgebra.Double;
using PS.Learning.Core.Domain;
using PS.Learning.IMRL.Domain.Managers.Motivation;
using PS.Learning.Social.Domain;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;

namespace PS.Learning.Tests.AltruismGPOptimization.Domain.Managers
{
    public class EchoMotivationManager : ArrayParamMotivationManager, IHungerMotivationManager
    {
        #region Constructor

        public EchoMotivationManager(IFoodSharingAgent agent)
            : base(agent)
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
            var altruismManager = (EchoSocialManager) this.Agent.SocialManager;
            return new DenseVector(new[]
                                       {
                                           altruismManager.GetOtherWorseSig(prevState, action, nextState),
                                           altruismManager.GetOtherHungerSignal(prevState, action, nextState),
                                           altruismManager.GetOtherPresSignal(prevState, action, nextState),
                                           altruismManager.GetExtEchoSignal(prevState, action, nextState),
                                           altruismManager.GetIntEchoSignal(prevState, action, nextState),
                                           altruismManager.GetIntPerfSignal(prevState, action, nextState),
                                           this.GetExtrinsicReward(prevState, action)
                                       });
        }
    }
}