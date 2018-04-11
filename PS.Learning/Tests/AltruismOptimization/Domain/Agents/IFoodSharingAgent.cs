// ------------------------------------------
// IFoodSharingAgent.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/3
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------

using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Memories;
using PS.Learning.IMRL.Domain.Managers.Motivation;
using PS.Learning.Social.Domain;
using PS.Learning.Tests.AltruismOptimization.Domain.Environments;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Agents
{
    public interface IFoodSharingAgent : ISocialAgent, ICellAgent
    {
        bool IsStronger { get; set; }

        new IFoodSharingEnvironment Environment { get; set; }

        new IHungerMotivationManager MotivationManager { get; }

        new ShortTermMemory ShortTermMemory { get; }

        //new IFoodSharingTestProfile TestProfile { get; }
    }
}