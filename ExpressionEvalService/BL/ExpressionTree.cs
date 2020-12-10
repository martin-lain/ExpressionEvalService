namespace ExpressionEvalService.BL
{
    /// <summary>
    /// Class for keeping actual expression tree, evaluating and adding new nodes
    /// </summary>
    public class ExpressionTree
    {
        public BTreeItem Root { get; set; }
        public BTreeItem Current { get; set; }


        /// <summary>
        /// Evaluate and return expression value of the tree
        /// </summary>
        /// <returns></returns>
        public double Evaluate()
        {
            if (Root == null) return 0;
            return Root.Evaluate();
        }

        public override string ToString()
        {
            return Root == null ? "" : Root.ToString();
        }

        /// <summary>
        /// Inserts new operator into the tree, if the right node of current node is empty
        /// </summary>
        /// <param name="opType"></param>
        private void AddOperatorToRight(BTreeItemType opType)
        {
            // only if right node is empty
            if (Current.Weight == 1)
            {
                // this should happen only for situations like +(, *( etc. so priority of inserted operator should always be greater
                if (Current.Type <= opType)
                {
                    // create new right node and make it current
                    Current.Right = new BTreeItem(0, opType, null, null, Current);
                    Current = Current.Right;

                    // if opening parenthesis always make left sub node empty, we are only adding to the right
                    // if + or - operator is actually a sign, prepend virtual 0, eg 3*-2 => 3* (0-2)
                    if (opType == BTreeItemType.Eval || opType == BTreeItemType.Sub || opType == BTreeItemType.Add)
                        Current.Left = new BTreeItem(0, BTreeItemType.Leaf, null, null, Current);
                }
                else
                // if + or - operator is actually a sign, prepend virtual 0, eg 3*-2 => 3* (0-2)
                if (opType == BTreeItemType.Sub || opType == BTreeItemType.Add)
                {
                    Current.Right = new BTreeItem(0, opType, null, null, Current);
                    Current = Current.Right;
                    Current.Left = new BTreeItem(0, BTreeItemType.Leaf, null, null, Current);
                }
                else
                {
                    throw new ExpresionException("Invalid expression");
                }
            }
            else
                throw new ExpresionException("Invalid expression");
        }

        /// <summary>
        /// Add new operator to the tree if right node is present - we have to do substitution
        /// </summary>
        /// <param name="opType"></param>
        private void SubstituteOperatorToRight(BTreeItemType opType)
        {
            // when priority is higher we go down the tree - just move actual content to the left sub-node and replace with new node 
            if (Current.Type <= opType)
            {
                var inserted = new BTreeItem(0, opType, null, null, Current);
                var left = Current.Right;
                inserted.Left = left;
                left.Parent = inserted;
                Current.Right = inserted;
                Current = inserted;
            }
            else
            {
                // move higher in the tree
                // only up to root, only until we find operator with lower priority or we hit opening bracket
                while (Current.Parent != null && Current.Type > opType && Current.Type != BTreeItemType.Eval)
                {
                    Current = Current.Parent;
                }
                // do not go past opening bracket, move one step back
                if (Current.Type == BTreeItemType.Eval)
                {
                    Current = Current.Right;
                }
                // create new parent node and put the whole subtree into left sub-node
                var inserted = new BTreeItem(0, opType, Current, null, Current.Parent);
                // are we rotating root node?
                if (Current.Parent is null)
                {
                    Root = inserted;
                }
                else
                {
                    Current.Parent.Right = inserted;
                }
                Current = inserted;
            }

        }

        /// <summary>
        /// Closes parenthesis in current subtree
        /// </summary>
        private void CloseParenthesis()
        {
            // go up the tree until we find nearest opening bracket
            while (Current.Parent != null && Current.Type != BTreeItemType.Eval)
            {
                Current = Current.Parent;
            }
            // if we have hit the top it means that we didnt find opening bracket
            if (Current.Parent == null && Current.Type != BTreeItemType.Eval)
                throw new ExpresionException("Expression error");

            // change eval to eval end - this is the indication that subtree brackets are closed
            if (Current.Parent == null || Current.Type != BTreeItemType.Eval) return;
            
            Current.Type = BTreeItemType.EvalEnd;
            Current = Current.Parent;
        }

        internal void AddOperandValue(double value)
        {
            // add value to left or right if left is not empty
            switch (Current.Weight)
            {
                case 0:
                    Current.Left = new BTreeItem(value, BTreeItemType.Leaf, null, null, Current); //should not occur
                    break;
                case 1:
                    Current.Right = new BTreeItem(value, BTreeItemType.Leaf, null, null, Current);
                    break;
                default:
                    throw new ExpresionException("Expression error");

            }
        }

        internal void AddOperator(BTreeItemType opType)
        {
            // close open parenthesis
            if (opType == BTreeItemType.EvalEnd)
            {
                CloseParenthesis();
                return;
            }
            switch (Current.Weight)
            {
                // we are not awaiting operator before operand
                case 0: throw new ExpresionException("Expression error");
                case 1:
                    // right sub-node is empty
                    AddOperatorToRight(opType);
                    break;
                case 3:
                    // right sub-node is present - make a rotation
                    SubstituteOperatorToRight(opType);
                    break;
                default: throw new ExpresionException("Expression error");
            }
        }
    }

}
