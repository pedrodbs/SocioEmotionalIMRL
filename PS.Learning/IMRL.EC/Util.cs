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
//    Project: Learning.IMRL.EC

//    Last updated: 03/26/2013
//    Author: Pedro Sequeira
//    E-mail: pedrodbs@gmail.com
// </summary>
// ------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AForge.Genetic;
using PS.Learning.IMRL.EC.Chromosomes;
using PS.Learning.IMRL.EC.Genes;
using PS.Utilities.Core;

namespace PS.Learning.IMRL.EC
{
    public class Util
    {
        private static readonly GPTreeNode ZeroNode = new GPTreeNode(new ConstantGPGene(0));

        public static HashSet<IGPChromosome> GenerateAllCombinations(GPChromosome chromosome)
        {
            var nodeStack = GetNodeStack(chromosome.ToString());
            if ((nodeStack == null) || (nodeStack.Count == 0))
                return new HashSet<IGPChromosome>();

            var rootNode = nodeStack.Pop();
            GenerateNodeStructure(rootNode, nodeStack);

            return GenerateAllCombinations(rootNode);
        }

        public static HashSet<IGPChromosome> GenerateAllCombinations(GPTreeNode node)
        {
            var combChromosomes = new HashSet<IGPChromosome>();

            //tests end condition, ie no more children, add node itself
            if ((node.Children == null) || (node.Children.Count == 0))
            {
                combChromosomes.Add(new GPExpressionChromosome(node.ToString()));
                return combChromosomes;
            }

            //add all combinations from each child
            foreach (var child in node.Children)
                combChromosomes.UnionWith(GenerateAllCombinations(child));

            //for one argument functions, gets children
            var children0 = GetAllChildren(node.Children[0]);

            //creates combinations for all children
            foreach (var child0 in children0)
            {
                var newNode = (GPTreeNode) node.Clone();
                newNode.Children[0] = child0;
                combChromosomes.Add(new GPExpressionChromosome(newNode.ToString()));
            }

            //for two argument functions
            if (node.Children.Count < 2) return combChromosomes;

            //gets children from both sides
            var children1 = GetAllChildren(node.Children[1]);

            //creates combinations between the different children
            foreach (var child0 in children0)
                foreach (var child1 in children1)
                {
                    var newNode = (GPTreeNode) node.Clone();
                    newNode.Children[0] = child0;
                    newNode.Children[1] = child1;
                    combChromosomes.Add(new GPExpressionChromosome(newNode.ToString()));
                }

            //if function is subtraction
            if (!node.Gene.ToString().Equals(FlexibleGPGene.GetFunctionTypeString(FunctionType.Subtract)))
                return combChromosomes;

            //consider right side negative by adding "0 - 'childNode'"
            foreach (var child1 in children1)
            {
                var newNode = (GPTreeNode) node.Clone();
                newNode.Children[0] = ZeroNode;
                newNode.Children[1] = child1;
                combChromosomes.Add(new GPExpressionChromosome(newNode.ToString()));
            }

            return combChromosomes;
        }

        public static List<GPTreeNode> GetAllChildren(GPTreeNode node)
        {
            var children = new List<GPTreeNode> {node};
            if ((node.Children != null) && (node.Children.Count > 0))
                foreach (var child in node.Children)
                    children.AddRange(GetAllChildren(child));
            return children;
        }

        public static Stack<GPTreeNode> GetNodeStack(string originalExpression)
        {
            if (string.IsNullOrWhiteSpace(originalExpression)) return null;

            var customGene = new FlexibleGPGene(FlexibleGPGene.AllFunctions.ToList(), 1);

            //gets the reverse (polish notation) expression and the elements in the expression
            var elements = originalExpression.Split(' ');

            //creates stack of nodes for each element in the reverse expression
            var nodeStack = new Stack<GPTreeNode>();
            foreach (var element in elements)
                if (!string.IsNullOrEmpty(element))
                    nodeStack.Push(IsZeroNode(element) ? ZeroNode : new GPTreeNode(customGene.CreateNew(element)));

            return nodeStack;
        }

