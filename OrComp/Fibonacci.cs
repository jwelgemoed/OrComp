namespace OrComp
{
    /// <summary>
    /// Creates 1st and 2nd order Fibonacci sequences.
    /// </summary>
    public static class Fibonacci
    {
        private static int _fiboSize = 37;

        public static int FibonacciSize
        {
            get
            {
                return _fiboSize;
            }
            set
            { _fiboSize = value;
            }
        }

        public static int[] CreateSequenceOrder2()
        {
            int[] Fibo = new int[FibonacciSize];
            Fibo[0] = 0;
            Fibo[1] = 1;

            for (int i = 2; i < FibonacciSize; i++)
            {
                Fibo[i] = Fibo[i - 1] + Fibo[i - 2];
            }
            return Fibo;
        }

        public static int[] CreateSequenceOrder3()
        {
            int[] Fibo = new int[FibonacciSize];
            Fibo[0] = 0;
            Fibo[1] = 1;
            Fibo[2] = 2;

            for (int i = 3; i < FibonacciSize; i++)
            {
                Fibo[i] = Fibo[i - 1] + Fibo[i - 2] + Fibo[i - 3];
            }
            return Fibo;
        }

    }
}
