// ------------------------------------------
// IDetectOthersPerceptionManager.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2014/2/20
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using PS.Learning.Core.Domain.Managers;
using PS.Learning.Core.Domain.States;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Managers
{
    public interface IDetectOthersPerceptionManager : IManager
    {
        bool SeeOtherAgents(IState state);
    }
}