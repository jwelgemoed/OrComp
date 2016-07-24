using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrComp
{
    public class OracleEncoder
    {
        private int[] _fibonacciSequenceOrder2;
        private int[] _fibonacciSequenceOrder3;

        public OracleEncoder()
        {
            _fibonacciSequenceOrder2 = Fibonacci.CreateSequenceOrder2();
            _fibonacciSequenceOrder3 = Fibonacci.CreateSequenceOrder3();
        }

        public int Encode2ndOrderCodeWord(BitOutputStream output, int n)
        {
            BitArrayWrapper encoding = new BitArrayWrapper();
            int temp = n;
            int i = Fibonacci.FibonacciSize - 1;

            while (i > -1)
            {
                if (_fibonacciSequenceOrder2[i] < temp)
                {
                    encoding.Set(i);
                    temp = temp - _fibonacciSequenceOrder2[i];
                }
                else if (_fibonacciSequenceOrder2[i] == temp)
                {
                    encoding.Set(i);
                    i = 0;
                }
                i--;
            }

            encoding.Set(encoding.Length());

            if (output != null)
			    WriteEncoding(output, encoding);

            return encoding.Length();
        }

        public int Encode3rdOrderCodeWord(BitOutputStream output, int n)
        {
            BitArrayWrapper encoding = new BitArrayWrapper();
            int temp = n / 2;
            int rem = n % 2;

            int i = Fibonacci.FibonacciSize - 1;

            while (i > -1)
            {
                if (_fibonacciSequenceOrder3[i] < temp)
                {
                    encoding.Set(i);
                    temp = temp - _fibonacciSequenceOrder3[i];
                }
                else if (_fibonacciSequenceOrder3[i] == temp)
                {
                    encoding.Set(i);
                    i = 0;
                }
                i--;
            }

            if (rem == 1)
            {
                encoding.Set(encoding.Length() + 1);
                encoding.Set(encoding.Length() + 1);
            }
            else
                encoding.Set(encoding.Length() + 2);

            encoding.Set(encoding.Length());
            encoding.Set(encoding.Length());

            if (output != null)
			    WriteEncoding(output, encoding);

            return encoding.Length();
        }

        public void WriteEncoding(BitOutputStream output, BitArrayWrapper encoding)
        {
            foreach (bool bit in encoding)
            {
                output.WriteBit(bit ? 1 : 0);
            }
        }

        public int[] CreateLetterLengthsSequence()
        {
            int[] letterLengths = new int[256];

            for (int i=0; i<256; i++)
            {
                letterLengths[i] = Encode2ndOrderCodeWord(null, 0) + Encode3rdOrderCodeWord(null, i);
            }

            return letterLengths;
        }
    }
}
