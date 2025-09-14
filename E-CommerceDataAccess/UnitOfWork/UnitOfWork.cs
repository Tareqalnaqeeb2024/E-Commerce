using E_Commerce.DataAccess.Repositories;
using E_CommerceDataAccess.BaseRepositry;
using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using E_CommerceDataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfwork
    {
        private readonly AppDbContext _context;

        public ICategoryRepository categories { get; private set; }

        public IProductRepository products {  get; private set; }

        public IOrderItemRepository orderItems {  get; private set; }

        public IOrderRepository orders {  get; private set; }

        public IUserRepository users {  get; private set; }

        private readonly UserManager<UserAccount> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UnitOfWork(AppDbContext context, UserManager<UserAccount> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            categories = new CategoryRepository(_context);
            products = new ProductRepository(_context);
            orderItems = new OrderItemRepository(_context);
            orders = new OrderRepository(_context);
            users = new UserRepository( _userManager, _roleManager , _context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }
            
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> CompleteAsync()
        {
          return  await  _context.SaveChangesAsync();    
        }
    }
}
