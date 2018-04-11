// ------------------------------------------
// <copyright file="Environment.cs" company="Pedro Sequeira">
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

using MathNet.Numerics.Random;
using PS.Learning.Core.Domain.Actions;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.States;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Utilities.Core;
using PS.Utilities.IO.Serialization;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.ShortestPath;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace PS.Learning.Core.Domain.Environments
{
    [Serializable]
    public abstract class Environment : XmlResource, IEnvironment
    {
        #region Protected Fields

        protected const double INFINITE_DISTANCE = double.MaxValue;

        [NonSerialized]
        protected readonly Random rand = new WH2006(true);

        [NonSerialized]
        protected AdjacencyGraph<Cell, Edge<Cell>> cellGraph = new AdjacencyGraph<Cell, Edge<Cell>>();

        [NonSerialized]
        protected FloydWarshallAllShortestPathAlgorithm<Cell, Edge<Cell>> shortestPathFWAlg;

        #endregion Protected Fields

        #region Private Fields

        private const string CELL_TAG = "cell";

        private const string CELLS_TAG = "cells";

        private const string COLOR_TAG = "color";

        private const string ELEMENT_TAG = "cell-element";

        private const string ELEMENTS_TAG = "cell-elements";

        private const string NUM_COLS_TAG = "num-cols";

        private const string NUM_ROWS_TAG = "num-rows";

        private const string XML_ROOT_NAME = "environment";

        private bool _debugMode;

        [NonSerialized]
        private LogWriter _logWriter;

        #endregion Private Fields

        #region Protected Constructors

        protected Environment(uint rows, uint cols)
        {
            this.Color = new ColorInfo(255, 255, 255);
            this.Rows = rows;
            this.Cols = cols;
            this.CellElements = new Dictionary<string, ICellElement>();
            this.TargetCells = new List<Cell>();
        }

        #endregion Protected Constructors

        #region Public Properties

        public Dictionary<string, ICellElement> CellElements { get; }

        public Cell[,] Cells { get; protected set; }

        public ColorInfo Color { get; protected set; }

        public uint Cols { get; protected set; }

        public bool DebugMode
        {
            get { return this._debugMode; }
            set
            {
                this._debugMode = value;
                if (this.Cells != null)
                    foreach (var cell in this.Cells)
                        cell.CellLocation.Visible = cell.StateValue.Visible = value;
            }
        }

        public LogWriter LogWriter
        {
            get { return this._logWriter; }
            set { this._logWriter = value; }
        }

        public uint Rows { get; protected set; }

        public IScenario Scenario { get; set; }

        public List<Cell> TargetCells { get; protected set; }

        #endregion Public Properties

        #region Protected Properties

        protected ITestsConfig TestsConfig
        {
            get { return this.Scenario.TestsConfig; }
        }

        #endregion Protected Properties

        #region Public Methods

        public abstract bool AgentFinishedTask(IAgent agent, IState state, IAction action);

        public virtual void CreateCells(uint rows, uint cols, string configFile)
        {
            if (string.IsNullOrEmpty(configFile) || !File.Exists(configFile))
                this.CreateDefaultGridCells(rows, cols);
            else if (configFile.EndsWith("csv"))
                this.ReadConfigFile(configFile);
            else if (configFile.EndsWith("xml"))
                this.LoadFromXmlFile(configFile);
            else this.CreateDefaultGridCells(rows, cols);

            this.SetNeighbourCells();

            //creates cell distance graph
            this.UpdateCellGraph();
        }

        public virtual void CreateCells(uint rows, uint cols)
        {
            this.CreateCells(rows, cols, "");
        }

        public IEnvironment CreateNew()
        {
            return CloneUtil.CreateInstance(this);
        }

        public override void DeserializeXML(XmlElement element)
        {
            base.DeserializeXML(element);

            this.InitElements();

            //reads num cells
            this.Cols = uint.Parse(element.GetAttribute(NUM_COLS_TAG), CultureInfo.InvariantCulture);
            this.Rows = uint.Parse(element.GetAttribute(NUM_ROWS_TAG), CultureInfo.InvariantCulture);

            var colorElem = (XmlElement)element.SelectSingleNode(COLOR_TAG);
            if (colorElem != null) this.Color.DeserializeXML(colorElem);

            //reads cell elements
            foreach (XmlElement childElement in element.SelectNodes(ELEMENTS_TAG + "/" + ELEMENT_TAG))
            {
                var cellElement = new CellElement();
                cellElement.DeserializeXML(childElement);

                //adds cell elements to list
                if (!cellElement.IdToken.Equals("agent") && !this.CellElements.ContainsKey(cellElement.IdToken))
                    this.CellElements.Add(cellElement.IdToken, cellElement);
            }

            //reads environment cells
            this.Cells = new Cell[this.Cols, this.Rows];

            //reads cells
            foreach (XmlElement childElement in element.SelectNodes(CELLS_TAG + "/" + CELL_TAG))
            {
                var cell = this.CreateCell(0, 0);
                cell.DeserializeXML(childElement);

                if (this.Cells[cell.XCoord, cell.YCoord] != null) continue;

                this.Cells[cell.XCoord, cell.YCoord] = cell;
                foreach (var elementID in cell.ElementIDs)
                {
                    if (this.CellElements.ContainsKey(elementID))
                        //cell.Elements.Add(this.CellElements[elementID]);
                        this.CellElements[elementID].Clone().Cell = cell;
                }
            }
        }

        public bool DetectInCell(int x, int y, string elementID)
        {
            return this.DetectInCell(this.Cells[x, y], elementID);
        }

        public bool DetectInCell(Cell cell, string elementID)
        {
            var element = cell.GetElement(elementID);
            return (element != null) && element.Visible;
        }

        public bool DetectInCells(IList<Cell> cells, string elementID)
        {
            for (var i = 0; i < cells.Count; i++)
                if (!this.IsWalkable(cells[i]))
                    break;
                else if (this.DetectInCell(cells[i], elementID))
                    return true;

            return false;
        }

        public bool DetectInCorridors(int startPosX, int startPosY, string elementID)
        {
            return
                this.DetectInLeftCorridor(startPosX, startPosY, elementID) ||
                this.DetectInRightCorridor(startPosX, startPosY, elementID) ||
                this.DetectInUpCorridor(startPosX, startPosY, elementID) ||
                this.DetectInDownCorridor(startPosX, startPosY, elementID);
        }

        public bool DetectInDownCorridor(int startPosX, int startPosY, string elementID)
        {
            return this.DetectInCells(this.GetDownCells(startPosX, startPosY), elementID);
        }

        public bool DetectInLeftCorridor(int startPosX, int startPosY, string elementID)
        {
            return this.DetectInCells(this.GetLeftCells(startPosX, startPosY), elementID);
        }

        public bool DetectInRightCorridor(int startPosX, int startPosY, string elementID)
        {
            return this.DetectInCells(this.GetRightCells(startPosX, startPosY), elementID);
        }

        public bool DetectInUpCorridor(int startPosX, int startPosY, string elementID)
        {
            return this.DetectInCells(this.GetUpCells(startPosX, startPosY), elementID);
        }

        public override void Dispose()
        {
            if (this.LogWriter != null) this.LogWriter.Close();
            foreach (var cell in this.Cells) cell.Dispose();
            this.cellGraph.Clear();
        }

        public abstract double GetAgentReward(IAgent agent, IState state, IAction action);

        public Cell GetCell(int xCoord, int yCoord)
        {
            return this.IsValidCoord(xCoord, yCoord)
                ? this.Cells[xCoord, yCoord]
                : null;
        }

        public Cell GetClosestWalkableCell(int xCoord, int yCoord)
        {
            var dist = 0;
            while (((xCoord - dist >= 0) || (xCoord + dist < this.Cols)) &&
                   ((yCoord - dist >= 0) || (yCoord + dist < this.Rows)))
            {
                //checks horizontal lines
                for (var vert = yCoord - dist; (dist != 0) && (vert <= yCoord + dist); vert += 2 * dist)
                    for (var horiz = xCoord - dist; horiz <= xCoord + dist; horiz++)
                    {
                        var curCell = this.GetCell(vert, horiz);
                        if ((curCell != null) && this.IsWalkable(curCell))
                            return curCell;
                    }

                //checks vertical lines
                for (var horiz = xCoord - dist; (dist != 0) && (horiz <= xCoord + dist); horiz += 2 * dist)
                    for (var vert = yCoord - dist; vert <= yCoord + dist; vert++)
                    {
                        var curCell = this.GetCell(vert, horiz);
                        if ((curCell != null) && this.IsWalkable(curCell))
                            return curCell;
                    }

                //increases distance
                dist++;
            }
            return null;
        }

        public virtual IList<Cell> GetCornerCells()
        {
            return new List<Cell>
                   {
                       this.Cells[1, 1],
                       this.Cells[this.Cols - 2, this.Rows - 2],
                       this.Cells[this.Cols - 2, 1],
                       this.Cells[1, this.Rows - 2]
                   };
        }

        public virtual double GetCurrentDistanceToTargetCell(ICellAgent agent)
        {
            return this.GetDistanceToTargetCell(agent.Cell);
        }

        public virtual double GetDistanceBetweenCells(Cell source, Cell target)
        {
            if ((source == null) || (target == null))
                return INFINITE_DISTANCE;
            if (source.Equals(target))
                return 0;

            double dist;

            //tries to return "walkable" distance between cells
            if ((this.cellGraph == null) || (this.shortestPathFWAlg == null) ||
                (this.shortestPathFWAlg.State != ComputationState.Finished) ||
                !this.shortestPathFWAlg.TryGetDistance(source, target, out dist))
                return INFINITE_DISTANCE;

            return dist;
        }

        public virtual double GetDistanceToTargetCell(Cell source)
        {
            if ((this.TargetCells == null) || (this.TargetCells.Count == 0)) return INFINITE_DISTANCE;
            return this.TargetCells.Min(targetCell => this.GetDistanceBetweenCells(source, targetCell));
        }

        public IList<Cell> GetDownCells(int startPosX, int startPosY)
        {
            var cells = new List<Cell>();
            for (var y = startPosY; y < this.Rows; y++)
                cells.Add(this.Cells[startPosX, y]);
            return cells;
        }

        public Cell GetFarthestCell(ICellElement fromElement)
        {
            return fromElement.Cell != null ? this.GetFarthestCell(fromElement.Cell) : null;
        }

        public Cell GetFarthestCell(Cell fromElement)
        {
            Cell farCell = null;
            var maxDistance = double.MinValue;
            foreach (var cell in this.Cells)
            {
                if (!this.IsWalkable(cell)) continue;
                var distance = this.GetDistanceBetweenCells(cell, fromElement);
                if (distance <= maxDistance) continue;
                maxDistance = distance;
                farCell = cell;
            }
            return farCell;
        }

        public IList<Cell> GetLeftCells(int startPosX, int startPosY)
        {
            var cells = new List<Cell>();
            for (var x = startPosX; x >= 0; x--)
                cells.Add(this.Cells[x, startPosY]);
            return cells;
        }

        public Cell GetMedianWalkableCell(IEnumerable<ICellElement> elements)
        {
            var xSum = 0;
            var ySum = 0;
            var numCells = 0;
            foreach (var element in elements)
                if (element.Cell != null)
                {
                    xSum += element.Cell.XCoord;
                    ySum += element.Cell.YCoord;
                    numCells++;
                }
            var xMed = xSum / numCells;
            var yMed = ySum / numCells;
            return this.GetClosestWalkableCell(xMed, yMed);
        }

        public virtual IList<Cell> GetNeighbourCells(Cell source)
        {
            //gets up, down, left, right cells
            var neighbourCells = new List<Cell>
                                 {
                                     (source.YCoord > 0)
                                         ? this.Cells[source.XCoord, source.YCoord - 1]
                                         : null,
                                     (source.YCoord < this.Rows - 1)
                                         ? this.Cells[source.XCoord, source.YCoord + 1]
                                         : null,
                                     (source.XCoord > 0)
                                         ? this.Cells[source.XCoord - 1, source.YCoord]
                                         : null,
                                     (source.XCoord < this.Cols - 1)
                                         ? this.Cells[source.XCoord + 1, source.YCoord]
                                         : null
                                 };
            return neighbourCells;
        }

        public virtual Cell GetNextCellInShortPath(Cell source, Cell target)
        {
            var cellPath = this.GetShortPathBetweenCells(source, target);
            return ((cellPath == null) || (cellPath.Count == 0)) ? null : cellPath[0];
        }

        public virtual Cell GetNextCellToFartherFromCell(Cell startCell, Cell fromCell)
        {
            if ((startCell == null) || (fromCell == null)) return null;

            //gets farthest cell from element
            var farCell = this.GetFarthestCell(fromCell);
            return farCell == null ? null : this.GetNextCellInShortPath(startCell, farCell);
        }

        public IList<Cell> GetPathBetweenCells(
            Cell source, Cell target, FloydWarshallAllShortestPathAlgorithm<Cell, Edge<Cell>> pathAlgorithm)
        {
            if ((source == null) || (target == null) || (source == target))
                return null;

            IEnumerable<Edge<Cell>> edges;

            //tries to return "walkable" path between cells
            if ((this.cellGraph == null) || (pathAlgorithm == null) ||
                (pathAlgorithm.State != ComputationState.Finished) ||
                !pathAlgorithm.TryGetPath(source, target, out edges))
                return null;

            return edges.Select(edge => edge.Target).ToList();
        }

        public virtual Cell GetRandomCell()
        {
            Cell randomCell;
            while (true)
            {
                randomCell = this.Cells[this.rand.Next((int)this.Cols), this.rand.Next((int)this.Rows)];
                if (randomCell.Elements.Any(cellElement => !cellElement.Walkable))
                    continue;
                break;
            }
            return randomCell;
        }

        public Cell GetRandomCell(IEnumerable<Cell> forbiddenCells, IEnumerable<Cell> possibleCells)
        {
            //checks possible cells
            if ((possibleCells == null) || (possibleCells.Count() == 0))
                throw new ArgumentException(@"Must provide possible cells", "possibleCells");

            //checks forbidden cells
            if (forbiddenCells == null)
                forbiddenCells = new List<Cell>();

            //gets random cell from possible cell set
            var cellSet = new HashSet<Cell>(possibleCells);
            cellSet.ExceptWith(forbiddenCells);
            return cellSet.ElementAt(this.rand.Next(cellSet.Count));
        }

        public IList<Cell> GetRightCells(int startPosX, int startPosY)
        {
            var cells = new List<Cell>();
            for (var x = startPosX; x < this.Cols; x++)
                cells.Add(this.Cells[x, startPosY]);
            return cells;
        }

        public virtual IList<Cell> GetShortPathBetweenCells(Cell source, Cell target)
        {
            return this.GetPathBetweenCells(source, target, this.shortestPathFWAlg);
        }

        public IList<Cell> GetUpCells(int startPosX, int startPosY)
        {
            var cells = new List<Cell>();
            for (var y = startPosY; y >= 0; y--)
                cells.Add(this.Cells[startPosX, y]);
            return cells;
        }

        public abstract void Init();

        public override void InitElements()
        {
            this.Color = new ColorInfo(255, 255, 255);
        }

        public bool IsValidCoord(int xCoord, int yCoord)
        {
            return this.IsValidXCoord(xCoord) && this.IsValidYCoord(yCoord);
        }

        public bool IsValidXCoord(int xCoord)
        {
            return (xCoord >= 0) && (xCoord < this.Cols);
        }

        public bool IsValidYCoord(int yCoord)
        {
            return (yCoord >= 0) && (yCoord < this.Rows);
        }

        public virtual bool IsWalkable(Cell cell)
        {
            return cell.Elements.All(cellElement => cellElement.Walkable);
        }

        public virtual bool IsWalkable(int x, int y)
        {
            return this.IsWalkable(this.Cells[x, y]);
        }

        public override void LoadFromXmlFile(string xmlFileName)
        {
            this.LoadFromXmlFile(xmlFileName, XML_ROOT_NAME);
        }

        public virtual void MoveFromElement(ICellElement element, ICellElement fromElement)
        {
            if ((element == null) || (fromElement == null)) return;

            var cellFartherFromElement = this.GetNextCellToFartherFromCell(element.Cell, fromElement.Cell);
            if (cellFartherFromElement != null)
                element.Cell = cellFartherFromElement;
        }

        public virtual void MoveRandomly(ICellElement element)
        {
            if (element == null) return;
            var neighbourCells = this.GetNeighbourCells(element.Cell);
            var randNeighbour = neighbourCells[this.rand.Next(neighbourCells.Count)];
            if ((randNeighbour != null) && this.IsWalkable(randNeighbour))
                element.Cell = randNeighbour;
        }

        public virtual void MoveToElement(ICellElement element, ICellElement toElement)
        {
            if ((element == null) || (toElement == null)) return;
            var cellCloserToElement = this.GetNextCellInShortPath(element.Cell, toElement.Cell);
            if (cellCloserToElement != null)
                element.Cell = cellCloserToElement;
        }

        public void ReadConfigFile(string configWorldFile)
        {
            var sr = new StreamReader(configWorldFile);

            uint x = 0;
            uint y = 0;

            var tempCells = new List<List<Cell>>();
            var line = sr.ReadLine();
            while (line != null)
            {
                var cells = line.Split(';');
                tempCells.Add(new List<Cell>());

                foreach (var cell in cells)
                {
                    var newCell = this.CreateCell(x, y);
                    newCell.InitElements();

                    //checks for wall
                    if ((cell != null) && (cell[0] == '#'))
                        new CellElement
                        {
                            IdToken = "wall",
                            Reward = 0,
                            Visible = true,
                            Walkable = false,
                            ImagePath = "./resources/wall.jpg"
                        };

                    tempCells[(int)y].Add(newCell);
                    x++;
                }

                this.Cols = Math.Max(this.Cols, x);
                x = 0;
                y++;
                line = sr.ReadLine();
            }
            this.Rows = y;

            this.Cells = new Cell[this.Cols, this.Rows];

            for (y = 0; y < tempCells.Count; y++)
                for (x = 0; x < tempCells[(int)y].Count; x++)
                    this.Cells[x, y] = tempCells[(int)y][(int)x];

            sr.Close();
        }

        public virtual void Reset()
        {
            if (this.LogWriter != null)
            {
                this.LogWriter.WriteLine(@"****************************************************");
                this.LogWriter.WriteLine(@"Test run terminated: resetting...");
            }
        }

        public void SaveToCSV(string path)
        {
            var sw = new StreamWriter(path + "/worldcfg.csv");
            for (var i = 0; i < this.Rows; i++)
            {
                var line = "";
                for (var j = 0; j < this.Cols; j++)
                {
                    foreach (var cellContent in this.Cells[j, i].Elements)
                        line += cellContent.IdToken + " - ";

                    line = line.Remove(line.Length - 3) + ";";
                }
                sw.WriteLine(line);
            }
            sw.Close();
        }

        public override void SaveToXmlFile(string xmlFileName)
        {
            this.SaveToXmlFile(xmlFileName, XML_ROOT_NAME);
        }

        public override void SerializeXML(XmlElement element)
        {
            base.SerializeXML(element);

            //writes num cells
            element.SetAttribute(NUM_COLS_TAG, this.Cols.ToString(CultureInfo.InvariantCulture));
            element.SetAttribute(NUM_ROWS_TAG, this.Rows.ToString(CultureInfo.InvariantCulture));

            var colorElem = element.OwnerDocument.CreateElement(COLOR_TAG);
            this.Color.SerializeXML(colorElem);
            element.AppendChild(colorElem);

            //writes cell elements info
            var childElement = element.OwnerDocument.CreateElement(ELEMENTS_TAG);
            element.AppendChild(childElement);
            foreach (var cellElement in this.CellElements.Values)
            {
                var newChild = element.OwnerDocument.CreateElement(ELEMENT_TAG);
                cellElement.SerializeXML(newChild);
                childElement.AppendChild(newChild);
            }

            //writes cell info
            childElement = element.OwnerDocument.CreateElement(CELLS_TAG);
            element.AppendChild(childElement);
            foreach (var cell in this.Cells)
            {
                var newChild = element.OwnerDocument.CreateElement(CELL_TAG);
                cell.SerializeXML(newChild);
                childElement.AppendChild(newChild);
            }
        }

        public abstract void Update();

        #endregion Public Methods

        #region Protected Methods

        protected virtual Cell CreateCell(uint x, uint y)
        {
            var cell = new Cell(this) { XCoord = (int)x, YCoord = (int)y };
            cell.InitElements();
            return cell;
        }

        protected virtual void CreateDefaultGridCells(uint rows, uint cols)
        {
            this.Rows = rows;
            this.Cols = cols;
            this.Cells = new Cell[cols, rows];
            for (uint x = 0; x < cols; x++)
                for (uint y = 0; y < rows; y++)
                    this.Cells[x, y] = this.CreateCell(x, y);
        }

        protected virtual void SetNeighbourCells()
        {
            foreach (var cell in this.Cells)
                cell.SetNeighbourCells();
        }

        protected virtual void UpdateCellGraph()
        {
            this.cellGraph.Clear();

            //add vertexs
            foreach (var cell in this.Cells)
                this.cellGraph.AddVertex(cell);

            //adds edges (possible path between cells)
            foreach (var cell in this.Cells)
            {
                //non-walkable cells don't have connections
                if ((cell.Elements == null) || cell.Elements.Any(element => !element.Walkable)) continue;

                //gets neighbour connections
                var leftCell = cell.XCoord > 0 ? this.Cells[cell.XCoord - 1, cell.YCoord] : null;
                var rightCell = cell.XCoord < (this.Cols - 1) ? this.Cells[cell.XCoord + 1, cell.YCoord] : null;
                var upCell = cell.YCoord > 0 ? this.Cells[cell.XCoord, cell.YCoord - 1] : null;
                var downCell = cell.YCoord < (this.Rows - 1) ? this.Cells[cell.XCoord, cell.YCoord + 1] : null;

                //tests neighbour connections and add edges
                if ((leftCell != null) && !leftCell.Elements.Any(element => !element.Walkable))
                    this.cellGraph.AddEdge(new Edge<Cell>(cell, leftCell));
                if ((rightCell != null) && !rightCell.Elements.Any(element => !element.Walkable))
                    this.cellGraph.AddEdge(new Edge<Cell>(cell, rightCell));
                if ((upCell != null) && !upCell.Elements.Any(element => !element.Walkable))
                    this.cellGraph.AddEdge(new Edge<Cell>(cell, upCell));
                if ((downCell != null) && !downCell.Elements.Any(element => !element.Walkable))
                    this.cellGraph.AddEdge(new Edge<Cell>(cell, downCell));
            }

            // constant cost between cells
            Func<Edge<Cell>, double> edgeCost = e => 1;

            // creates and computes shortest paths
            this.shortestPathFWAlg =
                new FloydWarshallAllShortestPathAlgorithm<Cell, Edge<Cell>>(this.cellGraph, edgeCost);
            this.shortestPathFWAlg.Compute();
        }

        #endregion Protected Methods
    }
}