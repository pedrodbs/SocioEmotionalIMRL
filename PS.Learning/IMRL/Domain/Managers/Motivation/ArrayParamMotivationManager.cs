// ------------------------------------------
// <copyright file="ArrayParamMotivationManager.cs" company="Pedro Sequeira">
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
//    Project: Learning.IMRL

//    Last updated: 07/11/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using MathNet.Numerics.LinearAlgebra.Double;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Testing.Config.Parameters;

namespace PS.Learning.IMRL.Domain.Managers.Motivation
{
    public abstract class ArrayParamMotivationManager : IntrinsicMotivationManager
    {
        protected ArrayParamMotivationManager(ICellAgent agent) : base(agent)
        {
            this.RewardParameters = ((IArrayParameter) this.Agent.TestParameters).Array;
        }

        public DenseVector RewardParameters { get; set; }

        public override double GetIntrinsicReward(uint prevState, uint action, uint nextState)
        {
            //gets the reward given by the weighted sum of all features
            return this.RewardParameters*this.GetRewardFeatures(prevState, action, nextState);
        }
    }
}