using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace EasyOrderProduct.Application.Contract.Hubs
{
    public class InventoryHub : Hub
    {
        public Task Subscribe(int productItemId)
            => Groups.AddToGroupAsync(Context.ConnectionId, $"product_{productItemId}");
    }
}
