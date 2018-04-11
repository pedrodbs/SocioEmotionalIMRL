// ------------------------------------------
// SimpleEchoGPSocialAgent.cs, Learning.Tests.AltruismGPOptimization
//
// Created by Pedro Sequeira, 2014/2/20
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System;

namespace PS.Learning.Tests.AltruismGPOptimization.Domain.Agents
{
    [Serializable]
    public class SimpleEchoGPSocialAgent : EchoGPSocialAgent
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