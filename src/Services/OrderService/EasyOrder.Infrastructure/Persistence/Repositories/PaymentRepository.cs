using EasyOrder.Application.Contracts.Interfaces.Repository;
using EasyOrder.Domain.Entities;
using EasyOrder.Infrastructure.Persistence.Context;
using EasyOrder.Infrastructure.Persistence.Repositories.Main;


namespace EasyOrder.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ReadDbContext readContext, WriteDbContext writeContext) : base(readContext, writeContext)
        {
        }
    }
}
