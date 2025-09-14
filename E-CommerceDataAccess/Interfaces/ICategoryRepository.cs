using E_CommerceDataAccess.BaseRepositry;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Interfaces
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<IEnumerable<Category>> GetAllCategories();
    }
}
