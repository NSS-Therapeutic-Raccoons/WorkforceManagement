using AngleSharp.Dom.Html;
using BangazonWorkforce.IntegrationTests.Helpers;
using BangazonWorkforce.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

//Added assert for input values of createhtml
namespace BangazonWorkforce.IntegrationTests
{
    public class DepartmentTests :
        IClassFixture<WebApplicationFactory<BangazonWorkforce.Startup>>
    {
        private readonly HttpClient _client;

        public DepartmentTests(WebApplicationFactory<BangazonWorkforce.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_IndexReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            string url = "/department";

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        /*
        Author:     Daniel Figueroa
        Purpose:    Verifies that the first item in the table matches the HTML element on screen.
        */
        [Fact]
        public async Task Get_DepartmentIndexContentVerified()
        {
            // Arrange
            string url = "/department";
            DepartmentIndexViewModel fullDepartment = (await GetFullDepartment());

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);

            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => td.TextContent.Contains(fullDepartment.Name));
            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => td.TextContent.Contains(fullDepartment.Budget.ToString()));
            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => td.TextContent.Contains(fullDepartment.EmployeeCount.ToString()));
        }

        /*
        Author:     Daniel Figueroa
        Purpose:    Verified that the content from the first department always matches the HTML element on screen.
        */
        [Fact]
        public async Task Get_DepartmentDetailsContentVerified()
        {
            // Arrange
            DepartmentIndexViewModel fullDepartment = (await GetFullDepartment());
            DepartmentDetailsViewModel singleDepartment = (await GetFullDepartmentDetails(fullDepartment.Id));
            string url = $"/department/details/{singleDepartment.Department.Id}";

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);

            Assert.Contains(indexPage.QuerySelectorAll("dd"),
                dd => dd.TextContent.Contains(singleDepartment.Department.Name));
            Assert.Contains(indexPage.QuerySelectorAll("dd"),
                dd => dd.TextContent.Contains(singleDepartment.Department.Budget.ToString()));
            if (singleDepartment.AllEmployees.Count == 0)
            {
                Assert.Contains(indexPage.QuerySelectorAll("li"),
                li => li.TextContent.Contains("No Employees"));
            }
            else
            {
                Assert.Contains(indexPage.QuerySelectorAll("li"),
                li => li.TextContent.Contains(singleDepartment.AllEmployees.First<Employee>().FirstName));
            }
        }

        [Fact]
        public async Task Post_CreateAddsDepartment()
        {
            // Arrange
            string url = "/department/create";
            HttpResponseMessage createPageResponse = await _client.GetAsync(url);
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);

            string newDepartmentName = StringHelpers.EnsureMaxLength("Dept-" + Guid.NewGuid().ToString(), 55);
            string newDepartmentBudget = new Random().Next().ToString();


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                createPage,
                new Dictionary<string, string>
                {
                    {"Name", newDepartmentName},
                    {"Budget", newDepartmentBudget}
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            Assert.Contains(
                indexPage.QuerySelectorAll("td"), 
                td => td.TextContent.Contains(newDepartmentName));
            Assert.Contains(
                indexPage.QuerySelectorAll("td"), 
                td => td.TextContent.Contains(newDepartmentBudget));
            Assert.Contains(
                createPage.QuerySelectorAll("Input"),
                i => i.Id == "Name");
            Assert.Contains(
               createPage.QuerySelectorAll("Input"),
               i => i.Id == "Budget");
        }

        /*
        Author:     Daniel Figueroa
        Purpose:    Queries for the first item in Department and Employee table to build DepartmentIndexViewModel to test
                    against DepartmentIndexContentVerfied
        */
        private async Task<DepartmentIndexViewModel> GetFullDepartment()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
            {
                string sql = $@"
                            SELECT
                                d.Id,
                                d.Name,
                                d.Budget,
                                COUNT(e.Id) AS 'Employee Count'
                            FROM Department d
                            LEFT OUTER JOIN Employee e ON d.Id = e.DepartmentId
                            GROUP BY d.Id, d.Name, d.Budget";
                var departments = await conn.QueryFirstAsync<DepartmentIndexViewModel>(sql);
                return departments;
            }
        }

        /*
        Author:     Daniel Figueroa
        Purpose:    Queries for the first item in Department and Employee table to build DepartmentIndexViewModel to test
                    against DepartmentIndexContentVerfied
        */
        private async Task<DepartmentDetailsViewModel> GetFullDepartmentDetails(int id)
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
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
                return placeholder;
            }
        }
    }
}
