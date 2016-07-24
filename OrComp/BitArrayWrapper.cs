using System.Collections;

namespace OrComp
{
    public class BitArrayWrapper : IEnumerable
    {
        private BitArray _bitArray;

        public BitArrayWrapper()
        {
            _bitArray = new BitArray(1);
        }

        public BitArrayWrapper(int size)
        {
            _bitArray = new BitArray(size);
        }

        public void Set(int index)
        {
            Set(index, true);
        }

        public void Set(int index, bool value)
        {
            if (_bitArray.Length < index + 1)
                _bitArray.Length = index + 1;

            _bitArray.Set(index, value);
        }

        public bool Get(int index)
        {
            return _bitArray.Get(index);
        }

        public int Length()
        {
            return _bitArray.Length;
        }

        public IEnumerator GetEnumerator()
        {
            return _bitArray.GetEnumerator();
        }
    }
}
