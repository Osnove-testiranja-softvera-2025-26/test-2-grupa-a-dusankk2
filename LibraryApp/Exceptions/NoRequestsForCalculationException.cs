using System;


namespace LibraryApp.Exceptions
{
    public class NoRequestsForCalculationException: Exception
    {
        public NoRequestsForCalculationException()
        {

        }

        public NoRequestsForCalculationException(string message): base(message)
        {

        }
    }
}
