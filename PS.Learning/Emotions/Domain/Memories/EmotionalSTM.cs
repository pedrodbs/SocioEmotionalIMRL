// ------------------------------------------
// <copyright file="EmotionalSTM.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL.Emotions

//    Last updated: 10/09/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.IO;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Memories;
using PS.Utilities.Math;

namespace PS.Learning.IMRL.Emotions.Domain.Memories
{
    public class EmotionalSTM : ShortTermMemory
    {
        public EmotionalSTM(IAgent agent) : base(agent)
        {
            this.AppraisalSet = new AppraisalSet();
            this.Clarity = new StatisticalQuantity {Id = "Clarity"};
            this.Mood = new StatisticalQuantity {Id = "Mood"};
        }

        public double Novelty
        {
            get { return this.AppraisalSet.Novelty.Value; }
        }

        public double Arousal
        {
            get { return this.AppraisalSet.Arousal.Value; }
        }

        public double Valence
        {
            get { return this.AppraisalSet.Valence.Value; }
        }

        public string EmotionLabel { get; set; }
        public bool WriteDimensionsLog { get; set; }

        public double Motivation
        {
            get { return this.AppraisalSet.GoalRelevance.Value; }
        }

        public double Control
        {
            get { return this.AppraisalSet.Control.Value; }
        }

        public StatisticalQuantity Clarity { get; protected set; }
        public StatisticalQuantity Mood { get; protected set; }
        public AppraisalSet AppraisalSet { get; protected set; }

        public override void PrintResults(string path)
        {
            base.PrintResults(path);
            //this.PrintDimensionsStats(path + "/STM");
        }

        public override void Dispose()
        {
            base.Dispose();
            this.AppraisalSet.Dispose();
            this.Mood.Dispose();
            this.Clarity.Dispose();
        }

        public override void Reset()
        {
            base.Reset();
            this.AppraisalSet = new AppraisalSet();
            this.Clarity = new StatisticalQuantity {Id = "Clarity"};
            this.Mood = new StatisticalQuantity {Id = "Mood"};
        }

        public override void Update()
        {
            base.Update();

            if (this.Agent.LogWriter != null)
                this.Agent.LogWriter.WriteLine("Emotion label: " + this.EmotionLabel);
        }

        protected void PrintDimensionsStats(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            //prints appraisal dimensions
            foreach (var dimension in this.AppraisalSet.Dimensions.Values)
                dimension.PrintStatisticsToCSV(string.Format("{0}/{1}.csv", path, dimension.Id));

            //prints other dimensions
            this.Clarity.PrintStatisticsToCSV(string.Format("{0}/{1}.csv", path, this.Clarity.Id));
            this.Mood.PrintStatisticsToCSV(string.Format("{0}/{1}.csv", path, this.Mood.Id));
        }
    }
}