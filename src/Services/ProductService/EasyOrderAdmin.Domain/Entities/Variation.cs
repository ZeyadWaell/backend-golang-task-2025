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
    public class Variation : BaseSoftIntDelete
    {

        [Required, MaxLength(100)]
        public string Name { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public ICollection<VariationOption> Options { get; set; } = new List<VariationOption>();
    }

}
