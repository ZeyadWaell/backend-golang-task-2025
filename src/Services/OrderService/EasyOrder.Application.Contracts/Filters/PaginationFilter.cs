using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Filters
{
    public class PaginationFilter
    {
        const int maxPageSize = 50;
        private int _defaultPageSize = 10;

        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = _defaultPageSize;
        }

        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; set; }
        public int PageSize
        {
            get => _defaultPageSize;
            set => _defaultPageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}
