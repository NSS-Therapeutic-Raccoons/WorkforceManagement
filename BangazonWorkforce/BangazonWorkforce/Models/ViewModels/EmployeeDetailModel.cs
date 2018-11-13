using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models
{
    public class EmployeeDetailModel
    {
        public Employee Employee { get; set; }
        public List<Department> AllDepartments { get; set; }
        public List<TrainingProgram> AllTrainingPrograms { get; set;}
        
        }
    }
/*
Create SQL statement to pull trainingprogram names and add to a new list of strings. 
I only need the names of the training Programs. I dont need the model of trainingprogram  
  */



