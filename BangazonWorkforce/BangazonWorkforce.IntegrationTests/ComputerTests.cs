using AngleSharp.Dom.Html;
using BangazonWorkforce.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

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
            string url = "/computer/create";
            HttpResponseMessage createPageResponse = await _client.GetAsync(url);
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);

            //object SystemTime = null;
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
            /*
            Assert.Contains(
                createPage.QuerySelectorAll("Input"),
                i => i.Id == "PurchaseDate");
            Assert.Contains(
               createPage.QuerySelectorAll("Input"),
               i => i.Id == "Manufacturer");
            Assert.Contains(
              createPage.QuerySelectorAll("Input"),
              i => i.Id == "Make");
              */
        }
    }
}
