using Markov;
using NUnit.Framework;
using System.Collections;

namespace MarkovBytes.UnitTests
{
    public class RecorderUnitTests
    {


        [Test]
        [TestCaseSource(nameof(InitValues))]
        public void InitTest(uint noOfStates)
        {
            var actual = MatrixRecording.Generate(noOfStates);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Rows, Is.Not.Null);
            Assert.That(actual.Rows.Length, Is.EqualTo(noOfStates));

            for (var i = 0; i < noOfStates; i += 1)
            {
                Assert.That(actual.Rows[i].Length, Is.EqualTo(noOfStates));
            }

            Assert.That(actual.RowDenominators, Is.Not.Null);
            Assert.That(actual.RowDenominators.Length, Is.EqualTo(noOfStates));
        }

        public static IEnumerable InitValues
        {
            get
            {
                // EVEN ALL
                yield return new TestCaseData(0U);
                yield return new TestCaseData(3U);
                yield return new TestCaseData(5U);
                yield return new TestCaseData(7U);
            }
        }
        
        [Test]
        public void UpdateTest()
        {
            var recording = MatrixRecording.Generate(5);
            var recorder = new MatrixRecorder(recording, 0);

            Assert.That(recorder.Recording, Is.EqualTo(recording));
            Assert.That(recorder.CurrentState, Is.EqualTo(0));

            recorder.Update(1);
            Assert.That(recorder.CurrentState, Is.EqualTo(1));
            Assert.That(recording.RowDenominators[0], Is.EqualTo(1));
            Assert.That(recording.Rows[0][1], Is.EqualTo(1));

            recorder.Update(1);
            Assert.That(recorder.CurrentState, Is.EqualTo(1));
            Assert.That(recording.RowDenominators[1], Is.EqualTo(1));
            Assert.That(recording.Rows[1][1], Is.EqualTo(1));

            recorder.Update(1);
            Assert.That(recorder.CurrentState, Is.EqualTo(1));
            Assert.That(recording.RowDenominators[1], Is.EqualTo(2));
            Assert.That(recording.Rows[1][1], Is.EqualTo(2));

            recorder.Update(0);
            Assert.That(recorder.CurrentState, Is.EqualTo(0));
            Assert.That(recording.RowDenominators[1], Is.EqualTo(3));
            Assert.That(recording.Rows[1][0], Is.EqualTo(1));
        }
    }
}
