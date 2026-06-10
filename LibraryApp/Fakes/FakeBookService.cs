using LibraryApp.Models;
using LibraryApp.Services;
using System;

public class FakeBookService : IBookService
{
    public BookRequestInfo BookRequestInfo { get; set; }

    public BookRequestInfo GetBookRequestsInTheLastMonthInfo(Guid bookId)
    {
        return BookRequestInfo;
    }
}