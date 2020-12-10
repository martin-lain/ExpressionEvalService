using ExpressionEvalService.BL;
using NUnit.Framework;

namespace ExpressionEvalService.Tests
{
    public class EvaluatorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IsSeparatorTest()
        {
            Assert.IsTrue(Evaluator.IsOperator('+'));
            Assert.IsTrue(Evaluator.IsOperator('-'));
            Assert.IsTrue(Evaluator.IsOperator('*'));
            Assert.IsTrue(Evaluator.IsOperator('/'));
            Assert.IsTrue(Evaluator.IsOperator('%'));
            Assert.IsTrue(Evaluator.IsOperator('^'));
            Assert.IsTrue(Evaluator.IsOperator('('));
            Assert.IsTrue(Evaluator.IsOperator(')'));
            Assert.IsFalse(Evaluator.IsOperator('1'));
            Assert.IsFalse(Evaluator.IsOperator('.'));
        }

        [Test]
        public void IsNumberTest()
        {
            Assert.IsTrue(Evaluator.IsNumeral('1'));
            Assert.IsTrue(Evaluator.IsNumeral('0'));
            Assert.IsTrue(Evaluator.IsNumeral('.'));
            Assert.IsFalse(Evaluator.IsNumeral('+'));
            Assert.IsFalse(Evaluator.IsNumeral('X'));
        }

        [Test]
        public void TestGetTokenQueueEmpty()
        {            
            var queue = Evaluator.GetTokenQueue("");
            Assert.IsNotNull(queue);
            Assert.IsEmpty(queue);
        }

        [Test]
        public void TestGetTokenQueueValue()
        {
            var queue = Evaluator.GetTokenQueue("1");
            Assert.IsNotNull(queue);
            Assert.AreEqual(1, queue.Count);
            Assert.AreEqual("1", queue.Dequeue());
        }

        [Test]
        public void TestGetTokenQueueValues()
        {
            var queue = Evaluator.GetTokenQueue("1+1.1-1*1.1/1%(1.1^1)");
            Assert.IsNotNull(queue);
            Assert.AreEqual(15, queue.Count);
            Assert.AreEqual("1", queue.Dequeue());
            Assert.AreEqual("+", queue.Dequeue());
            Assert.AreEqual("1.1", queue.Dequeue());
            Assert.AreEqual("-", queue.Dequeue());
            Assert.AreEqual("1", queue.Dequeue());
            Assert.AreEqual("*", queue.Dequeue());
            Assert.AreEqual("1.1", queue.Dequeue());
            Assert.AreEqual("/", queue.Dequeue());
            Assert.AreEqual("1", queue.Dequeue());
            Assert.AreEqual("%", queue.Dequeue());
            Assert.AreEqual("(", queue.Dequeue());
            Assert.AreEqual("1.1", queue.Dequeue());
            Assert.AreEqual("^", queue.Dequeue());
            Assert.AreEqual("1", queue.Dequeue());
            Assert.AreEqual(")", queue.Dequeue());
        }

        [Test]
        public void TestGetOperatorType()
        {
            Assert.AreEqual(BTreeItemType.Add, BTreeItem.GetOperatorType('+'));
            Assert.AreEqual(BTreeItemType.Sub, BTreeItem.GetOperatorType('-'));
            Assert.AreEqual(BTreeItemType.Div, BTreeItem.GetOperatorType('/'));
            Assert.AreEqual(BTreeItemType.Eval, BTreeItem.GetOperatorType('('));
            Assert.AreEqual(BTreeItemType.EvalEnd, BTreeItem.GetOperatorType(')'));
            Assert.AreEqual(BTreeItemType.Mod, BTreeItem.GetOperatorType('%'));
            Assert.AreEqual(BTreeItemType.Mul, BTreeItem.GetOperatorType('*'));
            Assert.AreEqual(BTreeItemType.Power, BTreeItem.GetOperatorType('^'));
        }

        [Test]
        public void TestBuildExpressionTree1()
        {
            var queue = Evaluator.GetTokenQueue("1+1");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.AreEqual(2, tree.Evaluate());
        }

