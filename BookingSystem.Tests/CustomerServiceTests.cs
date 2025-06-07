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
}
