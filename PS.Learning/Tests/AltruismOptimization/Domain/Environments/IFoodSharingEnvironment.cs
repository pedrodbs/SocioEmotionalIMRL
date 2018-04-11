// ------------------------------------------
// IFoodSharingEnvironment.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/3/26
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System.Collections.Generic;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.States;
using PS.Learning.Social.Domain.Environments;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Environments
{
    public interface IFoodSharingEnvironment : ISocialEnvironment
    {
        Dictionary<IFoodSharingAgent, int> CyclesWithoutEating { get; }
        bool AutoEat { get; set; }
        List<CellElement> FoodResources { get; }
        bool AteFood(IFoodSharingAgent agent);
        bool AteFood(IAgent agent, IState state, IAction action);
    }
}