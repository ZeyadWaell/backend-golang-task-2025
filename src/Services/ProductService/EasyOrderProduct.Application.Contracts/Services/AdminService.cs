using EasyOrderProduct.Application.Contract.Interfaces;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contract.Services
{
    
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseApiResponse> GetAllLowStockAsync()
        {
            var lowStockProducts = await _unitOfWork.InventoryRepository.GetLowStockAsync();

            return lowStockProducts;
        }

    }
}
