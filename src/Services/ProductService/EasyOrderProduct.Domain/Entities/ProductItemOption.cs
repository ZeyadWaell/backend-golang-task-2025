using EasyOrderProduct.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Domain.Entities
{
    public class ProductItemOption : BaseSoftIntDelete
    {
        public int ProductItemId { get; set; }
        public ProductItem ProductItem { get; set; }
        public int VariationOptionId { get; set; }
        public VariationOption VariationOption { get; set; }
    }
}
