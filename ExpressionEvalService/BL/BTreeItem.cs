using System;

namespace ExpressionEvalService.BL
{
    // known operator types - sorted by precedence, highest priority is at the bottom
    public enum BTreeItemType
    {
        // just decimal value no children
        Leaf,
        // sum of two children nodes
        Add,
        // substraction of 2 children nodes
        Sub,
        // multiplication of 2 children nodes
        Mul,
        // division of 2 children nodes
        Div,
        // Modulo of 2 children nodes
        Mod,
        // power of 2 children nodes
        Power,
        // just copy right child - used for parenthesis nesting
        Eval,
        EvalEnd,
    };

    /// <summary>
    /// Binary tree used for storing binary operators and arguments in left and right nodes
    /// </summary>
    public class BTreeItem
    {
        public BTreeItemType Type { get; set; }
        public double Value { get; set; }
        public BTreeItem Parent { get; set; }
        public BTreeItem Left { get; set; }
        public BTreeItem Right { get; set; }


        public BTreeItem()
        {
            Value = 0;
            Type = BTreeItemType.Leaf;
            Left = null;
            Right = null;
            Parent = null;
        }

        public BTreeItem(double value, BTreeItemType type, BTreeItem left, BTreeItem right, BTreeItem parent)
        {
            Value = value;
            Type = type;
            Left = left;
            Right = right;
            Parent = parent;
        }

        // Calculates "weight" of node. If left nor right is present = 0, if left = 1, if right = 2, if both = 3
        public int Weight => (Left is null ? 0 : 1) + (Right is null ? 0 : 2);

        private bool IsLeaf => Type == BTreeItemType.Leaf;

        public override string ToString()
        {
            switch (Type)
            {
                case BTreeItemType.Leaf:
                    return Value.ToString();
                case BTreeItemType.Add:
                    return (Left?.ToString() ?? "") + "+" + (Right?.ToString() ?? "");
                case BTreeItemType.Sub:
                    return (Left?.ToString() ?? "") + "-" + (Right?.ToString() ?? "");
                case BTreeItemType.Mul:
                    return (Left?.ToString() ?? "") + "*" + (Right?.ToString() ?? "");
                case BTreeItemType.Div:
                    return (Left?.ToString() ?? "") + "/" + (Right?.ToString() ?? "");
                case BTreeItemType.Mod:
                    return (Left?.ToString() ?? "") + "%" + (Right?.ToString() ?? "");
                case BTreeItemType.Power:
                    return (Left?.ToString() ?? "") + "^" + (Right?.ToString() ?? "");
                case BTreeItemType.Eval:
                    return "(" + (Right?.ToString() ?? ""); 
                case BTreeItemType.EvalEnd:
                    return "(" + (Right?.ToString() ?? "") + ")";
                default:
                    return "";
            }
        }

        public double Evaluate()
        {
            if (Type == BTreeItemType.Leaf)
                return Value;

            if (Left == null || Right == null) 
                throw new ExpresionException("Expression error");
            double val1 = Left.Evaluate();
            double val2 = Right.Evaluate();
            switch (Type)
            {
                case BTreeItemType.Leaf:
                    return Value;
                case BTreeItemType.Add:
                    return val1 + val2;
                case BTreeItemType.Sub:
                    return val1 - val2;
                case BTreeItemType.Mul:
                    return val1 * val2;
                case BTreeItemType.Div:
                    if (val2 == 0) throw new ExpresionException("Division by zero");
                    return val1 / val2;
                case BTreeItemType.Mod:
                    return (int)val1 % (int)val2;
                case BTreeItemType.Power:
                    return Math.Pow(val1, val2);
                case BTreeItemType.Eval:
                    throw new ExpresionException("Expression error. Parenthesis not closed.");
                case BTreeItemType.EvalEnd:
                    return val2;
                default:
                    return 0;
            }
        }

        public static BTreeItemType GetOperatorType(char chr)
        {
            switch (chr)
            {
                case '+': return BTreeItemType.Add;
                case '-': return BTreeItemType.Sub;
                case '*': return BTreeItemType.Mul;
                case '/': return BTreeItemType.Div;
                case '%': return BTreeItemType.Mod;
                case '^': return BTreeItemType.Power;
                case '(': return BTreeItemType.Eval;
                case ')': return BTreeItemType.EvalEnd;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
    
}
