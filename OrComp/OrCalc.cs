using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrComp
{
    public class OrCalc
    {
        private int[] _Sp;
        private int[] _lrs;
        private int[] _letterLengths;

        private byte[] _buffer;
        private int _maxRepeat;

        private OracleEncoder _encoder;

        public long MaxRepeat
        {
            get
            {
                return _maxRepeat;
            }
        }

        public OrCalc()
        {
            _encoder = new OracleEncoder();
            _letterLengths = _encoder.CreateLetterLengthsSequence();
        }

        public int LengthCommonSuffix(int pi1, int pi2)
        {
            if (pi2 == _Sp[pi1])
            {
                return _lrs[pi1];
            }
            else
            {
                while (_Sp[pi2] != _Sp[pi1])
                {
                    pi2 = _Sp[pi2];
                }

                if (_lrs[pi1] < _lrs[pi2])
                    return _lrs[pi1];
                else
                    return _lrs[pi2];
            }
        }

        public int LengthRepeatedSuffix(int pi1, int s)
        {
            if (s == 0)
            {
                return 0;
            }
            else
            {
                return (LengthCommonSuffix(pi1, s - 1) + 1);
            }
        }

        public void PairEncoder(BitOutputStream output, int length, int position)
        {
            if (length > _maxRepeat)
                _maxRepeat = length;

            if (length > 0)
            {
                long codelength = _encoder.Encode2ndOrderCodeWord(null, length) +
                                _encoder.Encode3rdOrderCodeWord(null, position);
                long code2 = 0;
                long i;
                for (i = position; i < position + length; i++)
                {
                    if (_buffer[i - 1] < 0)
                        code2 = code2 + _letterLengths[_buffer[i - 1] + 256];
                    else
                        code2 = code2 + _letterLengths[_buffer[i - 1]];
                }

                if (codelength < code2)
                {
                    _encoder.Encode2ndOrderCodeWord(output, length);
                    _encoder.Encode3rdOrderCodeWord(output, position);

                    String s = "(" + length + ")(" + position + ")";
                    Console.WriteLine(s);
                }
                else
                {
                    for (i = position; i < position + length; i++)
                    {
                        _encoder.Encode2ndOrderCodeWord(output, 0);
                        if (_buffer[i - 1] >= 0)
                            _encoder.Encode3rdOrderCodeWord(output, _buffer[i - 1]);
                        else
                            _encoder.Encode3rdOrderCodeWord(output, _buffer[i - 1] + 256);

                    }

                }
            }
            else
            {
                _encoder.Encode2ndOrderCodeWord(output, length);
                _encoder.Encode3rdOrderCodeWord(output, position);
                String s = "(" + length + ")(" + position + ")";
                Console.WriteLine(s);
            }
        }

        public int GetBetterPosition(int i, int lastPosition)
        {
            int k = i;
            bool done = false;

            while (done == false)
            {
                if (_lrs[k] >= (i - lastPosition) & (_lrs[_Sp[k]] < (i - lastPosition)))
                    done = true;
                else
                    k = _Sp[k];
            }

            return k;
        }

        public void Compress(Stream input, Stream output, int windowSize)
        {
            Oracle oracle;
            int size;
            int bsize = (int) input.Length;
            int fileLeft = 0;
            int a = -1;
            int lastPosition = 0;
            char s;

            BitArrayWrapper Bits = new BitArrayWrapper();

            float meter = 0;
            float oldm = 0;
            long bcounter = 0;

            using (BitOutputStream bitOutput = new BitOutputStream(output))
            {
                //First output the original file size, and the buffer size.
                _encoder.Encode3rdOrderCodeWord(bitOutput, bsize);
                _encoder.Encode3rdOrderCodeWord(bitOutput, windowSize);
                oracle = new Oracle();
                while (input.Position < input.Length)
                {
                    oracle.AddState(0);
                    size = (int) input.Length - (int) input.Position;

                    if (size <= windowSize)
                    {
                        _Sp = new int[size + 2];
                        _lrs = new int[size + 2];
                        _buffer = new byte[size + 1];
                        input.Read(_buffer, 0, (int)size);
                        fileLeft = size;
                    }
                    else
                    {
                        _Sp = new int[windowSize + 2];
                        _lrs = new int[windowSize + 2];
                        fileLeft = windowSize;
                        _buffer = new byte[windowSize + 1];
                        input.Read(_buffer, 0, (int)windowSize);
                    }

                    _Sp[0] = -1;
                    int k = 0;
                    int j;
                    int s1;
                    int pi1;
                    int endto;
                    lastPosition = 0;

                    if (fileLeft == size)
                        endto = fileLeft + 1;
                    else
                        endto = fileLeft;

                    for (int i = 1; i <= endto; i++)
                    {
                        bcounter++;

                        oracle.AddState(i);

                        s = (char)_buffer[i - 1];

                        Console.WriteLine(_buffer[i - 1]);

                        j = _Sp[i - 1];
                        pi1 = i - 1;
                        while ((j > -1) & (oracle.FindTransition(oracle.GetState(j), _buffer, s) == -2))
                        {
                            //Just keep track of the external transitions
                            oracle.AddTransition(j, i);
                            pi1 = j;
                            j = _Sp[j];
                        }
                        if (j == -1)
                            s1 = 0;
                        else
                            s1 = oracle.FindTransition(oracle.GetState(j), _buffer, s);
                        _Sp[i] = s1;
                        _lrs[i] = LengthRepeatedSuffix(pi1, _Sp[i]);

                        if ((i == fileLeft) & (lastPosition <= i - 1))
                        {
                            long z = 0;
                            for (z = (lastPosition); z < fileLeft; z++)
                            {
                                if (_buffer[z] >= 0)
                                    PairEncoder(bitOutput, 0, _buffer[z]);
                                else
                                    PairEncoder(bitOutput, 0, _buffer[z] + 256);
                            }
                            lastPosition = i - 1;
                        }
                        else if ((_lrs[i] < (i - lastPosition)))
                        {
                            if (_lrs[lastPosition + 1] == 0)
                            {
                                if (_buffer[lastPosition] >= 0)
                                    PairEncoder(bitOutput, 0, _buffer[lastPosition]);
                                else
                                    PairEncoder(bitOutput, 0, _buffer[lastPosition] + 256);

                                lastPosition = lastPosition + 1;
                            }
                            else
                            {
                                if (_lrs[i - 1] > (i - 1) - lastPosition)
                                {
                                    k = GetBetterPosition(i - 1, lastPosition);
                                    PairEncoder(bitOutput, (i - 1) - lastPosition, _Sp[k] - (i - 1) + lastPosition + 1);
                                    lastPosition = i - 1;
                                }
                                else
                                {
                                    PairEncoder(bitOutput, (i - 1) - lastPosition, _Sp[i - 1] - (i - 1) + lastPosition + 1);
                                    lastPosition = i - 1;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
