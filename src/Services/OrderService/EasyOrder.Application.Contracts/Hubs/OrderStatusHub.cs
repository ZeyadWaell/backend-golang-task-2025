using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Hubs
{
    public class OrderStatusHub : Hub
    {
        public Task Subscribe(int orderId)
            => Groups.AddToGroupAsync(Context.ConnectionId, $"order_{orderId}");
    }
}
