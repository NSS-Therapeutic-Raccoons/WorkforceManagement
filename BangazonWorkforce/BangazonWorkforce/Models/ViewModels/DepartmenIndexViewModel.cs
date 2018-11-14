using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models
{
    public class DepartmentIndexViewModel
    {
        public Department Department { get; set; }
        public List<Employee> AllEmployees { get; set; }
        public List<SelectListItem> AllDepartmentOptions
        {
            get
            {
                if (AllDepartments == null)
                {
                    return null;
                }

                return AllDepartments
                        .Select((d) => new SelectListItem(d.Name, d.Id.ToString()))
                        .ToList();
            }
        }
    }
}
