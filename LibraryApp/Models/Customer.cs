using System;


namespace LibraryApp.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public DateTime MembershipOpenedDate { get; set; }
    }
}
