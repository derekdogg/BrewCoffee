namespace BrewCoffeeApi
{
    //very rudimentary and basic locking object to increment count under low loads (since only one thread can access at a time)
    public static class RequestCounter
    {
        private static int requestCount = 0;
        private static readonly object lockObject = new object();

        public static int Increment()
        {
            lock (lockObject)
            {
                requestCount++;
                return requestCount;
            }
        }

        //add a reset method for testing purposes
        public static void Reset()
        {
            lock (lockObject)
            {
                requestCount = 0;
            }
        }

        public static int GetCount()
        {
            lock (lockObject)
            {
                return requestCount;
            }
        }
    }
}
