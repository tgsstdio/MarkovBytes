using NUnit.Framework;
using System;

namespace MarkovBytes.UnitTests
{
    class MatrixMultiplicationUnitTests
    {
        [Test]
        public void Empty()
        {
            var multi = new MatrixMultiplication();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(
              delegate { multi.Extract(0, 0, new ushort[0, 0] { }); });
            //Assert.That(ex.Message, Is.EqualTo("row must less than length.\r\nParameter name: row\r\nActual value was 0."));
            Assert.That(ex.ActualValue, Is.EqualTo(0U));
            Assert.That(ex.ParamName, Is.EqualTo("row"));
            //Assert.That(() =>
            //{
            //    throw new FormatException("This is a format exception.");
            //}, Throws.TypeOf<FormatException>());
        }

        public interface IRandomQuery
        {

        }        

        public int ExtractShort(uint row, uint randValue, uint rowDenominator, ushort[,] srcMatrix)
        {
            uint total = rowDenominator;
            int length = srcMatrix.GetLength(1);
            for (var j = 0; j < length; j += 1)
            {  
                if (srcMatrix[row, j] == 0)
                {
                    total -= srcMatrix[row, j];

                    if (total <= randValue)
                    {
                        return j;
                    }
                }                
            }
            return -1;
        }
    }

    class MatrixMultiplication
    {
        public ushort MaxPercent { get; set; }

        public ushort[] Extract(uint row, uint length, ushort[,] matrix)
        {
            if (row >= length)
                throw new ArgumentOutOfRangeException(nameof(row), row, nameof(row) + " must less than length.");

            var initialState = new ushort[length];
            // ALL ELSE IS ZERO
            initialState[row] = MaxPercent;
                     
            var result = new ushort[length];

            for (var j = 0; j < length; j += 1) {
                int sum = 0;
                for (var i = 0; i < length; i += 1)
                {
                    sum += (matrix[i, j] * initialState[j]);
                }
                result[j] = (ushort) (sum / MaxPercent);
            }

            return result;
        }        
    }
}
