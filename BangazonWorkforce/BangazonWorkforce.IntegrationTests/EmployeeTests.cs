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

        [Fact]
        public async Task Post_EditWillUpdateEmployee()
        {
            // Arrange
            Employee employee = (await GetAllEmloyees()).Last();
            Department department = (await GetAllDepartments()).Last();

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


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                editPage,
                new Dictionary<string, string>
                {
                    {"Employee_FirstName", firstName},
                    {"Employee_LastName", lastName},
                    {"Employee_IsSupervisor", isSupervisor},
                    {"Employee_DepartmentId", departmentId}
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

            IHtmlInputElement cb = (IHtmlInputElement)lastRow.QuerySelector("input[type='checkbox']");
            if (isSupervisor == "true")
            {
                Assert.True(cb.IsChecked);
            }
            else
            {
                Assert.False(cb.IsChecked);
            }
        }

        private async Task<List<Employee>> GetAllEmloyees()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
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
            
            * Purpose: Grab a Single employee with its department joined to it from the database for the Get_IndexContentVerified test above. 
        */
        private async Task<Employee> GetFullEmployee()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
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
            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
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

         /*Author: Jeremiah Pritchard
          * 
          * Purpose:Using a specific user to make sure fields are populated.
          * */
         [Fact]
         public async Task Get_EmployeeDetailVerification()
         {
         //Arrange
         string url = "/employee/details/1";

            //Act
            HttpResponseMessage response = await _client.GetAsync(url);


            //Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            IHtmlDocument DetailPage = await HtmlHelpers.GetDocumentAsync(response);


            Assert.Contains(
               DetailPage.QuerySelectorAll("dd"),
               dd => dd.TextContent.Contains("Mike"));
            Assert.Contains(
             DetailPage.QuerySelectorAll("dd"),
             dd => dd.TextContent.Contains("Parrish"));
            Assert.Contains(
            DetailPage.QuerySelectorAll("span"),
            span => span.TextContent.Contains("No"));
            Assert.Contains(
             DetailPage.QuerySelectorAll("dd"),
             dd => dd.TextContent.Contains("Navy"));
            Assert.Contains(
            DetailPage.QuerySelectorAll("dd"),
            dd => dd.TextContent.Contains("Surface Tablet"));
            Assert.Contains(
         DetailPage.QuerySelectorAll("dd"),
         dd => dd.TextContent.Contains("You Are the First Line of Defence!"));



        }


   }














}
