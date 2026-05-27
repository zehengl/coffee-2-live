namespace Coffee2Live.Tests;

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Coffee2Live.App.Controllers;
using Coffee2Live.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using NUnit.Framework;

public class CoffeesControllerTests
{
    private string _tempRoot = null!;

    [SetUp]
    public void SetUp()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), "Coffee2Live.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_tempRoot, "Data"));
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }

    [Test]
    public void GetAll_ReturnsOkResult_WithOriginAndEnumMapping()
    {
        var source = new[]
        {
            new
            {
                name = "Ethiopian Yirgacheffe",
                origin = "Yirgacheffe region, Ethiopia",
                tastingNotes = "Bright citrus and jasmine",
                bitterness = 3,
                acidity = "High",
                body = 2,
                roast = "Light",
                bestFor = "Pour over"
            }
        };

        var dataFile = Path.Combine(_tempRoot, "Data", "coffees.json");
        File.WriteAllText(dataFile, JsonSerializer.Serialize(source, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

        var controller = new CoffeesController(new TestWebHostEnvironment(_tempRoot));
        var result = controller.GetAll();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Which;
        var coffeeList = okResult.Value.Should().BeAssignableTo<IEnumerable<Coffee>>().Which.ToArray();

        coffeeList.Should().ContainSingle();
        coffeeList[0].Name.Should().Be("Ethiopian Yirgacheffe");
        coffeeList[0].Origin.Should().Be("Yirgacheffe region, Ethiopia");
        coffeeList[0].Acidity.Should().Be(Acidity.High);
        coffeeList[0].Roast.Should().Be(Roast.Light);
        coffeeList[0].BestFor.Should().Be("Pour over");
    }

    [Test]
    public void GetById_ReturnsNotFound_WhenCoffeeDoesNotExist()
    {
        var controller = new CoffeesController(new TestWebHostEnvironment(_tempRoot));

        var result = controller.GetById(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public TestWebHostEnvironment(string contentRootPath)
        {
            ContentRootPath = contentRootPath;
            ContentRootFileProvider = new PhysicalFileProvider(contentRootPath);
            WebRootPath = string.Empty;
            WebRootFileProvider = new NullFileProvider();
            EnvironmentName = "Development";
            ApplicationName = "Coffee2Live.Tests";
        }

        public string ApplicationName { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
    }
}
