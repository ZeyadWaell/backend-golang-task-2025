using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderIdentity.Domain.Common
{
    public abstract class BaseEntity : Base
    {
        public Guid Id { get; set; }
    }

    public abstract class BaseIntEntity : Base
    {
        public int Id { get; set; }
    }
    public abstract class BaseSoftDelete : Base
    {
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }

        [MaxLength(36)] public string? DeletedBy { get; set; }
    }
    public abstract class BaseSoftIntDelete : Base
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        [MaxLength(36)] public string? DeletedBy { get; set; }
    }

    public abstract class Base
    {
        public DateTime CreatedOn { get; set; }

        [MaxLength(36)]
        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [MaxLength(36)]
        public string? ModifiedBy { get; set; }
    }
}
