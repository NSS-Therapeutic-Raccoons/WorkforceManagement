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

namespace BangazonWorkforce.IntegrationTests
{
    
    /*
        * Author: Thereputic Raccoons
        * Purpose: This class houses all of the tests assocaited with the varous Views for Training Programs.
    */
    public class TrainingProgramTests : IClassFixture<WebApplicationFactory<BangazonWorkforce.Startup>>
    {
        private readonly HttpClient _client;

        public TrainingProgramTests(WebApplicationFactory<BangazonWorkforce.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_IndexReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            string url = "/trainingprogram";

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        /*
            * Author: Ricky Bruner
            * Purpose: This test gets all applicable training programs and tests to see if all of their parameters are represented on the DOM
        */
        [Fact]
        public async Task Get_TrainingProgramIndexContentVerified()
        {
            // Arrange
            string url = "/trainingprogram";
            List<TrainingProgram> upcomingTrainingPrograms = await GetUpcomingTrainingPrograms();

            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);

            foreach (TrainingProgram tp in upcomingTrainingPrograms) 
            { 
                Assert.Contains(
                    indexPage.QuerySelectorAll("td"),
                    td => td.TextContent.Contains(tp.Name));
                Assert.Contains(
                    indexPage.QuerySelectorAll("td"),
                    td => td.TextContent.Contains(tp.EndDate.ToString()));
                Assert.Contains(
                    indexPage.QuerySelectorAll("td"),
                    td => td.TextContent.Contains(tp.StartDate.ToString()));
                Assert.Contains(
                    indexPage.QuerySelectorAll("td"),
                    td => td.TextContent.Contains(tp.MaxAttendees.ToString()));
            }

        }

        /*
            * Author: Ricky Bruner
            * Purpose: Create a new training program with a date into the future, and then verify that it was added on the index page. Also, verify the inputs exist on the create page. 
        */
        [Fact]
        public async Task Post_CreateAddsTrainingProgram()
        {
            // Arrange
            string url = "/trainingprogram/create";
            HttpResponseMessage createPageResponse = await _client.GetAsync(url);
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);

            string newName = "Name-" + Guid.NewGuid().ToString();
            string newStartDate = new DateTime(DateTime.Now.Year, (DateTime.Now.Month + 1), DateTime.Now.Day).ToString();
            string newEndDate = new DateTime(DateTime.Now.Year, (DateTime.Now.Month + 1), (DateTime.Now.Day + 1)).ToString();
            string newMaxAttendees = new Random().Next(1, 30).ToString();


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                createPage,
                new Dictionary<string, string>
                {
                    {"TrainingProgram_Name", newName},
                    {"TrainingProgram_StartDate", newStartDate},
                    {"TrainingProgram_EndDate", newEndDate},
                    {"TrainingProgram_MaxAttendees", newMaxAttendees}
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            

            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newName));
            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newStartDate));
            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newEndDate));
            Assert.Contains(
                indexPage.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newMaxAttendees));
            Assert.Contains(
                createPage.QuerySelectorAll("Input"),
                i => i.Id == "TrainingProgram_Name");
            Assert.Contains(
               createPage.QuerySelectorAll("Input"),
               i => i.Id == "TrainingProgram_StartDate");
            Assert.Contains(
                createPage.QuerySelectorAll("Input"),
                i => i.Id == "TrainingProgram_EndDate");
            Assert.Contains(
               createPage.QuerySelectorAll("Input"),
               i => i.Id == "TrainingProgram_MaxAttendees");
        }

        /*
            * Author: Ricky Bruner
            * Purpose: Get only Training Programs that have a start date after the current date.
        */
        private async Task<List<TrainingProgram>> GetUpcomingTrainingPrograms()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
            {
                IEnumerable<TrainingProgram> trainingPrograms =
                    await conn.QueryAsync<TrainingProgram>($@"
                    SELECT
                        Id,
                        [Name],
                        StartDate,
                        EndDate,
                        MaxAttendees
                    FROM TrainingProgram
                    WHERE StartDate > '{DateTime.Now}'
                    ORDER BY Id");
                return trainingPrograms.ToList();
            }
        }

    }
}
