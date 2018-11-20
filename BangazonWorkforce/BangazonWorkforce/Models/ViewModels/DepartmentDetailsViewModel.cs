using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models
{
    public class DepartmentDetailsViewModel
    {
        public Department Department { get; set; }
        public List<Employee> AllEmployees { get; set; } = new List<Employee>();
    }
}