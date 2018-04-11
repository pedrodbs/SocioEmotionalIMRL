// ------------------------------------------
// <copyright file="AppraisalSet.cs" company="Pedro Sequeira">
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

using System;
using System.Collections.Generic;
using PS.Utilities.Math;

namespace PS.Learning.IMRL.Emotions
{
    public class AppraisalSet : IDisposable
    {
        public AppraisalSet()
        {
            this.Novelty = new StatisticalQuantity {Id = "Novelty"};
            this.Arousal = new StatisticalQuantity {Id = "Arousal"};
            this.Valence = new StatisticalQuantity {Id = "Valence"};
            this.GoalRelevance = new StatisticalQuantity {Id = "GoalRelevance"};
            this.Control = new StatisticalQuantity {Id = "Control"};

            this.Dimensions = new Dictionary<string, StatisticalQuantity>
                              {
                                  {this.Novelty.Id, this.Novelty},
                                  {this.Arousal.Id, this.Arousal},
                                  {this.Valence.Id, this.Valence},
                                  {this.GoalRelevance.Id, this.GoalRelevance},
                                  {this.Control.Id, this.Control}
                              };
        }

        public StatisticalQuantity Novelty { get; protected set; }
        public StatisticalQuantity Arousal { get; protected set; }
        public StatisticalQuantity Valence { get; protected set; }
        public StatisticalQuantity GoalRelevance { get; protected set; }
        public StatisticalQuantity Control { get; protected set; }
        public Dictionary<string, StatisticalQuantity> Dimensions { get; protected set; }

        #region IDisposable Members

        public virtual void Dispose()
        {
            foreach (var appraisalDimension in this.Dimensions.Values) appraisalDimension.Dispose();
        }

        #endregion

        public double DifferenceTo(AppraisalSet appraisalSet)
        {
            var sum = 0d;
            foreach (var appraisalDimensionId in this.Dimensions.Keys)
            {
                var diff = appraisalSet.Dimensions[appraisalDimensionId].Value -
                           this.Dimensions[appraisalDimensionId].Value;
                sum += diff*diff;
            }
            return Math.Sqrt(sum);
        }
    }
}