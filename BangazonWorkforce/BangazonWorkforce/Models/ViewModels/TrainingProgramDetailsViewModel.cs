using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class TrainingProgramDetailsViewModel
    {
        
        public TrainingProgram TrainingProgram { get; set; }

        public List<Employee> Employees { get; set; }

    }
}
