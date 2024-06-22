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
    }
}
