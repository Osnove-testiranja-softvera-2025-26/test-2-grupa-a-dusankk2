using LibraryApp.Models;
using LibraryApp.Services;
using System;

public class FakeDeliveryService : IDeliveryService
{
    public DeliveryType DeliveryType { get; set; }

    public DeliveryType GetDeliveryTypeForBook(Guid bookId)
    {
        return DeliveryType;
    }
}