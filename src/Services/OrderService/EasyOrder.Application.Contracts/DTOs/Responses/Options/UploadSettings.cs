using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.DTOs.Responses.Options
{
    public class UploadSettings
    {

        public string ImageFolder { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
    }
}
