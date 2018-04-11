// ------------------------------------------
// <copyright file="Util.cs" company="Pedro Sequeira">
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
//    Project: Learning.Social.IMRL.EC

//    Last updated: 03/26/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.Collections.Generic;
using System.Linq;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Learning.Social.IMRL.EC.Chromosomes;
using PS.Utilities.Collections;

namespace PS.Learning.Social.IMRL.EC
{
    public class Util
    {
        public static HashSet<IGPChromosome> GenerateAllCombinations(SocialGPChromosome socialChromosome)
        {
            var allSubCombinations = new GPChromosome[socialChromosome.NumAgents][];
            for (var i = 0; i < socialChromosome.NumAgents; i++)
            {
                //adds sub-element and all its combinations to list
                var gpChromosome = (GPChromosome) socialChromosome[i];
                var allChromCombs = Learning.IMRL.EC.Util.GenerateAllCombinations(gpChromosome).ToList();
                allChromCombs.Add(gpChromosome);
                var allChromCombsArray = new GPChromosome[allChromCombs.Count];
                for (var j = 0; j < allChromCombs.Count; j++)
                    allChromCombsArray[j] = (GPChromosome) allChromCombs[j];
                allSubCombinations[i] = allChromCombsArray;
            }

            //makes combinations with all chromosomes
            var chromCombs = allSubCombinations.AllCombinations();
            var allCombinations = new HashSet<IGPChromosome>();
            foreach (var chromComb in chromCombs)
                allCombinations.Add(new SocialGPChromosome(chromComb));

            return allCombinations;
        }
    }
}