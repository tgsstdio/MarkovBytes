using NUnit.Framework;
using System.Collections.Generic;

namespace MarkovBytes.UnitTests
{
    public class SlicersUnitTests
    {
        [Test]
        public void EmptyTest()
        {
            var rowData = new ushort[] { };
            var actual = ArrangeSlices(rowData);
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Length);
        }

        private static SliceData[] ArrangeSlices(ushort[] rowData)
        {
            ushort total = 0;
            var slices = new List<SliceData>();
            var count = rowData.Length;
            for (var i = 0; i < count; i += 1)
            {
                var percent = rowData[i];
                if (percent > 0)
                {
                    total += percent;
                    slices.Add(new SliceData { State = i, UpperLimit = total });
                }
            }
           return slices.ToArray();
        }

        private class SliceData
        {
            public int State { get; internal set; }
            public ushort UpperLimit { get; internal set; }
        }
    }
}
