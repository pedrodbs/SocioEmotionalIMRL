// ------------------------------------------
// <copyright file="Agent.cs" company="Pedro Sequeira">
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
using PS.Learning.Core.Domain.Managers.Behavior;
using PS.Learning.Core.Domain.Managers.Learning;
using PS.Learning.Core.Domain.Managers.Motivation;
using PS.Learning.Core.Domain.Memories;
using PS.Learning.Core.Testing.Config.Parameters;
using PS.Learning.Core.Testing.Config.Scenarios;
using PS.Utilities.Core;
using PS.Utilities.IO.Serialization;
using PS.Utilities.Math;
using System;
using System.Collections.Generic;
using System.IO;

namespace PS.Learning.Core.Domain.Agents
{
    [Serializable]
    public abstract class Agent : IAgent
    {
        #region Private Fields

        [NonSerialized]
        private LogWriter _logWriter;

        [NonSerialized]
        private StatisticsCollection _statisticsCollection;

        #endregion Private Fields

        #region Protected Constructors

        protected Agent()
        {
            this.StatisticsCollection = new StatisticsCollection();
            this.Actions = new Dictionary<string, IAction>();
        }

        #endregion Protected Constructors

        #region Public Properties

        public Dictionary<string, IAction> Actions { get; protected set; }

        public BehaviorManager BehaviorManager { get; private set; }

        public StatisticalQuantity Fitness { get; set; }

        string IIdentifiableObject.Description { get; set; }

        string IIdentifiableObject.IdToken { get; set; }

        public LearningManager LearningManager { get; private set; }

        public LogWriter LogWriter
        {
            get { return this._logWriter; }
            set { this._logWriter = value; }
        }

        public LongTermMemory LongTermMemory { get; private set; }

        public IMotivationManager MotivationManager { get; protected set; }

        public IScenario Scenario { get; set; }

        public ShortTermMemory ShortTermMemory { get; private set; }

        public StatisticsCollection StatisticsCollection
        {
            get { return this._statisticsCollection; }
            protected set { this._statisticsCollection = value; }
        }

        public ITestParameters TestParameters { get; set; }

        #endregion Public Properties

        #region Protected Properties

        protected virtual string MemoryBaseFilePath => Path.GetFullPath(
            $"{this.Scenario.TestsConfig.MemoryBaseFilePath}{Path.DirectorySeparatorChar}LTM");

        #endregion Protected Properties

        #region Public Methods

        public IAgent CreateNew()
        {
            return this.CreateInstance();
        }

        public virtual void Dispose()
        {
            this.BehaviorManager.Dispose();
            this.LearningManager.Dispose();
            this.LongTermMemory.Dispose();
            this.ShortTermMemory.Dispose();
            this.Actions.Clear();
            this.StatisticsCollection.Dispose();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is Agent)) return false;
            return ((IIdentifiableObject)obj).IdToken.Equals(((IIdentifiableObject)this).IdToken);
        }

        public override int GetHashCode()
        {
            return ((IIdentifiableObject)this).IdToken.GetHashCode();
        }

        public virtual void Init()
        {
            this.Actions.Clear();
            this.CreateActions();
            this.CreateMemories();
            this.CreateManagers();
            this.BehaviorManager.Init();
            this.InitStatisticsCollection();
        }

        public virtual void PrintAll(string path, string imgFormat)
        {
            //this.StatisticsCollection.PrintAllQuantities(path + "/OverallStatistics.xls");
            this.LongTermMemory.ImgFormat = imgFormat;
            this.LongTermMemory.PrintResults(path);
            this.ShortTermMemory.PrintResults(path);
            this.LearningManager.PrintResults(path);
            this.BehaviorManager.PrintResults(path);
        }

        public virtual void Reset()
        {
            this.LongTermMemory.Reset();
            this.ShortTermMemory.Reset();
            this.LearningManager.Reset();
            this.BehaviorManager.Reset();
            this.MotivationManager.Reset();

            var sampleSteps = this.StatisticsCollection.SampleSteps;
            var numSamples = this.StatisticsCollection.MaxNumSamples;
            this.StatisticsCollection = new StatisticsCollection
            {
                SampleSteps = sampleSteps,
                MaxNumSamples = numSamples
            };
            this.AddStatisticalQuantities();
            this.StatisticsCollection.InitParameters();
        }

        public abstract void Update();

        #endregion Public Methods

        #region Protected Methods

        protected virtual void AddStatisticalQuantities()
        {
            //adds statistical quantities to the log file
            //this.StatisticsCollection.Add("PredictionError", this.ShortTermMemory.PredictionErrorAbs);
            this.StatisticsCollection.Add("Reward", this.ShortTermMemory.CurrentReward);
            //this.StatisticsCollection.Add("LearningRate", this.LearningManager.LearningRate);
            //this.StatisticsCollection.Add("Discount", this.LearningManager.Discount);
            this.StatisticsCollection.Add("StateActionValue", this.ShortTermMemory.CurrentStateActionValue);
            this.StatisticsCollection.Add("ExtrinsicReward", this.MotivationManager.ExtrinsicReward);
            this.StatisticsCollection.Add("NumTasks", this.LongTermMemory.NumTasks);

            //adds actions statistics
            this.StatisticsCollection.AddRange(this.BehaviorManager.ActionStatistics);
        }

        protected abstract void CreateActions();

        protected abstract BehaviorManager CreateBehaviorManager();

        protected virtual LearningManager CreateLearningManager()
        {
            return new QLearningManager(this, this.LongTermMemory);
        }

        protected abstract LongTermMemory CreateLongTermMemory();

        protected virtual void CreateManagers()
        {
            this.BehaviorManager = this.CreateBehaviorManager();
            this.LearningManager = this.CreateLearningManager();
            this.MotivationManager = this.CreateMotivationManager();
        }

        protected virtual void CreateMemories()
        {
            this.ShortTermMemory = this.CreateShortTermMemory();
            this.LongTermMemory = this.CreateLongTermMemory();
            this.LongTermMemory.Reset();
            this.LongTermMemory.ReadAllStats(this.MemoryBaseFilePath);
        }

        protected abstract MotivationManager CreateMotivationManager();

        protected virtual ShortTermMemory CreateShortTermMemory()
        {
            return new ShortTermMemory(this);
        }

        protected virtual void InitStatisticsCollection()
        {
            this.AddStatisticalQuantities();
            var testsConfig = this.Scenario.TestsConfig;
            this.StatisticsCollection.SampleSteps = testsConfig.NumTimeSteps / testsConfig.NumSamples;
            this.StatisticsCollection.MaxNumSamples = testsConfig.NumSamples;
            this.StatisticsCollection.InitParameters();
        }

        #endregion Protected Methods
    }
}