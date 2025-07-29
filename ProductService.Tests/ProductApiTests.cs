using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace ProductService.Tests
{
    public class ProductApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProductApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllProducts_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/products");

            // Assert
            response.EnsureSuccessStatusCode(); // Проверяет, что статус 200-299
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }
    }
}
