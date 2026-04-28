using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesMonitor.Models
{
    [Table("Продукты")]
    public class Product
    {
        [Key]
        [Column("ID продукта")]
        public int Id { get; set; }

        [Required]
        [MaxLength(70)]
        [Column("Название продукта")]
        public string Name { get; set; } = string.Empty;

        [Column("Себестоимость", TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }

        [Column("Цена продажи", TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; }

        [Column("В продаже")]
        public bool IsOnSale { get; set; }

        [Column("Дата добавления в ассортимент", TypeName = "date")]
        public DateTime DateAdded { get; set; }

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ClientActivity? ClientActivity { get; set; }
    }
}