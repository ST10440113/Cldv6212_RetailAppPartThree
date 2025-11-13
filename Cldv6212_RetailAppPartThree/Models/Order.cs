using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cldv6212_RetailAppPartThree.Models
{
    public class Order : ITableEntity
    {
        [Key] public int OrderId { get; set; }

        [Display(Name = "Order Date")]

        [Timestamp]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }


        public string Status { get; set; }


        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }


        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }



        //ITableEntity stuff
        public string PartitionKey { get; set; } = "ORDER";
        public string RowKey { get; set; } = Guid.NewGuid().ToString("N");

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

    }
}

