using LibraryApp.Models;
using System;

namespace LibraryApp.Services
{
    public interface IBookService
    {
        BookRequestInfo GetBookRequestsInTheLastMonthInfo(Guid bookId);
    }
}
