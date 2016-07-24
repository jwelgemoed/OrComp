using System;
using System.IO;

namespace OrComp
{
    public class BitOutputStream : IDisposable
    {
        private Stream _output;
        private long _buffer;
        private long _bitCount;
        private object _lock = new object();

        public BitOutputStream(Stream stream)
        {
            _output = stream;
        }

        public void WriteBit(long bit)
        {
            if (bit != 0 && bit != 1)
                throw new ArgumentException("Input is not a bit!");

            lock (_lock)
            {
                _buffer |= ((uint) bit) << (int) _bitCount;
                _bitCount++;

                if (_bitCount == 8)
                {
                    Flush();
                }
            }
        }

        private void Flush()
        {
            if (_bitCount > 0)
            {
                _output.WriteByte((byte)_buffer);
                _bitCount = 0;
                _buffer = 0;
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
                    _output.Dispose();
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
