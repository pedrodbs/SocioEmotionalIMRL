// ------------------------------------------
// <copyright file="CellAgent.cs" company="Pedro Sequeira">
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
using PS.Learning.Core.Domain.Cells;
using PS.Learning.Core.Domain.Managers.Motivation;
using PS.Learning.Core.Domain.Managers.Perception;
using PS.Utilities.IO.Serialization;
using System;
using System.Xml;

namespace PS.Learning.Core.Domain.Agents
{
    [Serializable]
    public abstract class CellAgent : SituatedAgent, ICellAgent
    {
        #region Protected Fields

        protected const string MOVE_DOWN = "MoveDown";
        protected const string MOVE_LEFT = "MoveLeft";
        protected const string MOVE_RIGHT = "MoveRight";
        protected const string MOVE_UP = "MoveUp";
        protected Cell cell;

        #endregion Protected Fields

        #region Protected Constructors

        protected CellAgent(string label)
        {
            this.CellElement = new CellElement { Walkable = true };
            this.IdToken = label;
        }

        protected CellAgent() : this(AGENT_LABEL)
        {
        }

        #endregion Protected Constructors

        #region Public Properties

        public Cell Cell
        {
            get { return this.cell; }
            set
            {
                if (this.cell != null) this.cell.Elements.Remove(this);
                this.cell = value;

                if (value == null) return;

                this.cell.Elements.Add(this);
                if ((this.cell.Environment != null) && !this.cell.Environment.CellElements.ContainsKey(this.IdToken))
                    this.cell.Environment.CellElements.Add(this.IdToken, this);
            }
        }

        public ColorInfo Color
        {
            get { return this.CellElement.Color; }
            set { this.CellElement.Color = value; }
        }

        public string Description
        {
            get { return this.CellElement.Description; }
            set { this.CellElement.Description = value; }
        }

        //public Brush Brush
        //{
        //    get { return this.CellElement.Brush; }
        //}
        public bool ForceRepaint { get; set; }

        public bool HasSmell
        {
            get { return this.CellElement.HasSmell; }
            set { this.CellElement.HasSmell = value; }
        }

        public string IdToken
        {
            get { return this.CellElement.IdToken; }
            set { this.CellElement.IdToken = value; }
        }

        public string ImagePath
        {
            get { return this.CellElement.ImagePath; }
            set { this.CellElement.ImagePath = value; }
        }

        public new MotivationManager MotivationManager
        {
            get { return base.MotivationManager as MotivationManager; }
            protected set { base.MotivationManager = value; }
        }

        public PerceptionManager PerceptionManager { get; private set; }
        public double Reward { get; set; }

        //public Bitmap Image
        //{
        //    get { return this.CellElement.Image; }
        //}
        public bool Visible
        {
            get { return this.CellElement.Visible; }
            set { this.CellElement.Visible = value; }
        }

        public bool Walkable
        {
            get { return this.CellElement.Walkable; }
            set { this.CellElement.Walkable = value; }
        }

        #endregion Public Properties

        #region Protected Properties

        protected CellElement CellElement { get; }

        #endregion Protected Properties

        #region Public Methods

        public ICellElement Clone()
        {
            return this.CellElement.Clone();
        }

        public void DeserializeXML(XmlElement element)
        {
            this.CellElement.DeserializeXML(element);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.CellElement.Dispose();
            this.PerceptionManager.Dispose();
            this.MotivationManager.Dispose();
        }

        public bool Equals(ICellElement cellElem)
        {
            return (cellElem is CellAgent) && (((CellAgent)cellElem).IdToken == this.IdToken);
        }

        public virtual void InitElements()
        {
            this.CellElement.InitElements();
        }

        public override void PrintAll(string path, string imgFormat)
        {
            base.PrintAll(path, imgFormat);
            this.MotivationManager.PrintResults(path);
            this.PerceptionManager.PrintResults(path);
        }

        public void SerializeXML(XmlElement element)
        {
            this.CellElement.SerializeXML(element);
        }

        public override string ToString()
        {
            return this.IdToken;
        }

        public override void Update()
        {
            this.LogWriter.WriteLine("");

            this.ShortTermMemory.PreviousState = this.ShortTermMemory.CurrentState;

            var oldCell = this.Cell;

            this.BehaviorManager.Update();

            if ((oldCell != this.Cell) && (this.LogWriter != null))
                this.LogWriter.WriteLine($@"{oldCell.CellLocation.IdToken} -> {this.Cell.CellLocation.IdToken}: ");

            this.PerceptionManager.Update();
            this.ShortTermMemory.CurrentState = this.LongTermMemory.GetUpdatedCurrentState();

            this.MotivationManager.Update();
            var reward = this.MotivationManager.ExtrinsicReward.Value;

            this.ShortTermMemory.CurrentReward.Value = reward;

            this.LongTermMemory.Update();
            this.LearningManager.Update();
            this.ShortTermMemory.Update();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void CreateActions()
        {
            this.Actions.Add(MOVE_UP, new MoveUp(MOVE_UP, this));
            this.Actions.Add(MOVE_DOWN, new MoveDown(MOVE_DOWN, this));
            this.Actions.Add(MOVE_LEFT, new MoveLeft(MOVE_LEFT, this));
            this.Actions.Add(MOVE_RIGHT, new MoveRight(MOVE_RIGHT, this));
        }

        protected override void CreateManagers()
        {
            base.CreateManagers();
            this.PerceptionManager = this.CreatePerceptionManager();
        }

        protected override MotivationManager CreateMotivationManager()
        {
            return new EnvironmentMotivationManager(this);
        }

        protected virtual PerceptionManager CreatePerceptionManager()
        {
            return new NeighbourCellPerceptionManager(this);
        }

        #endregion Protected Methods
    }
}