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
    public class Product : BaseSoftIntDelete
    {
        [Required, MaxLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal BasePrice { get; set; }
        public ICollection<Variation>? Variations { get; set; } = new List<Variation>();
        public ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
    }
}
