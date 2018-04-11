// ------------------------------------------
// LeverEnvironment.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2012/11/23
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
    public class LeverEnvironment : FoodSharingEnvironment
    {
        #region Public Fields

        public const string LEVER_ID = "lever";

        #endregion Public Fields

        #region Protected Fields

        protected const string LEVER_CELL_ID_TOKEN = "lever-position";
        protected readonly List<Cell> possibleLeverCells = new List<Cell>();

        #endregion Protected Fields

        #region Public Properties

        public CellElement Lever { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public override void CreateCells(uint rows, uint cols, string configFile)
        {
            base.CreateCells(rows, cols, configFile);

            //gathers information about possible lever cells and creates electricity signs
            foreach (var cell in this.Cells)
                if (cell.Description.Contains(LEVER_CELL_ID_TOKEN))
                    this.possibleLeverCells.Add(cell);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.possibleLeverCells.Clear();
        }

        public override void Init()
        {
            base.Init();

            //creates the lever
            this.Lever = new CellElement
            {
                IdToken = LEVER_ID,
                Description = LEVER_ID,
                Reward = 0,
                Visible = true,
                Walkable = true,
                ImagePath = "../../../../bin/resources/lab/switch.png",
                Color = System.Drawing.Color.FromArgb(0, 0, 0).ToColorInfo(),
            };

            //changes image of food and visibility
            foreach (var foodResource in this.FoodResources)
            {
                foodResource.ImagePath = "../../../../bin/resources/lab/cheese.png";
                foodResource.Visible = false;
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.PlaceLevers();
        }

        public override void Update()
        {
            //checks whether agent(s) is/isn't in the lever (makes food visible/invisible)
            this.ShowFoodResources(this.CheckAgentsInLevers());

            base.Update();
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual bool CheckAgentsInLevers()
        {
            return this.Agents.Cast<IFoodSharingAgent>().Any(agent => Equals(agent.Cell, this.Lever.Cell));
        }

        protected override void PlaceFoodResources()
        {
            base.PlaceFoodResources();
            this.ShowFoodResources(false);
        }

        protected virtual void PlaceLevers()
        {
            //places lever in possible position
            this.Lever.Cell = this.possibleLeverCells.Count > 0
                ? this.possibleLeverCells[0]
                : this.GetRandomCell();
        }

        protected virtual void ShowFoodResources(bool show)
        {
            foreach (var foodResource in this.FoodResources)
            {
                foodResource.Visible = show;
                foodResource.ForceRepaint = true;
            }
        }

        #endregion Protected Methods
    }
}