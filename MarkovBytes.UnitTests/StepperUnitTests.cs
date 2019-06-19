using Markov;
using Moq;
using NUnit.Framework;

namespace MarkovBytes.UnitTests
{
    public class StepperUnitTests
    {
        [Test]
        public void Iterate_0()
        {
            var evaluator = new Mock<INodeEvaluator>();
            var analyser = new Mock<IBitAnalyser>();
            var stepper = new Stepper(evaluator.Object, analyser.Object);

            var rowTree = new RowTree {
                Branches = new ushort[] { },
                Instructions = new Instruction[] { },
                Leaves = new int[] { },
                TestChunks = new TestChunk[] { },
            };

            ushort value = 100;

            var actual = stepper.Iterate(rowTree, value);
            Assert.AreEqual(-1, actual);
            evaluator.Verify(ev => ev.Evaluate(It.IsAny<TestChunk>(), It.IsAny<ushort[]>(), value), Times.Never());
            analyser.Verify(a => a.GetRightmostBit(It.IsAny<uint>()), Times.Never());

            evaluator.VerifyNoOtherCalls();
            analyser.VerifyNoOtherCalls();
        }

        [Test]
        public void Iterate_1()
        {
            var evaluator = new Mock<INodeEvaluator>();
            var analyser = new Mock<IBitAnalyser>();
            var stepper = new Stepper(evaluator.Object, analyser.Object);

            ushort[] BRANCH_KEYS = new ushort[] {
                    // first branch key
                    1, 1, 1, 1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1, 1, 1, 1,
                };
            const int LOCAL_MASK = 0x1;
            var rowTree = new RowTree
            {
                Branches = BRANCH_KEYS,
                Instructions = new Instruction[] {
                    new Instruction
                    {
                        Chunk = 0,
                        Mask = LOCAL_MASK,
                    },
                },
                Leaves = new int[] {
                    0,
                },
                TestChunks = new TestChunk[] {
                    new TestChunk
                    {
                        KeyOffset = 0,
                        LeafOffset = 0,
                        LeafLength = 16,
                    }
                },
            };

            ushort value = 100;

            evaluator.Setup(ev => ev.Evaluate(It.IsAny<TestChunk>(), It.IsAny<ushort[]>(), value))
                .Returns(0xff);

            analyser.Setup(a => a.GetRightmostBit(It.IsAny<uint>()))
                .Returns(0);

            var actual = stepper.Iterate(rowTree, value);
            Assert.AreEqual(0, actual);
            evaluator.Verify(ev => ev.Evaluate(It.IsAny<TestChunk>(), BRANCH_KEYS, value), Times.Once());
            analyser.Verify(a => a.GetRightmostBit(LOCAL_MASK), Times.Once());

            evaluator.VerifyNoOtherCalls();
            analyser.VerifyNoOtherCalls();
        }

        [Test]
        public void Iterate_2()
        {
            var evaluator = new Mock<INodeEvaluator>();
            var analyser = new Mock<IBitAnalyser>();
            var stepper = new Stepper(evaluator.Object, analyser.Object);

            ushort[] BRANCH_KEYS = new ushort[] {
                    // first branch key
                    1, 1, 1, 1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1, 1, 1, 1,
                };
            const int FIRST_MASK = 0x1;
            const int SECOND_MASK = 0xfe;
            var rowTree = new RowTree
            {
                Branches = BRANCH_KEYS,
                Instructions = new Instruction[] {
                    new Instruction
                    {
                        Chunk = 0,
                        Mask = FIRST_MASK,
                    },
                    new Instruction
                    {
                        Chunk = 0,
                        Mask = SECOND_MASK,
                    },
                },
                Leaves = new int[] {
                    0,
                },
                TestChunks = new TestChunk[] {
                    new TestChunk
                    {
                        KeyOffset = 0,
                        LeafOffset = 0,
                        LeafLength = 16,
                    }
                },
            };

            ushort value = 100;

            evaluator.Setup(ev => ev.Evaluate(It.IsAny<TestChunk>(), It.IsAny<ushort[]>(), value))
                .Returns(0xff);

            analyser.Setup(a => a.GetRightmostBit(It.IsAny<uint>()))
                .Returns(0);

            var actual = stepper.Iterate(rowTree, value);
            Assert.AreEqual(0, actual);
            evaluator.Verify(ev => ev.Evaluate(It.IsAny<TestChunk>(), BRANCH_KEYS, value), Times.Once());
            analyser.Verify(a => a.GetRightmostBit(FIRST_MASK), Times.Once());
            analyser.Verify(a => a.GetRightmostBit(SECOND_MASK), Times.Once());

            evaluator.VerifyNoOtherCalls();
            analyser.VerifyNoOtherCalls();
        }

    }
}
