using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{

    /*
        * Author: Ricky Bruner
        * Purpose: House all of the data required to display the Employee Edit View.
              Why?:     
                - Employee: For the editing of the DepartmentId, FirstName, LastName, and IsSupervisor Columns.
                - AllDepartments: A list of every department for the Select on the Edit View.
                - Computer: The Computer being assigned/reassigned to the Employee. In full form because the Employee does not house a ComputerId column.
                - AllComputers: A list of every computer for the select on the Edit View.
                - EmployeeTrainingPrograms: A list of every training program the employee is currently enrolled in.
                - PreselectedTrainingPrograms: A list of the Id's of the Employees Training Programs.
                - SelectedTrainingProgramIds: An empty list of integers that will house the Id's of the Training Programs selected from the multiselect on the Edit View.
                - AllTrainingPrograms: A list of every training program for the select on the Edit View.
                - AllDepartmentOptions: A List of SelectListItems made from AllDepartments.
                - AllComputerOptions: A List of SelectListItems made from AllComputers.
                - AllTrainingProgramOptions: A List of SelectListItems made from AllTrainingPrograms, with the ones representing the EmployeeTrainingPrograms already pre-selected.
    */
    public class EmployeeEditViewModel
    {
        public Employee Employee { get; set; }

        public List<Department> AllDepartments { get; set; }

        [Display(Name = "Assigned Computer")]
        public Computer Computer { get; set; }

        public List<Computer> AllComputers { get; set; }

        [Display(Name = "Current Enrolled Training Programs")]
        public List<TrainingProgram> EmployeeTrainingPrograms { get; set; } 

        public List<int> PreselectedTrainingProgramIds { get; set; } 

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

                PreselectedTrainingProgramIds = EmployeeTrainingPrograms.Select((tp) => tp.Id).ToList();

                List<SelectListItem> allOptions = AllTrainingPrograms
                        .Select((tp) => new SelectListItem(tp.Name, tp.Id.ToString()))
                        .ToList();

                foreach (int Id in PreselectedTrainingProgramIds) {
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
