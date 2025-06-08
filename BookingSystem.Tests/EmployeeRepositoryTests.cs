using BookingSystem.API.Data;
using BookingSystem.API.Repositories;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Tests;

[TestClass]
public class EmployeeRepositoryTests
{
    private AppDbContext _context = null!;
    private EmployeeRepository _employeeRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Employee")
            .Options;

        _context = new AppDbContext(options);
        _employeeRepository = new EmployeeRepository(_context);
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsEmployee_WhenEmployeeExists()
    {
        // Arrange
        var employee = new Employee { Id = 1, FirstName = "Gazi", LastName = "Kingen", PhoneNumber = "112" };
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _employeeRepository.GetByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(employee.Id, result.Id);
        Assert.AreEqual(employee.FirstName, result.FirstName);
        Assert.AreEqual(employee.LastName, result.LastName);
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsNull_WhenEmployeeDoesNotExist()
    {
        // Act
        var result = await _employeeRepository.GetByIdAsync(199);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetByIdWithServicesAsync_ReturnsEmployeeWithServices_WhenEmployeeExists()
    {
        // Arrange
        var service1 = new Service { Id = 1, ServiceName = "Ögonbryn-Fade" };
        var service2 = new Service { Id = 2, ServiceName = "Hårarm-Fade" };
        var employee = new Employee
        {
            Id = 1,
            FirstName = "Test",
            LastName = "Test",
            PhoneNumber = "0909",
            Services = new List<Service> { service1, service2 }
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _employeeRepository.GetByIdWithServicesAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(employee.Id, result.Id);
        Assert.AreEqual(2, result.Services.Count);
        Assert.IsTrue(result.Services.Any(s => s.ServiceName == "Ögonbryn-Fade"));
        Assert.IsTrue(result.Services.Any(s => s.ServiceName == "Hårarm-Fade"));
    }

    [TestMethod]
    public async Task GetByIdWithServicesAsync_ReturnsNull_WhenEmployeeDoesNotExist()
    {
        // Act
        var result = await _employeeRepository.GetByIdWithServicesAsync(99);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllEmployeesWithServices()
    {
        // Arrange
        var service1 = new Service { Id = 1, ServiceName = "Klippning" };
        var service2 = new Service { Id = 2, ServiceName = "Hårfärgning" };

        var employee1 = new Employee
        {
            Id = 1,
            FirstName = "Cristiano",
            LastName = "Ronaldo",
            PhoneNumber = "7",
            Services = new List<Service> { service1 }
        };

        var employee2 = new Employee
        {
            Id = 2,
            FirstName = "Mike",
            LastName = "Tyson",
            PhoneNumber = "4444",
            Services = new List<Service> { service2 }
        };

        _context.Employees.AddRange(employee1, employee2);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _employeeRepository.GetAllAsync()).ToList();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);

        var first = result.FirstOrDefault(e => e.Id == 1);
        Assert.IsNotNull(first);
        Assert.AreEqual("Cristiano", first.FirstName);
        Assert.AreEqual(1, first.Services.Count);
        Assert.AreEqual("Klippning", first.Services.First().ServiceName);

        var second = result.FirstOrDefault(e => e.Id == 2);
        Assert.IsNotNull(second);
        Assert.AreEqual("Mike", second.FirstName);
        Assert.AreEqual(1, second.Services.Count);
        Assert.AreEqual("Hårfärgning", second.Services.First().ServiceName);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoEmployeesExist()
    {
        // Act
        var result = await _employeeRepository.GetAllAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }
}
