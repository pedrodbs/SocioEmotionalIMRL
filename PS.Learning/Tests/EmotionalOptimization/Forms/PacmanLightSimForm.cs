// ------------------------------------------
// <copyright file="PacmanLightSimForm.cs" company="Pedro Sequeira">
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
//    Project: Learning.Tests.EmotionalOptimization

//    Last updated: 10/17/2012
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------
using System.Windows.Forms;
using PS.Learning.Forms.Simulation;
using PS.Learning.Core.Testing.SingleTests;
using PS.Learning.Tests.EmotionalOptimization.Domain.Environments;

namespace PS.Learning.Tests.EmotionalOptimization.Forms
{
    public partial class PacmanLightSimForm : SimulationDisplayForm
    {
        protected PictureBox[] lifePBoxes = new PictureBox[4];
        protected uint prevLives;

        public PacmanLightSimForm(FitnessTest test) : base(test, false)
        {
            this.InitializeComponent();
        }


        public override void Init()
        {
            this.lifePBoxes[0] = this.lifePBox1;
            this.lifePBoxes[1] = this.lifePBox2;
            this.lifePBoxes[2] = this.lifePBox3;
            this.lifePBoxes[3] = this.lifePBox4;

            base.Init();
        }

        protected override bool Step()
        {
            lock (this.locker)
            {
                var retValue = base.Step();

                //updates pacman lives info
                if (retValue) this.UpdateLives();

                return retValue;
            }
        }

        protected virtual void UpdateLives()
        {
            lock (this.locker)
            {
                if (!(this.environmentControl.Environment is PacmanEnvironment))
                    return;

                var curLives = System.Math.Max(
                    1, ((PacmanEnvironment) this.environmentControl.Environment).NumLives);
                if (curLives == this.prevLives) return;

                for (var i = 0; i < this.lifePBoxes.Length; i++)
                    this.lifePBoxes[i].Visible = i < curLives;

                this.prevLives = curLives;
            }
        }
    }
}