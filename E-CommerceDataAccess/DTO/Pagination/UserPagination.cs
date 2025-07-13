using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.DTO.Pagination
{
    public class UserPaginationParams
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; }
        public string? RoleFilter { get; set; }
        public string? SortBy { get; set; } = "username";
        public bool SortDescending { get; set; } = false;
    }
}
