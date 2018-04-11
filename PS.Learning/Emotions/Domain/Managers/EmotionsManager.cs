// ------------------------------------------
// <copyright file="EmotionsManager.cs" company="Pedro Sequeira">
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
//    Project: Learning.Emotions

//    Last updated: 10/10/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Managers;
using PS.Learning.IMRL.Emotions.Domain.Memories;
using PS.Utilities.Math;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace PS.Learning.IMRL.Emotions.Domain.Managers
{
    [Serializable]
    public abstract class EmotionsManager : Manager
    {
        #region Protected Fields

        protected EmotionalSTM emotionalSTM;

        #endregion Protected Fields

        #region Protected Constructors

        protected EmotionsManager(IAgent agent)
            : base(agent)
        {
            this.EmotionLabelsCount = new StatisticsCollection();
            this.EmotionLabels = new Dictionary<string, EmotionLabel>();
            //this.ReadEmotionLabelsConfig();
            //this.InitEmotionLabelCounts();
            this.NoveltyDecay = 1.001f;
            this.emotionalSTM = this.Agent == null
                ? null
                : ((EmotionalSTM)this.Agent.ShortTermMemory);
        }

        #endregion Protected Constructors

        #region Public Properties

        public Dictionary<string, EmotionLabel> EmotionLabels { get; private set; }
        public StatisticsCollection EmotionLabelsCount { get; private set; }
        public double NoveltyDecay { get; set; }

        #endregion Public Properties

        #region Protected Properties

        protected AppraisalSet AppraisalSet
        {
            get { return this.emotionalSTM.AppraisalSet; }
        }

        protected string EmotionLabel
        {
            get { return this.emotionalSTM.EmotionLabel; }
            set { this.emotionalSTM.EmotionLabel = value; }
        }

        protected StatisticalQuantity PredictionError
        {
            get { return this.Agent.ShortTermMemory.PredictionErrorAbs; }
        }

        #endregion Protected Properties

        #region Public Methods

        public override void Dispose()
        {
            this.EmotionLabels.Clear();
            this.EmotionLabelsCount.Clear();
        }

        public abstract double GetArousal(uint prevState, uint action, uint newState);

        public abstract double GetControl(uint prevState, uint action, uint newState);

        public abstract double GetGoalRelevance(uint prevState, uint action, uint newState);

        public abstract double GetMood(uint prevState, uint action, uint newState);

        public abstract double GetNovelty(uint prevState, uint action, uint newState);

        public abstract double GetValence(uint prevState, uint action, uint newState);

        public override void PrintResults(string path)
        {
            path += "/Emotions";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            this.EmotionLabelsCount.PrintAllQuantitiesCountToCSV(path + "/EmotionsCount.xls");
            this.EmotionLabelsCount.PrintAllQuantitiesToCSV(path + "/EmotionLabel.csv");
            this.PrintEmotionDimensions(path + "/EmotionDimensions.csv");
        }

        public override void Reset()
        {
            this.EmotionLabelsCount.Clear();
            this.InitEmotionLabelCounts();
        }

        public override void Update()
        {
            var stm = this.emotionalSTM;
            if (stm.PreviousState == null) return;
            this.UpdateAppraisal(stm.PreviousState.ID, stm.CurrentAction.ID, stm.CurrentState.ID);
        }

        #endregion Public Methods

        #region Protected Methods

        protected void InitEmotionLabelCounts()
        {
            if (this.Agent == null) return;

            //creates statistical quantities for emotion labels count
            foreach (var emotionLabel in this.EmotionLabels.Keys)
            {
                this.EmotionLabelsCount[emotionLabel] =
                    new StatisticalQuantity
                    {
                        SampleSteps = this.Agent.StatisticsCollection.SampleSteps,
                        MaxNumSamples = this.Agent.StatisticsCollection.MaxNumSamples
                    };
            }
        }

        protected void PrintEmotionDimensions(string filePath)
        {
            if (File.Exists(filePath)) File.Delete(filePath);

            var allDimensions = new StatisticsCollection();
            allDimensions.Add(this.emotionalSTM.Mood.Id, this.emotionalSTM.Mood);
            allDimensions.Add(this.emotionalSTM.Clarity.Id, this.emotionalSTM.Clarity);
            foreach (var dimension in this.emotionalSTM.AppraisalSet.Dimensions.Values)
                allDimensions.Add(dimension.Id, dimension);

            allDimensions.PrintAllQuantitiesToCSV(filePath);
        }

        protected void PrintEmotionLabelsCountToCSV(string filePath)
        {
            filePath += "/EmotionsCount.csv";

            if (File.Exists(filePath))
                File.Delete(filePath);

            var sw = new StreamWriter(filePath);
            sw.WriteLine("Label; Count");
            foreach (var emotionCount in this.EmotionLabelsCount)
                sw.WriteLine("{0};{1}", emotionCount.Key, emotionCount.Value.Sum);
            sw.Close();
        }

        protected void ReadEmotionLabelsConfig()
        {
            var fileName = Path.GetFullPath("../../../../bin/config/emotions.csv");
            if (!File.Exists(fileName)) return;

            var sr = new StreamReader(fileName);
            var line = sr.ReadLine();
            if (line == null) return;

            var dimensionIds = line.Split(';');
            if (dimensionIds.Length < 2) return;

            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("//")) continue;

                var parameters = line.Split(';');
                if (parameters.Length != dimensionIds.Length) continue;

                var name = parameters[0];
                var emotionLabel = new EmotionLabel(name);
                for (var i = 1; i < dimensionIds.Length; i++)
                {
                    var dimensionId = dimensionIds[i];
                    var dimensionValue = Convert.ToSingle(parameters[i], CultureInfo.InvariantCulture);
                    emotionLabel.Dimensions[dimensionId].Value = dimensionValue;
                }
                this.EmotionLabels.Add(name, emotionLabel);
            }
        }

        protected virtual void UpdateAppraisal(uint prevState, uint action, uint newState)
        {
            //updates dimensions
            this.AppraisalSet.Arousal.Value = this.GetArousal(prevState, action, newState);
            this.AppraisalSet.GoalRelevance.Value = this.GetGoalRelevance(prevState, action, newState);
            this.AppraisalSet.Novelty.Value = this.GetNovelty(prevState, action, newState);
            this.AppraisalSet.Control.Value = this.GetControl(prevState, action, newState);
            this.AppraisalSet.Valence.Value = this.GetValence(prevState, action, newState);

            ////updates label
            //this.UpdateEmotionLabel();
        }

        protected virtual void UpdateEmotionLabel()
        {
            this.EmotionLabel = "ND";

            //finds the label corresponding to the minimal distance
            var minDistance = double.MaxValue;
            foreach (var emotionLabel in this.EmotionLabels.Values)
            {
                var distance = this.AppraisalSet.DifferenceTo(emotionLabel);
                if (distance >= minDistance) continue;
                minDistance = distance;
                this.EmotionLabel = emotionLabel.Label;
            }

            //checks label
            if (this.EmotionLabel == "ND") return;

            //updates label count (1 for current emotion label, 0 for the rest)
            foreach (var emotionLabelCount in this.EmotionLabelsCount)
                emotionLabelCount.Value.Value = emotionLabelCount.Key.Equals(this.EmotionLabel) ? 1 : 0;
        }

        #endregion Protected Methods
    }
}