        public static uint GetDepth(GPChromosome chromosome)
        {
            var nodeStack = GetNodeStack(chromosome.ToString());
            if ((nodeStack == null) || (nodeStack.Count == 0)) return 0;
            var rootNode = nodeStack.Pop();
            GenerateNodeStructure(rootNode, nodeStack);

            return GetDepth(rootNode, 0);
        }

        public static uint GetDepth(GPTreeNode curNode, uint curDepth)
        {
            //for each child determines depth by adding one level, return max child depth
            if ((curNode.Children != null) && (curNode.Children.Count > 0))
                return curNode.Children.Max(child => GetDepth(child, curDepth + 1));

            //or else just return current depth
            return curDepth;
        }

        public static bool IsZeroNode(string element)
        {
            return element.Equals(ZeroNode.Gene.ToString());
        }

        public static string ToNormalNotation(GPChromosome chromosome)
        {
            return ToNormalNotation(StringUtil.ToLiteral(chromosome.ToString()));
        }

        public static string ToNormalNotation(string originalExpression)
        {
            var nodeStack = GetNodeStack(originalExpression);

            //creates children node logic for all nodes of the expression
            var rootNode = nodeStack.Pop();
            GenerateNodeStructure(rootNode, nodeStack);

            return ToNormalNotation(rootNode);
        }

        public static string ToNormalNotation(GPTreeNode curNode)
        {
            var sb = new StringBuilder();

            if ((curNode.Children != null) && (curNode.Children.Count > 0))
            {
                sb.Append("(");
                if (curNode.Children.Count == 1)
                    //if node's gene is a 1-arg function, write (op child1) 
                    sb.AppendFormat("{0} {1}", curNode.Gene, ToNormalNotation(curNode.Children[0]));
                else
                //else, write expression (child1 op child2)
                    sb.AppendFormat("{0} {1} {2}",
                        ToNormalNotation(curNode.Children[0]), curNode.Gene,
                        ToNormalNotation(curNode.Children[1]));
                sb.Append(")");
            }
            else
            {
                //else just write the gene string rep
                sb.Append(curNode.Gene);
            }

            return sb.ToString();
        }

        protected static void GenerateNodeStructure(GPTreeNode curNode, Stack<GPTreeNode> nodeStack)
        {
            //processes each expected child node
            var argumentsCount = curNode.Gene.ArgumentsCount;
            for (var i = 0; i < argumentsCount; i++)
            {
                if (nodeStack.Count == 0) return;

                //gets child node from stack, if possible, and processes that node
                var childNode = nodeStack.Pop();
                if (curNode.Children == null) curNode.Children = new List<GPTreeNode>(argumentsCount);
                curNode.Children.Add(childNode);
                GenerateNodeStructure(childNode, nodeStack);
            }

            //reverse order of children for correct order
            if (argumentsCount == 0) return;
            curNode.Children.Reverse();
        }

        public static string GetTranslatedExpression(
            string chromosomeExpression, double[] constants, string[] variables)
        {
            //gets expression in inverse polish notation
            var inverseExpression = StringUtil.ToLiteral(chromosomeExpression).Replace("\"", "");

            //gets expression in normal notation
            var normalExpression = ToNormalNotation(inverseExpression);

            //replaces the operands (arguments) in the expression by a constant value or variable name
            //reverse order is used to prevent e.g. $11 substitution with $1
            var numVariables = variables.Length;
            var numInputElements = numVariables + constants.Length;
            for (var i = numInputElements - 1; i >= 0; i--)
            {
                normalExpression = normalExpression.Replace(
                    string.Format("${0}", i),
                    (i >= numVariables
                        ? constants[i - numVariables].ToString(CultureInfo.InvariantCulture)
                        : variables[i]));
            }
            return normalExpression;
        }
    }
}