using BookingSystem.API.Models.DTOs;
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

    [TestMethod]
    public async Task GetCustomerByPhoneNumberAsync_ShouldReturnCustomerDto_WhenCustomerIsFound()
    {
        // Arrange
        string phoneNumber = "123123";
        var customer = new Customer
        {
            Id = 1,
            FirstName = "Kylian",
            LastName = "Mbappe",
            PhoneNumber = phoneNumber
        };

        _customerRepoMock
            .Setup(r => r.GetCustomerByPhoneNumberAsync(phoneNumber))
            .ReturnsAsync(customer);

        // Act
        var result = await _customerService.GetCustomerByPhoneNumberAsync(phoneNumber);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(customer.Id, result.Id);
        Assert.AreEqual(customer.FirstName, result.FirstName);
        Assert.AreEqual(customer.LastName, result.LastName);
        Assert.AreEqual(customer.PhoneNumber, result.PhoneNumber);

        _customerRepoMock.Verify(r => r.GetCustomerByPhoneNumberAsync(phoneNumber), Times.Once);
    }

    [TestMethod]
    public async Task GetCustomerByPhoneNumberAsync_ShouldReturnNull_WhenCustomerIsNotFound()
    {
        // Arrange
        string phoneNumber = "98765";

        _customerRepoMock
            .Setup(r => r.GetCustomerByPhoneNumberAsync(phoneNumber))
            .ReturnsAsync((Customer)null!);

        // Act
        var result = await _customerService.GetCustomerByPhoneNumberAsync(phoneNumber);

        // Assert
        Assert.IsNull(result);

        _customerRepoMock.Verify(r => r.GetCustomerByPhoneNumberAsync(phoneNumber), Times.Once);
    }

    [TestMethod]
    public async Task CreateCustomerAsync_ShouldCreateCustomer_ReturnSuccessfulMessage()
    {
        // Arrange
        int id = 1;
        string firstName = "Cristiano";
        string lastName = "Ronaldo";
        string phoneNumber = "77777";

        // Act
        var result = await _customerService.CreateCustomerAsync(id, firstName, lastName, phoneNumber);

        // Assert
        Assert.IsNotNull(result);
        StringAssert.Contains(result, $"Customer {firstName} {lastName} was created.");

        // Verifying repository calls, correct data sent
        _customerRepoMock.Verify(r => r.AddCustomerAsync(It.Is<Customer>(
            c => c.Id == id &&
                 c.FirstName == firstName &&
                 c.LastName == lastName &&
                 c.PhoneNumber == phoneNumber)), Times.Once);

        _customerRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task UpdateCustomerAsync_ShouldReturnNull_WhenCustomerNotFound()
    {
        // Arrange
        _customerRepoMock
            .Setup(r => r.GetCustomerByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Customer)null!);

        var updatedCustomerDto = new CustomerDto
        {
            FirstName = "Test",
            LastName = "Name",
            PhoneNumber = "00000"
        };

        // Act
        var result = await _customerService.UpdateCustomerAsync(1, updatedCustomerDto);

        // Assert
        Assert.IsNull(result);
        _customerRepoMock.Verify(repo => repo.UpdateCustomerAsync(It.IsAny<Customer>()), Times.Never);
        _customerRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task UpdateCustomerAsync_ShouldReturnUpdatedDto_WhenCustomerIsFound()
    {
        // Arrange
        var existingCustomer = new Customer
        {
            Id = 1,
            FirstName = "OldFirstname",
            LastName = "OldLastname",
            PhoneNumber = "1234567"
        };

        var updatedCustomerDto = new CustomerDto
        {
            FirstName = "NewFirstname",
            LastName = "NewLastname",
            PhoneNumber = "7654321"
        };

        _customerRepoMock
            .Setup(r => r.GetCustomerByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(existingCustomer);

        // Act
        var result = await _customerService.UpdateCustomerAsync(1, updatedCustomerDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(updatedCustomerDto.FirstName, result.FirstName);
        Assert.AreEqual(updatedCustomerDto.LastName, result.LastName);
        Assert.AreEqual(updatedCustomerDto.PhoneNumber, result.PhoneNumber);

        _customerRepoMock.Verify(repo => repo.UpdateCustomerAsync(existingCustomer), Times.Once);
        _customerRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task DeleteCustomerAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        _customerRepoMock
            .Setup(repo => repo.GetCustomerByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Customer)null!);

        // Act
        var result = await _customerService.DeleteCustomerAsync(1);

        // Assert
        Assert.IsNull(result);
        // Make sure to NOT save when customer is not found
        _customerRepoMock.Verify(repo => repo.DeleteCustomerAsync(It.IsAny<Customer>()), Times.Never);
        _customerRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [TestMethod]
    public async Task DeleteCustomerAsync_DeletesCustomer_WhenCustomerIsFound()
    {
        // Arrange 
        var customer = new Customer
        {
            Id = 1,
            FirstName = "Test",
            LastName = "Customer"
        };

        _customerRepoMock
            .Setup(repo => repo.GetCustomerByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _customerService.DeleteCustomerAsync(1);

        // Assert
        Assert.IsNotNull(result);
        StringAssert.Contains(result, "Customer with ID 1 has been deleted");

        _customerRepoMock.Verify(repo => repo.DeleteCustomerAsync(customer), Times.Once);
        _customerRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }
}
