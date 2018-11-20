using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using BangazonWorkforce.Models;

namespace BangazonWorkforce.Controllers
{
    public class DepartmentController : Controller
    {
        private IConfiguration _config;
        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public DepartmentController(IConfiguration config)
        {
            _config = config;
        }


        /*
            * Author: Klaus Hardt (Ticket #1)
            * Index calls for all Department including the name and budget. 

            * Author: Daniel Figueroa (Ticket #6)
            * Comment: Index() now calls on view model 'DepartmentIndexViewModel' to display
            * department name/budget and Employee count for that department.
       */

        public async Task<IActionResult> Index()
        {
            using (IDbConnection conn = Connection)
            {
                Dictionary<int, List<Employee>> report = new Dictionary<int, List<Employee>>();
                string sql = $@"
                            SELECT
                                d.Id,
                                d.Name,
                                d.Budget,
                                COUNT(e.Id) AS EmployeeCount
                            FROM Department d
                            LEFT OUTER JOIN Employee e ON d.Id = e.DepartmentId
                            GROUP BY d.Id, d.Name, d.Budget";
                IEnumerable<DepartmentIndexViewModel> departments = await conn.QueryAsync<DepartmentIndexViewModel>(sql);
                return View(departments);
            }
        }

        /*
        Author:     Daniel Figueroa (Ticket #7)
        Comment:    Details() calls on view model 'DepartmentDetailsViewModel' to display
                    department name/budget and list of Employese within that department.
       */
        public async Task<ActionResult> Details(int id)
        {
            DepartmentDetailsViewModel placeholder = new DepartmentDetailsViewModel();
            string sql = $@"
            SELECT
                d.Id,
                d.Name,
                d.Budget,
                e.Id, 
                e.FirstName,
                e.LastName, 
                e.IsSupervisor,
                e.DepartmentId
            FROM Department d
            LEFT OUTER JOIN Employee e ON d.Id = e.DepartmentId
            WHERE d.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Department> departments = await conn.QueryAsync<Department, Employee, Department>(
                sql,
                (department, employee) =>
                    {
                        placeholder.Department = department;
                        if (employee != null)
                        {
                            placeholder.AllEmployees.Add(employee);
                        }
                        return (department);
                    });
                    return View(placeholder);
            }
        }

    // GET: Department/Create
    public IActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Budget")] Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            using (IDbConnection conn = Connection)
            {
                string sql = $@"INSERT INTO Department (Name, Budget) 
                                     VALUES ('{department.Name}', {department.Budget});";

                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Department department = await GetById(id.Value);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(department);
            }

            using (IDbConnection conn = Connection)
            {
                string sql = $@"UPDATE Department 
                                   SET Name = '{department.Name}', 
                                       Budget = {department.Budget}
                                 WHERE id = {id}";

                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }


        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Department department = await GetById(id.Value);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            using (IDbConnection conn = Connection)

            {
                string sql = $@"UPDATE Employee
                SET DepartmentID = 0 
                where DepartmentId = {id}";


            }

            using (IDbConnection conn = Connection)
            {

        
                string sql = $@"DELETE FROM Department WHERE id = {id}";

                await conn.ExecuteAsync(sql);

                return RedirectToAction(nameof(Index));
            }
        }


        private async Task<Department> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT Id, Name, Budget 
                                  FROM Department
                                 WHERE id = {id}";

                IEnumerable<Department> departments = await conn.QueryAsync<Department>(sql);
                return departments.SingleOrDefault();
            }
        }

        private async Task<List<Employee>> GetDepartmentEmployees(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"
                    SELECT Id,
                           FirstName,
                           LastName,
                           IsSupervisor,
                           DepartmentId
                    FROM Employee
                    WHERE DepartmentId = {id}";
                List<Employee> employees = (await conn.QueryAsync<Employee>(sql)).ToList();
                return employees;
            }
        }
    }
}