using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models
{
    public class DepartmentDetailsViewModel
    {
        public Department Department { get; set; }
        public Employee Employee { get; set; }
        public List<Employee> AllEmployees { get; set; } = new List<Employee>();
        public List<SelectListItem> AllEmployeeOptions
        {
            get
            {
                if (AllEmployees == null)
                {
                    return null;
                }

                return AllEmployees
                        .Select((d) => new SelectListItem(d.FirstName, d.Id.ToString()))
                        .ToList();
            }
        }
    }
}