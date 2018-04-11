using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AForge.Genetic;
using Learning.Genetics.Testing;
using Microsoft.CSharp;

namespace Learning.Genetics
{
    public class Util
    {
        private static readonly GPTreeNode ZeroNode = new GPTreeNode(new ConstantGene(0));

        public static HashSet<ChromosomeParameters> GenerateAllCombinations(RewardChromosome chromosome)
        {
            var nodeStack = GetNodeStack(chromosome.ToString());
            var rootNode = nodeStack.Pop();
            ProcessNode(rootNode, nodeStack);

            return GenerateAllCombinations(rootNode);
        }

        public static HashSet<ChromosomeParameters> GenerateAllCombinations(GPTreeNode node)
        {
            var combChromosomes = new HashSet<ChromosomeParameters>();

            //tests end condition, ie no more children, add node itself
            if ((node.Children == null) || (node.Children.Count == 0))
            {
                combChromosomes.Add(new ChromosomeParameters {ProgramExpression = node.ToString()});
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
                var newNode = (GPTreeNode)node.Clone();
                newNode.Children[0] = child0;
                combChromosomes.Add(new ChromosomeParameters { ProgramExpression = newNode.ToString() });
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
                    combChromosomes.Add(new ChromosomeParameters {ProgramExpression = newNode.ToString()});
                }

            //if function is subtraction
            if (!node.Gene.ToString().Equals(RewardGeneFunction.GetFunctionTypeString(FunctionType.Subtract)))
                return combChromosomes;

            //consider right side negative by adding "0 - 'childNode'"
            foreach (var child1 in children1)
            {
                var newNode = (GPTreeNode) node.Clone();
                newNode.Children[0] = ZeroNode;
                newNode.Children[1] = child1;
                combChromosomes.Add(new ChromosomeParameters {ProgramExpression = newNode.ToString()});
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

        public static string ToLiteral(string input)
        {
            var writer = new StringWriter();
            var provider = new CSharpCodeProvider();
            provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
            return writer.GetStringBuilder().ToString().Replace("\"", "");
        }

        public static string ToNormalNotation(IChromosome chromosome)
        {
            return ToNormalNotation(ToLiteral(chromosome.ToString()));
        }

        public static string ToNormalNotation(string originalExpression)
        {
            var nodeStack = GetNodeStack(originalExpression);

            //creates children node logic for all nodes of the expression
            var rootNode = nodeStack.Pop();
            ProcessNode(rootNode, nodeStack);

            return ToNormalNotation(rootNode);
        }

        public static Stack<GPTreeNode> GetNodeStack(string originalExpression)
        {
            var customGene = new RewardGeneFunction(RewardGeneFunction.AllFunctions, 1);

            //gets the reverse (polish notation) expression and the elements in the expression
            var elements = originalExpression.Split(' ');

            //creates stack of nodes for each element in the reverse expression
            var nodeStack = new Stack<GPTreeNode>();
            foreach (var element in elements)
                if (!string.IsNullOrEmpty(element))
                    nodeStack.Push(IsZeroNode(element) ? ZeroNode : new GPTreeNode(customGene.CreateNew(element)));

            return nodeStack;
        }

        public static bool IsZeroNode(string element)
        {
            return element.Equals(ZeroNode.Gene.ToString());
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
                sb.Append(curNode.Gene.ToString());
            }

            return sb.ToString();
        }

        protected static void ProcessNode(GPTreeNode curNode, Stack<GPTreeNode> nodeStack)
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
                ProcessNode(childNode, nodeStack);
            }

            //reverse order of children for correct order
            if (argumentsCount == 0) return;
            curNode.Children.Reverse();
        }
    }
}