        [Test]
        public void TestBuildExpressionTree2()
        {
            var queue = Evaluator.GetTokenQueue("1+2*3");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.AreEqual(7, tree.Evaluate());
        }

        [Test]
        public void TestBuildExpressionTree3()
        {
            var queue = Evaluator.GetTokenQueue("1*2*3*4+5");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.AreEqual(1 * 2 * 3 * 4 + 5, tree.Evaluate());
        }

        [Test]
        public void TestBuildExpressionTree4()
        {
            var queue = Evaluator.GetTokenQueue("(8*3)+3");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.AreEqual((8d * 3d)+3 , tree.Evaluate());
        }

        [Test]
        public void TestBuildExpressionTree5()
        {
            var queue = Evaluator.GetTokenQueue("(8*3+3)/(3*3+24)");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.AreEqual((8d * 3d + 3d) / (3d * 3d + 24d), tree.Evaluate());
        }
        [Test]
        public void TestBuildExpressionTree6()
        {
            var queue = Evaluator.GetTokenQueue("1+2*3+4*2/(1+3)+1");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.AreEqual(1d + 2d * 3d + 4d * 2d / (1d + 3d) + 1d, tree.Evaluate());
        }
        [Test]
        public void TestBuildExpressionTree7()
        {
            var queue = Evaluator.GetTokenQueue("1+2*3+4*2/(1+3*(3+2))+1");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.AreEqual(1d + 2d * 3d + 4d * 2d / (1d + 3d * (3d + 2d)) + 1d, tree.Evaluate());
        }
        [Test]
        public void TestBuildExpressionTree8()
        {
            var queue = Evaluator.GetTokenQueue("1+2*3+4*2/(1+3*(3+2/(1+2)))+1");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.AreEqual(1d + 2d * 3d + 4d * 2d / (1d + 3d * (3d + 2d / (1d + 2d))) + 1d, tree.Evaluate());
        }
        [Test]
        public void TestBuildExpressionTree9()
        {
            var queue = Evaluator.GetTokenQueue("1%3.2");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.AreEqual(1%3.2d, tree.Evaluate());
        }
        [Test]
        public void TestBuildExpressionTree10()
        {
            var queue = Evaluator.GetTokenQueue("-1");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.AreEqual(-1d, tree.Evaluate());
        }
        [Test]
        public void TestBuildExpressionTree11()
        {
            var queue = Evaluator.GetTokenQueue("1*-1");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.AreEqual(1d * -1d, tree.Evaluate());
        }
        [Test]
        public void TestBuildExpressionTree12()
        {
            var queue = Evaluator.GetTokenQueue("1*+1");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.AreEqual(1d * +1d, tree.Evaluate());
        }

        [Test]
        public void TestBuildExpressionTreeError1()
        {
            var queue = Evaluator.GetTokenQueue("1+");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.Throws<ExpresionException>(() => tree.Evaluate());
        }
        [Test]
        public void TestBuildExpressionTreeError2()
        {
            var queue = Evaluator.GetTokenQueue("(2*3");
            var tree = Evaluator.BuildExpressionTree(queue);
            Assert.Throws<ExpresionException>(() => tree.Evaluate());
        }
        [Test]
        public void TestBuildExpressionTreeError3()
        {
            var queue = Evaluator.GetTokenQueue("2*+");
            Assert.Throws<ExpresionException>(() =>
            {
                var tree = Evaluator.BuildExpressionTree(queue);
                tree.Evaluate();
            });
        }
        [Test]
        public void TestBuildExpressionTreeError4()
        {
            var queue = Evaluator.GetTokenQueue("");
            Assert.Throws<ExpresionException>(() =>
            {
                var tree = Evaluator.BuildExpressionTree(queue);
                tree.Evaluate();
            });
        }
        [Test]
        public void TestBuildExpressionTreeError5()
        {
            var queue = Evaluator.GetTokenQueue("1+2)");
            Assert.Throws<ExpresionException>(() =>
            {
                var tree = Evaluator.BuildExpressionTree(queue);
                tree.Evaluate();
            });
        }
    }
}