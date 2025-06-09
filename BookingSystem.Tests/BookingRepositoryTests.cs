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
using Microsoft.Data.Sqlite;
using System.Data.Common;


namespace BookingSystem.Tests
{

    public class TestContextFactory : IDisposable
    {
        private DbConnection _connection;

        private DbContextOptions<AppDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection).Options;
        }

        public AppDbContext CreateContext()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                using var tempContext = new AppDbContext(CreateOptions());
                tempContext.Database.EnsureCreated();
            }

            return new TestAppDbContext(CreateOptions());
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _connection = null;
        }
    }

    public class TestAppDbContext : AppDbContext
    {
        public TestAppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }

    [TestClass]
    public class BookingRepositoryIntegrationTests
    {
        private TestContextFactory _factory;

        [TestInitialize]
        public void Init() => _factory = new TestContextFactory();

        [TestCleanup]
        public void Cleanup() => _factory.Dispose();

        [TestMethod]
        public async Task GetBookingsForEmployeeAsync_ReturnsCorrectBookings()
        {
            using (var context = _factory.CreateContext())
            {
                context.Bookings.RemoveRange(context.Bookings);
                await context.SaveChangesAsync();

                var testBookings = new List<Booking> {
                        new Booking { EmployeeId = 1, CustomerId = 1, IsCancelled = false,
                        StartTime = new DateTime(2024, 1, 1, 10, 0, 0), EndTime = new DateTime(2024, 1, 1, 11, 0, 0) },
                        new Booking { EmployeeId = 1, CustomerId = 2, IsCancelled = false,
                        StartTime = new DateTime(2024, 1, 1, 12, 0, 0), EndTime = new DateTime(2024, 1, 1, 13, 0, 0) },
                        new Booking { EmployeeId = 2, CustomerId = 3, IsCancelled = false,
                        StartTime = new DateTime(2024, 1, 1, 14, 0, 0), EndTime = new DateTime(2024, 1, 1, 15, 0, 0) },
                        new Booking { EmployeeId = 1, CustomerId = 4, IsCancelled = true,
                        StartTime = new DateTime(2024, 1, 1, 16, 0, 0), EndTime = new DateTime(2024, 1, 1, 17, 0, 0) }
                    };

                context.AddRange(testBookings);

                await context.SaveChangesAsync();
            };

            using (var context = _factory.CreateContext())
            {
                var repo = new BookingRepository(context);
                //Act
                var result = await repo.GetBookingsForEmployeeAsync(1, null, null);

                Assert.AreEqual(2, result.Count);
                Assert.IsTrue(result.All(b => b.EmployeeId == 1));
                Assert.IsTrue(result.All(b => !b.IsCancelled));
            }

        }

        [DataTestMethod]
        [DataRow(0)]  // 0 Bookings
        [DataRow(1)]  
        [DataRow(2)]  // 2 Bookings
        public async Task GetAllAsync_ReturnsExpectedBookingCount_AndIncludesRelatedData(int bookingCount)
        {
            using (var context = _factory.CreateContext())
            {
                context.Bookings.RemoveRange(context.Bookings);
                await context.SaveChangesAsync();

                if (bookingCount > 0)
                {
                    var customer = new Customer { FirstName = "Alice", LastName = "Chalice", PhoneNumber = "123213" };
                    var employee = new Employee { FirstName = "Ferdinand", LastName = "Berdinand", PhoneNumber = "34343" };
                    var services = new List<Service> {
                new Service { ServiceName = "Tvätt", Duration = TimeSpan.FromMinutes(15), Price = 123 },
                new Service { ServiceName = "Klipp", Duration = TimeSpan.FromMinutes(30), Price = 143 },
                new Service { ServiceName = "Hårtransplantation", Duration = TimeSpan.FromMinutes(180), Price = 80 },
            };

                    for (int i = 0; i < bookingCount; i++)
                    {
                        var booking = new Booking
                        {
                            Customer = customer,
                            Employee = employee,
                            Services = services,
                            StartTime = DateTime.UtcNow.AddMinutes(i * 15),
                            EndTime = DateTime.UtcNow.AddMinutes(i * 15 + 15),
                            IsCancelled = false
                        };

                        context.Bookings.Add(booking);
                    }

                    await context.SaveChangesAsync();
                }
            }

            using (var context = _factory.CreateContext())
            {
                var repo = new BookingRepository(context);
                var result = (await repo.GetAllAsync()).ToList();

                Assert.AreEqual(bookingCount, result.Count);

                if (bookingCount > 0)
                {
                    var first = result[0];
                    Assert.AreEqual("Alice", first.Customer.FirstName);
                    Assert.AreEqual("Ferdinand", first.Employee.FirstName);
                    Assert.AreEqual("Klipp", first.Services.OrderBy(s => s.ServiceName).ToList()[1].ServiceName);
                }
            }
        }


        [TestMethod]
        public async Task GetAllAsync_ReturnsEmptyList_IfNoBookings()
        {
            using var context = _factory.CreateContext();
            context.Bookings.RemoveRange(context.Bookings);
            await context.SaveChangesAsync();

            var repo = new BookingRepository(context);
            var result = (await repo.GetAllAsync()).ToList();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsCustomerEmployeeServices()
        {
            int bookingId;

            using (var context = _factory.CreateContext())
            {
                context.Bookings.RemoveRange(context.Bookings);
                await context.SaveChangesAsync();

                var customer = new Customer { FirstName = "Alice", LastName = "Chalice", PhoneNumber = "123213" };
                var employee = new Employee { FirstName = "Ferdinand", LastName = "Berdinand", PhoneNumber = "34343" };
                var services = new List<Service>
                {
                    new Service { ServiceName = "Tvätt", Duration = TimeSpan.FromMinutes(15), Price = 123 },
                    new Service { ServiceName = "Klipp", Duration = TimeSpan.FromMinutes(30), Price = 143 },
                    new Service { ServiceName = "Hårtransplantation", Duration = TimeSpan.FromMinutes(180), Price = 80 },
                };

                var booking = new Booking
                {
                    Customer = customer,
                    Employee = employee,
                    Services = services,
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddMinutes(15),
                    IsCancelled = false
                };

                context.Bookings.Add(booking);
                await context.SaveChangesAsync();

                bookingId = booking.Id;
            }

            using (var context = _factory.CreateContext())
            {
                var repo = new BookingRepository(context);
                //Act
                var result = await repo.GetByIdAsync(bookingId);

                Assert.IsNotNull(result);
                Assert.AreEqual("Alice", result.Customer.FirstName);
                Assert.AreEqual("Ferdinand", result.Employee.FirstName);
                Assert.IsTrue(3 ==  result.Services.Count);
                Assert.IsTrue(result.Services.Any(s => s.ServiceName == "Hårtransplantation"));
                Assert.IsTrue(result.Services.Any(s => s.ServiceName == "Klipp"));
            }
        }

        //[TestMethod]
        //public async Task AddAsync_
    }
}

