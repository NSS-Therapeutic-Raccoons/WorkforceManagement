using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace BangazonWorkforce.Controllers
{
    public class TrainingProgramController : Controller
    {
        private IConfiguration _config;

        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public TrainingProgramController(IConfiguration config)
        {
            _config = config;
        }

        // GET: TrainingProgram
        public async Task<IActionResult> Index()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = @"SELECT tp.Id, 
                                      tp.[Name],
                                      tp.StartDate, 
                                      tp.EndDate,
                                      tp.MaxAttendees
                                FROM TrainingProgram tp
                                ORDER BY tp.Id";
                IEnumerable<TrainingProgram> trainingPrograms = await conn.QueryAsync<TrainingProgram>(sql);

                TrainingProgramIndexViewModel viewModel = new TrainingProgramIndexViewModel();
                viewModel.TrainingPrograms = trainingPrograms.Where(tp => tp.StartDate > DateTime.Today).ToList();
                return View(viewModel);
            }
        }

        // GET: TrainingProgram/Details/5
        public async Task<IActionResult> Details(int id)
        {

            TrainingProgramDetailsViewModel viewmodel = new TrainingProgramDetailsViewModel();

            viewmodel.TrainingProgram = await GetTrainingProgramById(id);
            viewmodel.Employees = await GetTrainingProgramEmployees(id);
            
            
        
            return View(viewmodel);
        }

        

        // GET: TrainingProgram/Create
        public ActionResult Create()
        {
            TrainingProgramCreateViewModel viewmodel = new TrainingProgramCreateViewModel();
            
            return View(viewmodel);
        }

        // POST: TrainingProgram/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingProgramCreateViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewmodel);
            }


            TrainingProgram trainingProgram = viewmodel.TrainingProgram;

            using (IDbConnection conn = Connection)
            {
                string sql = $@"
                        INSERT INTO TrainingProgram 
                        ([Name], StartDate, EndDate, MaxAttendees) 
                        VALUES 
                        ('{trainingProgram.Name}', '{trainingProgram.StartDate}', '{trainingProgram.EndDate}', '{trainingProgram.MaxAttendees}');
                    ";

                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TrainingProgram/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            TrainingProgramEditViewModel viewmodel = new TrainingProgramEditViewModel {
                TrainingProgram = await GetTrainingProgramById(id)
            };
            return View(viewmodel);
        }

        // POST: TrainingProgram/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingProgramEditViewModel viewmodel)
        {
            if (id != viewmodel.TrainingProgram.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                viewmodel.TrainingProgram = await GetTrainingProgramById(id);
                return View(viewmodel);
            }

            TrainingProgram tp = viewmodel.TrainingProgram;

            using (IDbConnection conn = Connection)
            {
                string sql = $@"
                            UPDATE TrainingProgram
                            SET [Name] = '{tp.Name}',
                                StartDate = '{tp.StartDate}',
                                EndDate = '{tp.EndDate}',
                                MaxAttendees = '{tp.MaxAttendees}'
                            WHERE Id = {id}
                            ";

                await conn.ExecuteAsync(sql);

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TrainingProgram/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainingProgram/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private async Task<List<Employee>> GetTrainingProgramEmployees(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"
                            SELECT e.Id, 
                                   e.FirstName,
                                   e.LastName,
                                   e.IsSupervisor,
                                   e.DepartmentId
                            FROM Employee e
                            JOIN EmployeeTraining et ON et.EmployeeId = e.Id
                            JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
                            WHERE tp.Id = {id}
                            AND tp.StartDate > '{DateTime.Today}'
                            ";

                List<Employee> employees = (await conn.QueryAsync<Employee>(sql)).ToList();
                return employees;
            }
        }

        private async Task<TrainingProgram> GetTrainingProgramById(int id)
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
                            WHERE tp.Id = {id}
                            ";

                TrainingProgram trainingProgram = await conn.QueryFirstAsync<TrainingProgram>(sql);
                return trainingProgram;
            }
        }
    }
}