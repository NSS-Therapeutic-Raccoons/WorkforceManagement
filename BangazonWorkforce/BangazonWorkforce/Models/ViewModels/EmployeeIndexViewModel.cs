using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{

    /*
         * Class: EmployeeIndexViewModel
         * Purpose: This View Model accepts a list of employees from the database for display on the MVC Index list of employees. It mainly exists for scalability.
         * Author: Ricky Bruner
         * Methods: None. This is a super simple view model.
         * Yeah it's pretty dumb
    */
    public class EmployeeIndexViewModel
    {

        public IEnumerable<Employee> Employees { get; set; }

    }
}
