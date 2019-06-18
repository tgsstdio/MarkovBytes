using Markov;
using Moq;
using NUnit.Framework;
using System;
using System.Numerics;

namespace Markov
{

    public class DefaultNodeEvaluator : INodeEvaluator
    {
        public uint Evaluate(TestChunk testChunk, ushort[] branches, ushort singleValue)
        {
            var nodeSpace = new Span<ushort>(branches, testChunk.KeyOffset, testChunk.LeafLength);
            return SetMask16_SIMD_Optimized_1(nodeSpace, singleValue);
        }

        static readonly ushort[] BITSHIFT_16 = new ushort[]
        {
            1,
            2,
            4,
            8,
            16,
            32,
            64,
            128,
            256,
            512,
            1024,
            2048,
            4096,
            8192,
            16384,
            32768,
        };

        private static uint SetMask16_SIMD_Optimized_1(Span<ushort> checks, ushort value)
        {
            var a = new Vector<ushort>(value);
            var b = new Vector<ushort>(checks);

            // INTERNAL TEST 
            var test = Vector.LessThanOrEqual(a, b);
            var right = new Vector<ushort>(BITSHIFT_16);

            var y = Vector.ConditionalSelect(test, right, Vector<ushort>.Zero);

            // Console.WriteLine(test);
            // Console.WriteLine(y);

            Vector.Widen(y, out Vector<uint> f8, out Vector<uint> b8);
            var shuffle = Vector.BitwiseOr(f8, b8);

            // CONSOLIDATE
            Vector.Widen(shuffle, out Vector<ulong> f4, out Vector<ulong> b4);
            var sum = Vector.BitwiseOr(f4, b4);
            return (uint)(sum[0] | sum[1] | sum[2] | sum[3]);
        }

    }
}

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
