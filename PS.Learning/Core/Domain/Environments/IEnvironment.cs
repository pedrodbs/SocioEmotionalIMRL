// ------------------------------------------
// <copyright file="IEnvironment.cs" company="Pedro Sequeira">
// 
//     Copyright (c) 2018 Pedro Sequeira
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// </copyright>
// <summary>
//    Project: PS.Learning.Core

//    Last updated: 10/05/2015
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.States;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Utilities.Core;
using PS.Utilities.IO.Serialization;
using System;
using System.Collections.Generic;

namespace PS.Learning.Core.Domain.Environments
{
    public interface IEnvironment : IDisposable, IUpdatable, IXmlSerializable, IInitializable
    {
        #region Public Properties

        Dictionary<string, ICellElement> CellElements { get; }
        Cell[,] Cells { get; }
        ColorInfo Color { get; }
        uint Cols { get; }
        bool DebugMode { get; set; }
        uint Rows { get; }
        IScenario Scenario { get; set; }
        List<Cell> TargetCells { get; }

        #endregion Public Properties

        #region Public Methods

        bool AgentFinishedTask(IAgent agent, IState state, IAction action);

        void CreateCells(uint cols, uint rows, string environmentConfigFile);

        IEnvironment CreateNew();

        bool DetectInCell(int x, int y, string elementID);

        bool DetectInCell(Cell cell, string elementID);

        bool DetectInCells(IList<Cell> cells, string elementID);

        bool DetectInCorridors(int startPosX, int startPosY, string elementID);

        bool DetectInDownCorridor(int startPosX, int startPosY, string elementID);

        bool DetectInLeftCorridor(int startPosX, int startPosY, string elementID);

        bool DetectInRightCorridor(int startPosX, int startPosY, string elementID);

        bool DetectInUpCorridor(int startPosX, int startPosY, string elementID);

        double GetAgentReward(IAgent agent, IState state, IAction action);

        Cell GetCell(int xCoord, int yCoord);

        Cell GetClosestWalkableCell(int xCoord, int yCoord);

        IList<Cell> GetCornerCells();

        double GetCurrentDistanceToTargetCell(ICellAgent agent);

        double GetDistanceBetweenCells(Cell source, Cell target);

        double GetDistanceToTargetCell(Cell source);

        IList<Cell> GetDownCells(int startPosX, int startPosY);

        Cell GetFarthestCell(ICellElement fromElement);

        Cell GetFarthestCell(Cell fromElement);

        IList<Cell> GetLeftCells(int startPosX, int startPosY);

        Cell GetMedianWalkableCell(IEnumerable<ICellElement> elements);

        IList<Cell> GetNeighbourCells(Cell source);

        Cell GetNextCellInShortPath(Cell source, Cell target);

        Cell GetNextCellToFartherFromCell(Cell startCell, Cell fromCell);

        Cell GetRandomCell();

        Cell GetRandomCell(IEnumerable<Cell> forbiddenCells, IEnumerable<Cell> possibleCells);

        IList<Cell> GetRightCells(int startPosX, int startPosY);

        IList<Cell> GetShortPathBetweenCells(Cell source, Cell target);

        IList<Cell> GetUpCells(int startPosX, int startPosY);

        bool IsValidCoord(int xCoord, int yCoord);

        bool IsValidXCoord(int xCoord);

        bool IsValidYCoord(int yCoord);

        bool IsWalkable(Cell cell);

        bool IsWalkable(int x, int y);

        void LoadFromXmlFile(string xmlFileName);

        void MoveFromElement(ICellElement element, ICellElement fromElement);

        void MoveRandomly(ICellElement element);

        void MoveToElement(ICellElement element, ICellElement toElement);

        void Reset();

        void SaveToXmlFile(string xmlFileName);

        #endregion Public Methods
    }
}