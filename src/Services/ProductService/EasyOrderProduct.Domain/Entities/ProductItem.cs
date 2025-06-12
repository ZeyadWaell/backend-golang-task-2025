using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyOrderProduct.Domain.Common;

namespace EasyOrderProduct.Domain.Entities
{
    public class ProductItem : BaseSoftIntDelete
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Sku { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? PriceOverride { get; set; }
        public ICollection<ProductItemOption> Options { get; set; } = new List<ProductItemOption>();
        public Inventory Inventory { get; set; }
        [NotMapped]
        public IEnumerable<(string VariationName, string OptionValue)> Variations =>
            Options.Select(pio => (pio.VariationOption.Variation.Name, pio.VariationOption.Value));
    }
}
