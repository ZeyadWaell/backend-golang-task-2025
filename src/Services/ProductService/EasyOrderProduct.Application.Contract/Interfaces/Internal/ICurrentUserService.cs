﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contracts.Interfaces.InternalServices
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserRole { get;  }
    }
}
