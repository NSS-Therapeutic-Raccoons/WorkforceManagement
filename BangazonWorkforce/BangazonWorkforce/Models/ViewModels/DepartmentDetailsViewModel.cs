using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models
{
    /*
    Class:      DepartmentDetailsViewModel
    Author:     Daniel Figueroa
    Purpose:    This ViewModel accepts a Department class and List of Employee classes
    Methods:    None.
    */
    public class DepartmentDetailsViewModel
    {
        public Department Department { get; set; }
        public List<Employee> AllEmployees { get; set; } = new List<Employee>();
    }
}