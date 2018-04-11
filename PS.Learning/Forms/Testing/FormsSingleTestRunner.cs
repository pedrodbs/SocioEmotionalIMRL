// ------------------------------------------
// <copyright file="FormsSingleTestRunner.cs" company="Pedro Sequeira">
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
//    Project: Learning.Forms
//    Last updated: 03/10/2014
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.Collections.Generic;
using System.Windows.Forms;
using PS.Learning.Forms.Simulation;
using PS.Learning.Core.Testing.Config;
using PS.Learning.Core.Testing.Runners;
using PS.Learning.Core.Testing.SingleTests;
using PS.Utilities.Forms;

namespace PS.Learning.Forms.Testing
{
    public class FormsSingleTestRunner : SingleTestRunner
    {
        private const string OPTION_CONSOLE = "Run on &console";
        private const string OPTION_FORM = "Run visual &form";
        private const string OPTIONS_FORM_TITLE = "Options for single test: ";

        public FormsSingleTestRunner(ITestsConfig testsConfig) : base(testsConfig)
        {
        }

        protected override void RunSimulation(FitnessTest test)
        {
            //tests execution only on console
            if (this.ForceConsole || !this.TestsConfig.GraphicsEnabled)
            {
                this.RunConsoleApplication(test);
                return;
            }

            //shows menu
            InitApplication();
            using (var chooseForm =
                new ChooseOptionForm(new List<string> {OPTION_CONSOLE, OPTION_FORM})
                {Text = OPTIONS_FORM_TITLE + test.TestName})
            {
                chooseForm.Show();
                while (!chooseForm.IsDisposed)
                    Application.DoEvents();

                switch (chooseForm.ChosenOption)
                {
                    case OPTION_FORM:
                        this.RunFormApplication(this.TestsConfig.CellSize, false, test);
                        break;
                    case OPTION_CONSOLE:
                        this.RunConsoleApplication(test);
                        break;
                    default:
                        this.RunFormApplication(this.TestsConfig.CellSize, true, test);
                        break;
                }
            }
        }

        protected override void RunConsoleApplication(FitnessTest test)
        {
            //also shows progress form
            using (new ProgressFormUpdater(test)
                   {
                       Visible = this.TestsConfig.GraphicsEnabled,
                       Text = test.TestName
                   })
            {
                base.RunConsoleApplication(test);
            }
        }

        protected virtual void RunFormApplication(int cellSize, bool limitedUser, FitnessTest test)
        {
            //creates form application
            var simulationForm = this.CreateSimulationForm(test, limitedUser);
            simulationForm.Text = test.TestName;
            simulationForm.CellSize = cellSize;

            //runs player form application
            simulationForm.Init();
            simulationForm.PauseResumeSimulation();
            Application.Run((Form) simulationForm);
        }

        protected virtual ISimulationForm CreateSimulationForm(FitnessTest test, bool limitedUser)
        {
            return new SimulationDisplayForm(test, limitedUser);
        }

        private static void InitApplication()
        {
            Application.EnableVisualStyles();
        }
    }
}