


using System;

namespace LibraryApp.Models
{
    public class Purchase
    {
        public Guid BookId { get; set; }
        public int NumberOfCopiesToBePurchased { get; set; }

        public Purchase(Guid bookId)
        {
            BookId = bookId;
        }

        public Purchase()
        {

        }

    }
}
