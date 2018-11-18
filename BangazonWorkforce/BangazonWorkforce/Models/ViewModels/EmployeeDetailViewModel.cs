using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeDetailViewModel
    {
        
        public Employee Employee { get; set; }

        [Display(Name = "Assigned Computer")]
        public Computer Computer { get; set; }

        [Display(Name = "All Training Programs")]
        public List<TrainingProgram> TrainingPrograms { get; set; }

    }
}
