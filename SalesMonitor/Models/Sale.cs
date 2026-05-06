using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesMonitor.Models
{
    [Table("Продажи")]
    public class Sale
    {
        [Key]
        [Column("ID продажи")]
        public int Id { get; set; }

        [Column("ID продукта")]
        public int ProductId { get; set; }

        [Column("Дата продажи", TypeName = "date")]
        public DateTime SaleDate { get; set; }

        [Column("Количество")]
        public int Quantity { get; set; }

        [Column("Общая сумма чека", TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column("Маржа", TypeName = "decimal(18,2)")]
        public decimal Margin { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;
    }
}