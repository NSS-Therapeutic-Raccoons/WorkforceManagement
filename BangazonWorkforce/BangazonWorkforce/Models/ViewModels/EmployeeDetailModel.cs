using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace BangazonWorkforce.Models
{
    public class EmployeeDetailModel
    {
        public Employee Employee { get; set; }
        public Department Department { get; set; }
        public List<string> TrainingPrograms = new List<string>();

        public EmployeeDetailModel(IConfiguration config, int Id, Employee employee)
        {
            using (IDbConnection conn = new SqlConnection(config.GetConnectionString("DefaultConnection")))
            {
                IEnumerable<TrainingProgram> trainingPrograms = conn.Query<TrainingProgram>($@"
                 SELECT
				    tp.Name
                FROM TrainingProgram tp
                JOIN EmployeeTraining et ON tp.Id = et.TrainingProgramId
                JOIN Employee e ON e.Id = et.EmployeeId
			    WHERE e.Id = {Id}    
                ");
                TrainingPrograms = trainingPrograms.Select(tp => tp.Name).ToList();

                Employee = employee;
            }
        }
    }
}
/*
Create SQL statement to pull trainingprogram names and add to a new list of strings. 
I only need the names of the training Programs. I dont need the model of trainingprogram  
  */

