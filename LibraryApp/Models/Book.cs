using System;


namespace LibraryApp.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title {get; set;}
        public string Author { get; set; }

        public double Price { get; set; }
        public int NumberOfCopies { get; set; }
    }
}
