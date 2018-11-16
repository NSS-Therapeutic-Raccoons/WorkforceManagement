using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public Employee Employee { get; set; }

        public List<Department> AllDepartments { get; set; }

        [Display(Name = "Assigned Computer")]
        public Computer Computer { get; set; }

        //[Display]
        //public Computer SelectedComputer { get; set; }

        public List<Computer> AllComputers { get; set; }

        [Display(Name = "Select Training Programs")]
        public List<TrainingProgram> SelectedTrainingPrograms { get; set; }

        [Display(Name = "Current Enrolled Training Programs")]
        public List<TrainingProgram> EmployeeTrainingPrograms { get; set; }

        public List<TrainingProgram> AllTrainingPrograms { get; set; }

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
        public List<SelectListItem> AllComputerOptions
        {
            get
            {
                if (AllComputers == null)
                {
                    return null;
                }

                return AllComputers
                        .Select((c) => new SelectListItem(c.Make, c.Id.ToString()))
                        .ToList();
            }
        }
        public List<SelectListItem> AllTrainingProgramOptions
        {
            get
            {
                if (AllTrainingPrograms == null)
                {
                    return null;
                }

                return AllTrainingPrograms
                        .Select((tp) => new SelectListItem(tp.Name, tp.Id.ToString()))
                        .ToList();
            }
        }
    }
}
