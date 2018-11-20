using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models
{
    /*
    Class:      DepartmentIndexViewModel
    Author:     Daniel Figueroa
    Purpose:    This ViewModel accepts a count of employees in addition to the normal Department Details
    Methods:    None.
    */
    public class DepartmentIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Budget { get; set; }
        public int EmployeeCount { get; set; }
    }
}
