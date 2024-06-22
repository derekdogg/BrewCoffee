namespace BrewCoffeeApi
{
     
        public class Response
        {
            public string Message { get; set; }
            public string Prepared { get; set; } // this should be a datetime, but for total simplicity we keep it as a string.
        }
    
}
