using LibraryApp.Models;
using System;


namespace LibraryApp.Services
{
    public interface IDeliveryService
    {
        DeliveryType GetDeliveryTypeForBook(Guid bookId);
    }
}
