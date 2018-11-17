using AngleSharp.Dom.Html;
using BangazonWorkforce.IntegrationTests.Helpers;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.IntegrationTests
{
    
    /*
        * Author: Theraputic Raccoons 
        * Purpose: Houses the employee specific integration tests and neccesary database queries to ensure that the employees section words properly.
    */
    public class EmployeeTests :
        IClassFixture<WebApplicationFactory<BangazonWorkforce.Startup>>
    {
        private readonly HttpClient _client;

        public EmployeeTests(WebApplicationFactory<BangazonWorkforce.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_IndexReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            string url = "/employee";
            
            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }


        /*
            * Author: Ricky Bruner 
            * Purpose: This test verifies that the index page displays a first and last name along with correct department name for any employee from the database.
        */
        [Fact]
        public async Task Get_IndexContentVerified()
        {
            // Arrange
            string url = "/employee";
            Employee fullEmployee = (await GetFullEmployee());

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);

            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => td.TextContent.Contains(fullEmployee.FirstName));
            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => td.TextContent.Contains(fullEmployee.LastName));
            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => td.TextContent.Contains(fullEmployee.Department.Name));

        }
    


        [Fact]
        public async Task Post_CreateAddsEmployee()
        {
            // Arrange
            Department department = (await GetAllDepartments()).First();
            string url = "/employee/create";
            HttpResponseMessage createPageResponse = await _client.GetAsync(url);
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);

            string newFirstName = "FirstName-" + Guid.NewGuid().ToString();
            string newLastName = "LastName-" + Guid.NewGuid().ToString();
            string isSupervisor = "true";
            string departmentId = department.Id.ToString();
            string departmentName = department.Name;


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                createPage,
                new Dictionary<string, string>
                {
                    {"Employee_FirstName", newFirstName},
                    {"Employee_LastName", newLastName},
                    {"Employee_IsSupervisor", isSupervisor},
                    {"Employee_DepartmentId", departmentId}
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            var lastRow = indexPage.QuerySelector("tbody tr:last-child");

            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newFirstName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newLastName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(departmentName));
        }


        /*
            * Author: Ricky Bruner 
            * Purpose: This test gets all pertinent data from the db needed to update an employee, then changes it to recognizeably changed data. It then runs an edit and asserts that the chages were made from the index view.
        */
        [Fact]
        public async Task Post_EditWillUpdateEmployee()
        {
            // Arrange
            Employee employee = (await GetAllEmloyees()).Last();
            Department department = (await GetAllDepartments()).Last();
            Computer computer = (await GetAllComputers()).Where(c => c.DecomissionDate == null).Last();
            string selectedId = (await GetAllTrainingPrograms()).Select((tp) => tp.Id).ToList().Last().ToString();

            string url = $"employee/edit/{employee.Id}";
            HttpResponseMessage editPageResponse = await _client.GetAsync(url);
            IHtmlDocument editPage = await HtmlHelpers.GetDocumentAsync(editPageResponse);

            string firstName = StringHelpers.EnsureMaxLength(
                employee.FirstName + Guid.NewGuid().ToString(), 55);
            string lastName = StringHelpers.EnsureMaxLength(
                employee.LastName + Guid.NewGuid().ToString(), 55);
            string isSupervisor = employee.IsSupervisor ? "false" : "true";
            string departmentId = department.Id.ToString();
            string departmentName = department.Name;
            string computerId = computer.Id.ToString();

            

            // Act
            HttpResponseMessage response = await _client.SendAsync(
                editPage,
                new Dictionary<string, string>
                {
                    {"Employee_FirstName", firstName},
                    {"Employee_LastName", lastName},
                    {"Employee_IsSupervisor", isSupervisor},
                    {"Employee_DepartmentId", departmentId},
                    {"Computer_Id", computerId},
                    {"SelectedTrainingProgramIds", selectedId }
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            var lastRow = indexPage.QuerySelector("tbody tr:last-child");

            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(firstName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(lastName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(departmentName));

        }

        private async Task<List<Employee>> GetAllEmloyees()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionSring))
            {
                IEnumerable<Employee> allEmployees =
                    await conn.QueryAsync<Employee>( @"
                    SELECT
                        Id,
                        FirstName,
                        LastName,
                        IsSupervisor,
                        DepartmentId
                    FROM Employee
                    ORDER BY Id");
                return allEmployees.ToList();
            }
        }


        /* 
            * Author: Ricky Bruner
            * Purpose: Grab a Single employee with its department joined to it from the database.
        */
        private async Task<Employee> GetFullEmployee()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionSring))
            {
                string sql = @"
                    SELECT TOP 1 
                        e.Id, 
                        e.FirstName, 
                        e.LastName, 
                        e.IsSupervisor, 
                        e.DepartmentId,
                        d.Id,
                        d.Name,
                        d.Budget
                    FROM Employee e
                    JOIN Department d ON d.Id = e.DepartmentId
                ";
                IEnumerable<Employee> Employees =
                    await conn.QueryAsync<Employee, Department, Employee>(sql, (employee, department) => {
                        employee.Department = department;
                        return employee;
                    });
                return Employees.First();
            }
        }

        private async Task<List<Department>> GetAllDepartments()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionSring))
            {
                IEnumerable<Department> allDepartments = 
                    await conn.QueryAsync<Department>(@"
                    SELECT
                        Id,
                        Name,
                        Budget
                    FROM Department");
                return allDepartments.ToList();
            }
        }


        /*
            * Author: Ricky Bruner 
            * Purpose: Get all Computers from the Database.
        */
        private async Task<List<Computer>> GetAllComputers()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionSring))
            {
                IEnumerable<Computer> allComputers =
                    await conn.QueryAsync<Computer>(@"
                    SELECT
                        Id,
                        PurchaseDate,
                        DecomissionDate,
                        Make,
                        Manufacturer
                    FROM Computer");
                return allComputers.ToList();
            }
        }


        /*
            * Author: Ricky Bruner 
            * Purpose: Get all Training Programs from the Database.
        */
        private async Task<List<TrainingProgram>> GetAllTrainingPrograms()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionSring))
            {
                IEnumerable<TrainingProgram> allTrainingPrograms =
                    await conn.QueryAsync<TrainingProgram>(@"
                    SELECT
                        Id,
                        [Name],
                        StartDate,
                        EndDate,
                        MaxAttendees
                    FROM TrainingProgram");
                return allTrainingPrograms.ToList();
            }
        }
    }
}
