//Computer MVC controller which will have basic CRUD functionality and display on HTML

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
    public class ComputerController : Controller
    {
        private IConfiguration _config;
        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public ComputerController(IConfiguration config)
        {
            _config = config;
        }




        public async Task<IActionResult> Index()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = 
                    "SELECT Id, PurchaseDate, Manufacturer, Make, DecomissionDate FROM Computer";
                IEnumerable<Computer> computers = await conn.QueryAsync<Computer>(sql);

                return View(computers);
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Computer computer = await GetById(id.Value);
            if (computer == null)
            {
                return NotFound();
            }
            return View(computer);
        }

        // GET: Computer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Computer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PurchaseDate, DecomissionDate, Manufacturer, Make")] Computer computer)
        {
            if (!ModelState.IsValid)
            {
                return View(computer);
            }

            using (IDbConnection conn = Connection)
            {
                string sql = $@"INSERT INTO Computer (PurchaseDate, Manufacturer, Make) 
                                     VALUES ('{computer.PurchaseDate}', '{computer.Manufacturer}', '{computer.Make}' );";

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

            Computer computer = await GetById(id.Value);
            if (computer == null)
            {
                return NotFound();
            }
            return View(computer);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Computer computer)
        {
            if (id != computer.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(computer);
            }

            using (IDbConnection conn = Connection)
            {
                string sql = $@"UPDATE Computer 
                                 SET Purchasedate = {computer.PurchaseDate},
                                 DecomissionDate = {computer.DecomissionDate},
                                 Manufacturer = '{computer.Manufacturer}',
                                 Make = '{computer.Make}'
                                       
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

            Computer computer = await GetById(id.Value);
            if (computer == null)
            {
                return NotFound();
            }
            return View(computer);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
                using (IDbConnection conn = Connection)
                {

                    string sql = $@"DELETE FROM Computer WHERE id = {id}";
                try {
                    await conn.ExecuteAsync(sql);

                } catch (SqlException e)
                {
                    Console.WriteLine(e);
                    return RedirectToAction(nameof(Index));
                 };

                    /*
                    if (rowsDeleted > 0)
                    {
                        return NotFound();
                    }
    */
                    //find id match in computeremployee and if employeeId not NULL present error
                    //can't delete anyway because get foreign key constraint
                    return RedirectToAction(nameof(Index));
                }
            
        }


        private async Task<Computer> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT Id, PurchaseDate, DecomissionDate, Manufacturer, Make 
                                  FROM Computer
                                 WHERE id = {id}";

                IEnumerable<Computer> computers = await conn.QueryAsync<Computer>(sql);
                return computers.SingleOrDefault();
            }
        }
    }
}