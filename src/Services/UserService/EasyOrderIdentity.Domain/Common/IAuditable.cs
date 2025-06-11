using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderIdentity.Domain.Common
{
    public interface IAuditable
    {
        DateTime CreatedOn { get; set; }
        string? CreatedBy { get; set; }
        DateTime? ModifiedOn { get; set; }
        string? ModifiedBy { get; set; }
    }
}
