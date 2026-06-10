using LibraryApp.Models;
using LibraryApp.Services;

namespace LibraryApp.Test
{
    public class FakePurchaseService : IPurchaseService
    {
        public Purchase CreatedPurchase { get; set; }

        public void CreatePurchase(Purchase purchase)
        {
            CreatedPurchase = purchase;
        }
    }
}