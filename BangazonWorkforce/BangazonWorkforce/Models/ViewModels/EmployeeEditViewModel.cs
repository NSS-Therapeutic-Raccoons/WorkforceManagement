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

        [Display(Name = "Current Enrolled Training Programs")]
        public List<TrainingProgram> EmployeeTrainingPrograms { get; set; } 

        public List<int> PreselectedTrainingPrograms { get; set; } 

        public List<int> SelectedTrainingProgramIds { get; set; }

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
                List<SelectListItem> computerOptions = new List<SelectListItem>();
                computerOptions = AllComputers
                        .Select((c) => new SelectListItem(c.Make, c.Id.ToString()))
                        .ToList();

                if (Computer.Id == 0) 
                {
                    computerOptions.Insert(0, new SelectListItem
                    {
                        Text = "Assign a Computer...",
                        Value = "0"
                    });
                }
                
                return computerOptions;
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

                PreselectedTrainingPrograms = EmployeeTrainingPrograms.Select((tp) => tp.Id).ToList();

                List<SelectListItem> allOptions = AllTrainingPrograms
                        .Select((tp) => new SelectListItem(tp.Name, tp.Id.ToString()))
                        .ToList();
                foreach (int Id in PreselectedTrainingPrograms) {
                    foreach (SelectListItem sli in allOptions) {
                        if (sli.Value == Id.ToString())
                        {
                            sli.Selected = true;
                            
                        }
                    }
                }

                return allOptions;
            }
        }
    }
}
