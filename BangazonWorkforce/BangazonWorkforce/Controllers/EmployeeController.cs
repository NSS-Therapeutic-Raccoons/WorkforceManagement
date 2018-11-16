using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Dapper;
using BangazonWorkforce.Models.ViewModels;

namespace BangazonWorkforce.Controllers
{
    public class EmployeeController : Controller
    {
        private IConfiguration _config;
        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public EmployeeController(IConfiguration config)
        {
            _config = config;
        }

        /*
         
             * Author: Ricky Bruner
         
             * Index calls for all Employees, and retrieves a department for the Employee via a join on DepartmentId. It then feeds these employees into the EmployeeIndexViewModel
         
        */
        public async Task<IActionResult> Index()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = @"SELECT e.Id, 
                                      e.FirstName,
                                      e.LastName, 
                                      e.IsSupervisor,
                                      e.DepartmentId,
                                      d.Id,
                                      d.Name,
                                      d.Budget
                                 FROM Employee e JOIN Department d on e.DepartmentId = d.Id
                             ORDER BY e.Id";
                IEnumerable<Employee> employees = await conn.QueryAsync<Employee, Department, Employee>(
                    sql,
                    (employee, department) => {
                        employee.Department = department;
                        return employee;
                    });

                EmployeeIndexViewModel viewModel = new EmployeeIndexViewModel();
                viewModel.Employees = employees;
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employee employee = await GetById(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // GET: Employee/Create
        /*Create makes a list of All the Departments and puts that list of Departments into the ViewModel to be used.*/
        public async Task<IActionResult> Create()
        {
            List<Department> allDepartments = await GetAllDepartments();
            EmployeeAddViewModel viewmodel = new EmployeeAddViewModel
            {
                AllDepartments = allDepartments
            };
            return View(viewmodel);
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Create will post a new Employee to the Database through the EmployeeAddEditViewModel.
        // The EmployeeAddEditViewModel takes an Employee model, List of Department models, and SelectListItems of those Departments.
        // Db connection is made to Insert the new Employee, Department is selected via SelectListItems using Department and DepartmentId
        // Page redirect back to Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeAddViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                List<Department> allDepartments = await GetAllDepartments();
                viewmodel.AllDepartments = allDepartments;
                return View(viewmodel);
            }

            Employee employee = viewmodel.Employee;

            using (IDbConnection conn = Connection)
            {
                string sql = $@"
                INSERT INTO Employee (
                    FirstName,
                    LastName,
                    IsSupervisor,
                    DepartmentId)
                VALUES (
                    '{employee.FirstName}',
                    '{employee.LastName}',
                    {(employee.IsSupervisor ? 1 : 0)},
                    {employee.DepartmentId});";

                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Department> allDepartments = await GetAllDepartments();

            List<Computer> allActiveComputers = await GetAllActiveComputers();

            List<TrainingProgram> employeeTrainingPrograms = await GetEmployeeTrainingPrograms(id.Value);

            List<TrainingProgram> allTrainingPrograms = await GetAllTrainingPrograms();

            Employee employee = await GetById(id.Value);

            Computer computer = await GetEmployeeComputer(id.Value);

            if (employee == null)
            {
                return NotFound();
            }

            EmployeeEditViewModel viewmodel = new EmployeeEditViewModel
            {
                Employee = employee,
                AllDepartments = allDepartments,
                AllComputers = allActiveComputers,
                Computer = computer,
                EmployeeTrainingPrograms = employeeTrainingPrograms,
                AllTrainingPrograms = allTrainingPrograms
            };

            return View(viewmodel);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel viewmodel)
        {
            if (id != viewmodel.Employee.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                List<Department> allDepartments = await GetAllDepartments();
                viewmodel.AllDepartments = allDepartments;
                return View(viewmodel);
            }

            Employee employee = viewmodel.Employee;

            List<TrainingProgram> employeeTrainingPrograms = viewmodel.SelectedTrainingPrograms;

            Computer computer = viewmodel.Computer;

            using (IDbConnection conn = Connection)
            {
                string sql = $@"UPDATE Employee 
                                   SET FirstName = '{employee.FirstName}', 
                                       LastName = '{employee.LastName}', 
                                       IsSupervisor = {(employee.IsSupervisor ? 1 : 0)},
                                       DepartmentId = {employee.DepartmentId}
                                 WHERE id = {id};";

                string computerSql = $@"UPDATE ComputerEmployee
                                            SET ComputerId = {computer.Id}
                                        WHERE EmployeeId = {employee.Id};";

                sql = sql + computerSql;

                await conn.ExecuteAsync(sql);

                return RedirectToAction(nameof(Index));
            }
        }


        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employee employee = await GetById(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"DELETE FROM Employee WHERE id = {id}";
                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }



        // Private async queries
        private async Task<Employee> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT e.Id, 
                                       e.FirstName,
                                       e.LastName, 
                                       e.IsSupervisor,
                                       e.DepartmentId,
                                       d.Id,
                                       d.Name,
                                       d.Budget
                                  FROM Employee e JOIN Department d on e.DepartmentId = d.Id
                                 WHERE e.id = {id}";
                IEnumerable<Employee> employees = await conn.QueryAsync<Employee, Department, Employee>(
                    sql,
                    (employee, department) => {
                        employee.Department = department;
                        return employee;
                    });

                return employees.SingleOrDefault();
            }
        }

        private async Task<List<Department>> GetAllDepartments()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT Id, Name, Budget FROM Department";

                IEnumerable<Department> departments = await conn.QueryAsync<Department>(sql);
                return departments.ToList();
            }
        }

        private async Task<List<Computer>> GetAllActiveComputers()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT c.Id, 
                                       c.Make, 
                                       c.Manufacturer 
                                FROM Computer c
                                WHERE c.DecomissionDate IS NULL";

                IEnumerable<Computer> computers = await conn.QueryAsync<Computer>(sql);
                return computers.ToList();
            }
        }

        private async Task<Computer> GetEmployeeComputer(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT c.Id, 
                                       c.Make, 
                                       c.Manufacturer 
                                FROM Computer c
                                JOIN ComputerEmployee ce ON ce.ComputerId = c.Id
                                JOIN Employee e ON e.Id = ce.EmployeeId
                                WHERE e.Id = {id}";

                Computer computer = await conn.QueryFirstAsync<Computer>(sql);
                return computer;
            }
        }



        private async Task<List<TrainingProgram>> GetAllTrainingPrograms()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"
                            SELECT tp.Id, 
                                   tp.[Name],
                                   tp.StartDate,
                                   tp.EndDate,
                                   tp.MaxAttendees
                            FROM TrainingProgram tp
                            ";

                IEnumerable<TrainingProgram> trainingPrograms = await conn.QueryAsync<TrainingProgram>(sql);
                return trainingPrograms.ToList();
            }
        }

        private async Task<List<TrainingProgram>> GetEmployeeTrainingPrograms(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"
                            SELECT tp.Id, 
                                   tp.[Name],
                                   tp.StartDate,
                                   tp.EndDate,
                                   tp.MaxAttendees
                            FROM TrainingProgram tp
                            JOIN EmployeeTraining etp ON etp.TrainingProgramId = tp.Id
                            JOIN Employee e On e.Id = etp.EmployeeId
                            WHERE e.Id = {id}
                            ";

                IEnumerable<TrainingProgram> trainingPrograms = await conn.QueryAsync<TrainingProgram>(sql);
                return trainingPrograms.ToList();
            }
        }
    }
}