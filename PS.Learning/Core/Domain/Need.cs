// ------------------------------------------
// <copyright file="Need.cs" company="Pedro Sequeira">
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
//    Project: Learning
//    Last updated: 10/9/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
namespace PS.Learning.Core.Domain
{
    public class Need
    {
        protected uint value;

        public Need(string name, uint maxValue, double highReward, double mediumReward, double lowReward)
        {
            this.Name = name;
            this.MaxValue = maxValue;
            this.HighLevelReward = highReward;
            this.LowLevelReward = lowReward;
            this.MediumLevelReward = mediumReward;
        }

        public string HighLevelSensation
        {
            get { return this.Name + "-high"; }
        }

        public string MediumLevelSensation
        {
            get { return this.Name + "-medium"; }
        }

        public string LowLevelSensation
        {
            get { return this.Name + "-low"; }
        }

        public double HighLevelReward { get; protected set; }
        public double MediumLevelReward { get; protected set; }
        public double LowLevelReward { get; protected set; }
        public uint MaxValue { get; protected set; }
        public string Name { get; protected set; }

        public uint Value
        {
            get { return this.value; }
            set { if ((value >= 0) && (value <= this.MaxValue)) this.value = value; }
        }

        public double Reward
        {
            get
            {
                var percent = (double) this.Value/this.MaxValue;
                if (percent < (1f/3f))
                    return this.LowLevelReward;
                return percent < (2f/3f) ? this.MediumLevelReward : this.HighLevelReward;
            }
        }

        public string Sensation
        {
            get
            {
                var percent = (double) this.Value/this.MaxValue;
                if (percent < (1f/3f))
                    return this.LowLevelSensation;
                return percent < (2f/3f) ? this.MediumLevelSensation : this.HighLevelSensation;
            }
        }
    }
}