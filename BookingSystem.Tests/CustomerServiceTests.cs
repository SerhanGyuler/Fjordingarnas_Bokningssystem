using BookingSystem.API.Repositories;
using BookingSystem.API.Services;
using Fjordingarnas_Bokningssystem.Models;
using Moq;

namespace BookingSystem.Tests;

[TestClass]
public class CustomerServiceTests
{
    private Mock<ICustomerRepository> _customerRepoMock = null!;
    private CustomerService _customerService = null!;

    [TestInitialize]
    public void Setup()
    {
        _customerRepoMock = new Mock<ICustomerRepository>();

        // Inject mock repo into service
        _customerService = new CustomerService(_customerRepoMock.Object);
    }

    [TestMethod]
    public async Task GetAllCustomerDtosAsync_ShouldReturnMappedCustomerDtos()
    {
        // Arrange
        var fakeCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Ada", LastName = "Lovelace", PhoneNumber = "123456" },
            new Customer { Id = 2, FirstName = "Alan", LastName = "Turing", PhoneNumber = "654321" }
        };

        _customerRepoMock
            .Setup(repo => repo.GetAllCustomersAsync())
            .ReturnsAsync(fakeCustomers);

        // Act
        var result = await _customerService.GetAllCustomerDtosAsync();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Ada", result[0].FirstName);
        Assert.AreEqual("Turing", result[1].LastName);
        Assert.AreEqual("654321", result[1].PhoneNumber);
    }

    [TestMethod]
    public async Task GetCustomerByIdAsync_ShouldReturnCustomer_WhenFound()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            FirstName = "Ada",
            LastName = "Lovelace",
            PhoneNumber = "123456"
        };

        _customerRepoMock
            .Setup(repo => repo.GetCustomerByIdAsync(1))
            .ReturnsAsync(customer);

        // Act
        var result = await _customerService.GetCustomerDtoByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result!.Id);
        Assert.AreEqual("Ada", result.FirstName);
    }

    [TestMethod]
    public async Task GetCustomerByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        _customerRepoMock
            .Setup(repo => repo.GetCustomerByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _customerService.GetCustomerDtoByIdAsync(999);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetCustomerByFirstNameAsync_ShouldReturnCustomerDto_WhenFound()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            FirstName = "Ada",
            LastName = "Lovelace",
            PhoneNumber = "123456"
        };

        _customerRepoMock
            .Setup(repo => repo.GetCustomerByFirstNameAsync("Ada"))
            .ReturnsAsync(customer);

        // Act
        var result = await _customerService.GetCustomerByFirstNameAsync("Ada");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Ada", result!.FirstName);
        Assert.AreEqual("Lovelace", result.LastName);
    }

    [TestMethod]
    public async Task GetCustomerByFirstNameAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        _customerRepoMock
            .Setup(repo => repo.GetCustomerByFirstNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _customerService.GetCustomerByFirstNameAsync("NonExistent");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetCustomerByLastNameAsync_ShouldReturnCustomerDto_WhenFound()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 2,
            FirstName = "Alan",
            LastName = "Turing",
            PhoneNumber = "654321"
        };

        _customerRepoMock
            .Setup(repo => repo.GetCustomerByLastNameAsync("Turing"))
            .ReturnsAsync(customer);

        // Act
        var result = await _customerService.GetCustomerByLastNameAsync("Turing");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Alan", result!.FirstName);
        Assert.AreEqual("Turing", result.LastName);
    }

    [TestMethod]
    public async Task GetCustomerByLastNameAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        _customerRepoMock
            .Setup(repo => repo.GetCustomerByLastNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _customerService.GetCustomerByLastNameAsync("NonExistent");

        // Assert
        Assert.IsNull(result);
    }
}
