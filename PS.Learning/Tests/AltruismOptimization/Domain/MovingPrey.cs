// ------------------------------------------
// MovingPrey.cs, Learning.Tests.AltruismOptimization
//
// Created by Pedro Sequeira, 2014/2/24
//
// pedro.sequeira@gaips.inesc-id.pt
// ------------------------------------------
using System;
using MathNet.Numerics.Random;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Tests.AltruismOptimization.Domain.Environments;
using PS.Utilities.Core;

namespace PS.Learning.Tests.AltruismOptimization.Domain
{
    [Serializable]
    public class MovingPrey : CellElement, IUpdatable
    {
        public const double MOVE_PROB = 0.1;
        protected static readonly Random Random = new WH2006();
        protected readonly HuntingEnvironment environment;

        public MovingPrey(HuntingEnvironment environment)
        {
            this.environment = environment;
            this.MoveProb = MOVE_PROB;
        }

        public double MoveProb { get; set; }

        #region IUpdatable Members

        public void Update()
        {
            //checks for same cell, nothing to do
            if ((this.Cell == null) || (this.Cell.Environment == null)) return;

            //checks for move prob
            if (Random.NextDouble() > this.MoveProb) return;

            //updates prey movement based on hunters (agents) positions
            var medCell = ((ICellAgent)this.environment.Agents[0]).Cell;
            //this.environment.GetMedianWalkableCell(this.environment.Agents);
            if (medCell == null) return;
            var nextCell = this.environment.GetNextCellToFartherFromCell(this.Cell, medCell);
            if (nextCell != null)
                this.Cell = nextCell;
        }

        #endregion
    }
}