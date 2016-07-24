using System.Collections;
using System.IO;

namespace OrComp
{
    public class OracleDecoder
    {
        private int _currentBitsRead;
        private int[] _fibonacciSequenceOrder2;
        private int[] _fibonacciSequenceOrder3;

        public OracleDecoder()
        {
            _fibonacciSequenceOrder2 = Fibonacci.CreateSequenceOrder2();
            _fibonacciSequenceOrder3 = Fibonacci.CreateSequenceOrder3();
        }
                
        public int Decode2ndOrderCodeWord(BitArrayWrapper bits)
        {
            if (bits.Length() == 0)
                return -1;

            int value = 0;

            for (int i = 0; i < bits.Length(); i++)
            {
                if (bits.Get(i))
                {
                    value += _fibonacciSequenceOrder2[i];
                }
            }

            return value;
        }

        public int Decode3rdOrderCodeWord(BitArrayWrapper bits)
        {
            if (bits.Length() == 0)
                return -1;

            int value = 0;


            int rem = 0;

            if (bits.Get(bits.Length() - 5))
                rem = 1;

            for (int i = 0; i < bits.Length(); i++)
            {
                if (bits.Get(i))
                {
                    value += _fibonacciSequenceOrder3[i];
                }
            }

            value = (value * 2) + rem;


            return value;
        }

        public BitArrayWrapper Read2ndOrderCodeWord(BitInputStream input)
        {
            bool done = false;
            int prevbit = 0;
            int curbit = 0;
            int bcounter = -1;
            BitArrayWrapper tempSet = new BitArrayWrapper();

            while (done == false)
            {
                curbit = input.ReadBit();
                bcounter++;

                if ((curbit == 1) & (prevbit == 1))
                    done = true;
                else if ((curbit == 1) & (prevbit == 0))
                {
                    tempSet.Set(bcounter);
                    prevbit = curbit;
                }
                else
                {
                    prevbit = curbit;
                }
            }

            return tempSet;
        }

        public BitArrayWrapper Read3rdOrderCodeWord(BitInputStream input)
        {
            bool done = false;
            int prevbit = 0;
            int prevbit2 = 0;
            int prevbit3 = 0;
            int curbit = 0;
            int bcounter = -1;
            BitArrayWrapper tempSet = new BitArrayWrapper();

            while (done == false)
            {
                curbit = input.ReadBit();

                bcounter++;
                if ((curbit == 1) & (prevbit == 1) & (prevbit2 == 1) & (prevbit3 == 0))
                {
                    tempSet.Set(bcounter);
                    done = true;
                }
                else if (curbit == 1)
                {
                    tempSet.Set(bcounter);
                    prevbit3 = prevbit2;
                    prevbit2 = prevbit;
                    prevbit = curbit;
                }
                else
                {
                    prevbit3 = prevbit2;
                    prevbit2 = prevbit;
                    prevbit = curbit;
                }
            }

            return tempSet;
        }

        public void WriteDecoding(Stream stream, byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }

        public void Decompress(BitInputStream input, Stream output, long bufferSize)
        {
            long pairLength = -1000;
            long pairPosition = -1000;
            long currentBufferPosition = 0;
            long fileSize;
            long adj = 0;

            BitArray tempArray = new BitArray(1);

            fileSize = Decode3rdOrderCodeWord(Read3rdOrderCodeWord(input));
            bufferSize = Decode3rdOrderCodeWord(Read3rdOrderCodeWord(input));

            byte[] buffer = new byte[fileSize];

            while (currentBufferPosition < fileSize)
            {
                if (fileSize != bufferSize)
                {
                    adj = currentBufferPosition;
                }

                pairLength = Decode2ndOrderCodeWord(Read2ndOrderCodeWord(input));
                pairPosition = Decode3rdOrderCodeWord(Read3rdOrderCodeWord(input));

                if ((pairLength > -1) & (pairPosition > -1))
                {
                    if (pairLength == 0)
                    {
                        buffer[currentBufferPosition] = (byte)pairPosition;
                        currentBufferPosition++;
                    }
                    else
                    {
                        for (long i = 0; i < pairLength; i++)
                        {
                            buffer[currentBufferPosition + i] = buffer[adj + pairPosition + i - 1];
                        }
                        currentBufferPosition += pairLength;
                    }
                }
            }

            WriteDecoding(output, buffer);
        }
    }
}
