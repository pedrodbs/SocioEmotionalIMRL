// ------------------------------------------
// <copyright file="CorridorSocialPerceptionManager.cs" company="Pedro Sequeira">
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
//    Project: Learning.Social

//    Last updated: 12/5/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System;
using System.Collections.Generic;
using PS.Learning.Core.Domain.Agents;
using PS.Learning.Core.Domain.Managers.Perception;
using PS.Learning.Social.Domain.Environments;

namespace PS.Learning.Social.Domain.Managers
{
    public class CorridorSocialPerceptionManager : CellPerceptionManager
    {
        public CorridorSocialPerceptionManager(ICellAgent agent) : base(agent)
        {
            this.SeeOtherAgents = new HashSet<ISocialAgent>();
        }

        public HashSet<ISocialAgent> SeeOtherAgents { get; private set; }


        public override void Dispose()
        {
            base.Dispose();
            this.SeeOtherAgents.Clear();
            this.SeeOtherAgents = null;
        }

        protected override void AddExternalSensations()
        {
            base.AddExternalSensations();

            //detects other agents in corridors
            this.SeeOtherAgents.Clear();
            var agentX = this.Agent.Cell.XCoord;
            var agentY = this.Agent.Cell.YCoord;
            var env = (ISocialEnvironment) this.Agent.Environment;

            foreach (var otherAgent in env.Agents)
            {
                if (otherAgent.Equals(this.Agent))
                    continue;

                var elementID = otherAgent.IdToken;
                var sensation = otherAgent.IdToken;

                if (env.DetectInRightCorridor(agentX, agentY, elementID))
                {
                    this.SeeOtherAgents.Add(otherAgent);
                    this.CurrentSensations.Add(String.Format("{0}-r", sensation));
                }
                if (env.DetectInLeftCorridor(agentX, agentY, elementID))
                {
                    this.SeeOtherAgents.Add(otherAgent);
                    this.CurrentSensations.Add(String.Format("{0}-l", sensation));
                }
                if (env.DetectInUpCorridor(agentX, agentY, elementID))
                {
                    this.SeeOtherAgents.Add(otherAgent);
                    this.CurrentSensations.Add(String.Format("{0}-u", sensation));
                }
                if (env.DetectInDownCorridor(agentX, agentY, elementID))
                {
                    this.SeeOtherAgents.Add(otherAgent);
                    this.CurrentSensations.Add(String.Format("{0}-d", sensation));
                }
            }
        }
    }
}