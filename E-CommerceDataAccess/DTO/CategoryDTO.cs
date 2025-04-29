using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.DTO
{
   public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }
    public class CategoryCreateDTO
    {
        public string Name { get; set; }
    }

    public class CategoryUpdateDTO
    {
        public string Name { get; set; }
    }

}
