using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.DTOs.Responses
{
    public class ImageUploadResponse
    {
        public bool Success { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
