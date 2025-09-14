using E_CommerceDataAccess.BaseRepositry;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.UnitOfWork
{
    public interface IUnitOfwork : IDisposable
    {
        ICategoryRepository categories { get; }
        IProductRepository    products { get; }
        IOrderItemRepository orderItems { get; }
        IOrderRepository orders { get; }
        IUserRepository users { get; }
     

        int Complete(); 
        Task <int> CompleteAsync();

    }
}
