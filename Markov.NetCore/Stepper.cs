namespace Markov
{
    public class Stepper
    {
        private readonly INodeEvaluator mEvaluator;
        private readonly IBitAnalyser mAnalyser;

        public Stepper(INodeEvaluator evaluator, IBitAnalyser analyser)
        {
            mEvaluator = evaluator;
            mAnalyser = analyser;
        }

        public int Iterate(RowTree tree, ushort value)
        {
            var noOfInstructions = tree.Instructions.Length;
            int currentCheck = -1;
            uint resultBitField = 0;
            for (var i = 0; i < noOfInstructions; i += 1)
            {
                var currentInstruction = tree.Instructions[i];

                if (currentCheck != currentInstruction.Chunk)
                {
                    currentCheck = currentInstruction.Chunk;
                    // do check
                    resultBitField = mEvaluator.Evaluate(tree.TestChunks[currentCheck], tree.Branches, value);
                }

                var validationMask = resultBitField & currentInstruction.Mask;

                if (validationMask > 0)
                {
                    var branchIndex = mAnalyser.GetRightmostBit(validationMask) + tree.TestChunks[currentCheck].KeyOffset;
                    var arrayIndex = tree.TestChunks[currentCheck].LeafOffset + branchIndex;
                    return tree.Leaves[arrayIndex];
                }
            }

            return -1;
        }
    }
}
