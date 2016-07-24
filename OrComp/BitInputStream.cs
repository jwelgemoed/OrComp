using System;
using System.IO;

namespace OrComp
{
    public class BitInputStream : IDisposable
    {
        private Stream _input;
        private int _buffer;
        private int _nextBit = 8;
        private object _lock = new object();

        public BitInputStream(Stream input)
        {
            _input = input;
        }

        public int ReadBit()
        {
            lock(_lock)
            {
                if (_nextBit == 8)
                {
                    _buffer = _input.ReadByte();

                    if (_buffer == -1)
                        throw new Exception("End of stream!");

                    _nextBit = 0;
                }

                int bit = _buffer & (1 << (int) _nextBit);
                _nextBit++;
                bit = bit == 0 ? 0 : 1;

                return bit;
            }
        }


        #region IDisposable Support

        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _input.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
