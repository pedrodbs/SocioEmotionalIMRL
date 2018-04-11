// ------------------------------------------
// HeavyLeverEnvironment.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2013/12/17
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Tests.AltruismOptimization.Domain.Agents;
using PS.Utilities.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PS.Learning.Tests.AltruismOptimization.Domain.Environments
{
    [Serializable]
    public class HeavyLeverEnvironment : LeverEnvironment
    {
        #region Protected Fields

        protected readonly List<CellElement> greenLights = new List<CellElement>();
        protected bool bothInLevers;

        #endregion Protected Fields

        #region Private Fields

        private const string GREEN_LIGHT_ID = "green-light";

        #endregion Private Fields

        #region Public Methods

        public override void CreateCells(uint rows, uint cols, string configFile)
        {
            base.CreateCells(rows, cols, configFile);

            //gathers information about possible lever cells and creates electricity signs
            foreach (var cell in this.Cells)
                if (this.IsWalkable(cell))
                    this.greenLights.Add(new CellElement
                    {
                        IdToken = GREEN_LIGHT_ID,
                        Description = GREEN_LIGHT_ID,
                        Visible = false,
                        Walkable = true,
                        ImagePath = "../../../../bin/resources/lab/green-light.png",
                        Color = System.Drawing.Color.FromArgb(255, 0, 255, 43).ToColorInfo(),
                        Cell = cell
                    });
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var greenLight in this.greenLights)
                greenLight.Dispose();
            this.greenLights.Clear();
        }

        public override void Reset()
        {
            base.Reset();

            //resets levers when someone eats
            this.bothInLevers = false;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override bool CheckAgentsInLevers()
        {
            //both levers have to be pressed (or have been pressed)
            this.bothInLevers =
                (this.bothInLevers ||
                 this.Agents.Cast<IFoodSharingAgent>().All(agent => agent.Cell.Equals(this.Lever.Cell)));

            this.TurnLights(this.bothInLevers);

            return this.bothInLevers;
        }

        protected override void PlaceLevers()
        {
            base.PlaceLevers();
            this.TurnLights(false);
        }

        protected virtual void TurnLights(bool value)
        {
            //turns lights on/off
            foreach (var greenLight in this.greenLights)
            {
                greenLight.Visible = value;
                greenLight.ForceRepaint = true;
            }
        }

        #endregion Protected Methods
    }
}