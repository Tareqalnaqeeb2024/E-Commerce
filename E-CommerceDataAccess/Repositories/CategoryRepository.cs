using E_CommerceDataAccess.BaseRepositry;
using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Repositories
{
    public class CategoryRepository :BaseRepository<Category>,  ICategoryRepository
    {

        public CategoryRepository(AppDbContext context):base(context) 
        {
        }

     
    
    
    }
}
