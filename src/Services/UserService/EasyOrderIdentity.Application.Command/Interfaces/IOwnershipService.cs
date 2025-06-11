using EasyOrderIdentity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderIdentity.Application.Interfaces
{
    public interface IOwnershipService
    {
        Task EnsureOwnerAsync<TEntity>(Guid id) where TEntity : BaseEntity;
    }
}
