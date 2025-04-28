using E_CommerceDataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Data
{
    public class AppDbContext : IdentityDbContext<UserAccount>

    {
        public AppDbContext(DbContextOptions options ):base(options)
        {
            
        }
    }
}
