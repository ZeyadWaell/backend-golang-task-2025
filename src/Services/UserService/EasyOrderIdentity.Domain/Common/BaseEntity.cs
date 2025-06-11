using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderIdentity.Domain.Common
{
    public abstract class BaseEntity : IAuditable
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        [MaxLength(36)]
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        [MaxLength(36)]
        public string? ModifiedBy { get; set; }
    }
}
