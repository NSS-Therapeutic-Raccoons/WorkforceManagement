using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Dapper;

/*
 Author: Jeremiah Pritchard
 
 Purpose: This is a ViewModel that generates Employee data to be used on the Employee Details View. 
 */

namespace BangazonWorkforce.Models
{
    public class EmployeeDetailsViewModel
    {
        public Employee Employee { get; set; }
        public Department Department { get; set; }
        public List<string> TrainingPrograms = new List<string>();
        public List<Computer> Computers { get; set; } = new List<Computer>();

        public EmployeeDetailsViewModel(IConfiguration config, int Id, Employee employee)
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

                IEnumerable<Computer> computers = conn.Query<Computer>($@"
                        SELECT
				            c.Make,
                            c.Manufacturer
                        FROM Computer c
                        JOIN ComputerEmployee ce ON c.Id = ce.ComputerId
                        JOIN Employee e ON e.Id = ce.EmployeeId
			            WHERE e.Id = {Id}  
                        ");
                Computers = computers.ToList();
            }
        }
    }
}

