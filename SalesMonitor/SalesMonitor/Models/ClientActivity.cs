using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesMonitor.Models
{
    [Table("Активность клиентов")]
    public class ClientActivity
    {
        [Key]
        [Column("ID продукта_")]
        public int Id { get; set; }

        [Column("ID продукта")]
        public int ProductId { get; set; }

        [Column("Дата последней продажи", TypeName = "date")]
        public DateTime? LastSaleDate { get; set; }

        [Column("Средний интервал между заказами", TypeName = "decimal(18,2)")]
        public decimal AverageOrderInterval { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;
    }
}