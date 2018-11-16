using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models
{
    public class DepartmentIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Budget { get; set; }
        public int EmployeeCount { get; set; }
    }
}
