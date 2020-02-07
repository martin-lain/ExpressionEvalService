using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressionEvalService.BL
{
    /// <summary>
    /// Logic for parsing of string expression
    /// </summary>
    public static class Evaluator
    {
        private static readonly char[] operators = { '+', '-', '*', '/', '%', '^', '(', ')' };
        private static IFormatProvider culture = CultureInfo.InvariantCulture;
   
        public static bool IsOperator(char chr)
        {
            return operators.Contains(chr);
        }

        public static bool IsOperator(string s)
        {
            if (s.Length != 1) return false;
            return IsOperator(s[0]);
        }

        public static bool IsNumeral(char chr)
        {
            return chr >= '0' && chr <= '9' || chr == '.';
        }

        /// <summary>
        /// Converts expression string into queue collection type of operands and operators
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>expression string splitted into tokens</returns>
        public static Queue<string> GetTokenQueue(string expression)
        {
            if (expression.Length == 0) return new Queue<string>();

            var queue = new Queue<string>();

            string token = "";

            for(int i=0; i<expression.Length; i++)
            {
                char c = expression[i];
                if (IsOperator(c))
                {
                    if (token.Length > 0)
                    {
                        queue.Enqueue(token);
                        token = "";
                    }
                    queue.Enqueue(c.ToString());
                    continue;
                }
                token += c;
            }
            if (token.Length > 0)
            {
                queue.Enqueue(token);
            }

            return queue;
        }

        public static ExpressionTree InitializeTree()
        {
            var tree = new ExpressionTree();
            // root node is always fictional 0+expr
            tree.Root = new BTreeItem(0, BTreeItemType.Add, null, null, null);
            tree.Root.Left = new BTreeItem(0, BTreeItemType.Leaf, null, null, tree.Root);
            // current node is root node
            tree.Current = tree.Root;
            return tree;
        }

        /// <summary>
        /// Costruct binary from expression in the form of tokens queue
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static ExpressionTree BuildExpressionTree(Queue<string> tokens)
        {
            ExpressionTree tree = InitializeTree();

            string token;
            while(tokens.TryDequeue(out token))
            {
                if (IsOperator(token))
                {
                    var opType = BTreeItem.GetOperatorType(token[0]);
                    tree.AddOperator(opType);
                }
                else
                {
                    double value;
                    if(!double.TryParse(token, NumberStyles.Float, culture, out value))
                    {
                        throw new ExpresionException("Expression error");
                    }
                    else
                    {
                        tree.AddOperandValue(value);
                    }
                }
                Debug.WriteLine($"Token: {token}, Tree: {tree}");
            }

            return tree;
        }


        /// <summary>
        /// Entry method for evaluating string expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static double Evaluate(string expression)
        {
            var queue = Evaluator.GetTokenQueue(expression);
            var tree = Evaluator.BuildExpressionTree(queue);
            return tree.Evaluate();
        }
        
    }
}
