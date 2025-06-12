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
    public class VariationOption : BaseSoftIntDelete
    {

        [Required, MaxLength(100)]
        public string Value { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal PriceModifier { get; set; } = 0m;
        public int VariationId { get; set; }
        [ForeignKey(nameof(VariationId))]
        public Variation Variation { get; set; }
        public ICollection<ProductItemOption> ProductItemOptions { get; set; } = new List<ProductItemOption>();
    }
}
