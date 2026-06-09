using System;


namespace LibraryApp.Models
{
    public class BookRequestInfo
    {
        public Guid BookId { get; set; }
        public int NumberOfTotalRequests { get; set; }
        public double PercentOfUnprocessedRequests { get; set; }
        

    }
}
