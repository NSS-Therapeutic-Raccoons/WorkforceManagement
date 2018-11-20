/*the computer test has verification for display for index, input for create and delete
NOTE that the create for testing purposes only! sets the create view to accept text as input type. This changes it from using a 
drop down calander and date time slots. This is an issue with the library used for the integration tests where it cannot convert 
the date time to string properly. If create html has input type removed the app will work with a date time drop down however
the test will fail. 

*/
using AngleSharp.Dom.Html;
using BangazonWorkforce.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using BangazonWorkforce.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

//Added assert for input values of createhtml
namespace BangazonWorkforce.IntegrationTests
{
    public class ComputerTests :
        IClassFixture<WebApplicationFactory<BangazonWorkforce.Startup>>
    {
        private readonly HttpClient _client;

        public ComputerTests(WebApplicationFactory<BangazonWorkforce.Startup> factory)
        {
            _client = factory.CreateClient();
        }
        
        [Fact]
        public async Task Get_IndexReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            string url = "/computer";
            
            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_CreateAddsComputer()
        {
            // Arrange
            string url = "computer/create";
            HttpResponseMessage createPageResponse = await _client.GetAsync(url);
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);

           
            string newComputerPurchaseDate = DateTime.Now.ToString();
            string newComputerManufacturer = "Manufacturer-" + Guid.NewGuid().ToString();
            string newComputerMake = "Make-" + Guid.NewGuid().ToString();


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                createPage,
                new Dictionary<string, string>
                {
                    {"PurchaseDate", newComputerPurchaseDate},
                    {"Manufacturer", newComputerManufacturer},
                    {"Make", newComputerMake}
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            
           Assert.Contains(
                indexPage.QuerySelectorAll("td"), 
               td => td.TextContent.Contains(newComputerPurchaseDate));
                
            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newComputerManufacturer));
            Assert.Contains(
               indexPage.QuerySelectorAll("td"),
               td => td.TextContent.Contains(newComputerMake));
            
            Assert.Contains(
               createPage.QuerySelectorAll("Input"),
                i => i.Id == "PurchaseDate");
            Assert.Contains(
               createPage.QuerySelectorAll("Input"),
               i => i.Id == "Manufacturer");
            Assert.Contains(
              createPage.QuerySelectorAll("Input"),
              i => i.Id == "Make");
              
        }

        [Fact]
        public async Task Delete_WillRemoveComputer()
        {
            // Arrange
            Computer computer = (await GetAllAvailableComputers()).Last();
            string url = $"computer/delete/{computer.Id}";
            HttpResponseMessage deletePageResponse = await _client.GetAsync(url);
            IHtmlDocument deletePage = await HtmlHelpers.GetDocumentAsync(deletePageResponse);


            // Act
            HttpResponseMessage response = await _client.SendAsync(deletePage, new Dictionary <string, string> ());

            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);

            Assert.Contains(
                 indexPage.QuerySelectorAll("td"),
                td => !td.TextContent.Contains(computer.PurchaseDate.ToString()));

            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => !td.TextContent.Contains(computer.Make));
            Assert.Contains(
               indexPage.QuerySelectorAll("td"),
               td => !td.TextContent.Contains(computer.Manufacturer));



 }

        private async Task<List<Computer>> GetAllAvailableComputers()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
            {
                string sql = $@"SELECT	c.Id,
		                                c.PurchaseDate,
		                                c.DecomissionDate, 
		                                c.Make, 
		                                c.Manufacturer 
                                FROM Computer c
                                LEFT JOIN ComputerEmployee ce ON c.Id = ce.ComputerId
                                WHERE ce.ComputerId IS NULL
                                AND c.DecomissionDate IS NULL";

                IEnumerable<Computer> computers = await conn.QueryAsync<Computer>(sql);
                return computers.ToList();
            }
        }




    }
    }
