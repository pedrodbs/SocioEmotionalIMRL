// ------------------------------------------
// TwoLeverEnvironment.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/17
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System;
using System.Linq;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Environments
{
    [Serializable]
    public class TwoLeverEnvironment : HeavyLeverEnvironment
    {
        public CellElement Lever2 { get; private set; }

        public override void Init()
        {
            base.Init();

            //creates the lever
            this.Lever2 = (CellElement) this.Lever.Clone();
        }

        public override void Update()
        {
            this.Lever2.IdToken = this.Lever.IdToken;

            base.Update();
        }

        protected override void PlaceLevers()
        {
            base.PlaceLevers();

            //places lever 2 in possible position
            this.Lever2.Cell = this.possibleLeverCells.Count > 1
                ? this.possibleLeverCells[1]
                : this.GetRandomCell();
        }

        protected override bool CheckAgentsInLevers()
        {
            //both levers have to be pressed (or have been pressed)
            this.bothInLevers =
                (this.bothInLevers ||
                 (this.Agents.Cast<IFoodSharingAgent>().Any(agent => agent.Cell.Equals(this.Lever.Cell)) &&
                  this.Agents.Cast<IFoodSharingAgent>().Any(agent => agent.Cell.Equals(this.Lever2.Cell))));

            //turns lights on/off
            foreach (var greenLight in this.greenLights)
            {
                greenLight.Visible = this.bothInLevers;
                greenLight.ForceRepaint = true;
            }

            return this.bothInLevers;
        }
    }
}