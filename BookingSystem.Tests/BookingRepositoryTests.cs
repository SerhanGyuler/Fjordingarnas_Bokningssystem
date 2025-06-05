using Fjordingarnas_Bokningssystem.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingSystem.API.Data;
using BookingSystem.API.Repositories;

namespace BookingSystem.Tests
{
    class BookingRepositoryTests
    {
        //1-6
        [TestMethod]
        public async Task GetAllAsync_ReturnsAllBookingsWithCustomerEmployeeAndServices()
        {
            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1, 
                    Customer = new Customer { Id = 1, FirstName = "Douglas", LastName = "MacArthur", PhoneNumber = "0523523535" },
                    Employee = new Employee { Id = 1, FirstName = "Hamtaro", LastName = "Baker", PhoneNumber = "3535353555" },
                    Services = new List<Service>
                    {
                        new Service { Id = 1, ServiceName = "Gräsklipp", Duration = TimeSpan.FromSeconds(1), Price = 1234}
                    }
                }
            };

            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(c => c.Bookings).ReturnsDbSet(bookings);

            var repo = new BookingRepository(mockContext.Object);

            var result = await repo.GetAllAsync();

            var booking = result.Single();

            Assert.AreEqual("Douglas", booking.Customer.FirstName);
            Assert.AreEqual("Hamtaro", booking.Employee.FirstName);
            Assert.AreEqual("Shave", booking.Services.Single().ServiceName);
        }





        //
    }
}
