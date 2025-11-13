using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace Cldv6212_RetailAppPartThree.Models
{
    public class Customer : ITableEntity
    {
        [Key] public int CustomerId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress] public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        //ITableEntity stuff
        public string PartitionKey { get; set; } = "CUSTOMER";
        public string RowKey { get; set; } = Guid.NewGuid().ToString("N");

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

    }
}

