using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Hubs
{
    public class ProductHub : Hub
    {
        public async Task UpdateProductStock(string productId, int newStock)
        {
            await Clients.All.SendAsync("ReceiveStockUpdate", productId, newStock);
        }
    }
}
