using Fjordingarnas_Bokningssystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Data
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Service> Services { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public AppDbContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Booking => Customer (Many-to-One)    
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId);

            // Booking => Employee (Many-to-One)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Employee)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EmployeeId);

            // Booking => Service (Many-to-Many)
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Services)
                .WithMany(s => s.Bookings)
                .UsingEntity(j => j.ToTable("BookingServices"));

            // Employee => Service (Many-to-Many)
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Services)
                .WithMany(s => s.Employees)
                .UsingEntity(j => j.ToTable("EmployeeServices"));

            // Seed data
            Seed(modelBuilder);
        }

        private static void Seed(ModelBuilder modelBuilder)
        {
            // Customers
            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, FirstName = "Anna", LastName = "Svensson", PhoneNumber = "0701111111" },
                new Customer { Id = 2, FirstName = "Björn", LastName = "Nilsson", PhoneNumber = "0702222222" },
                new Customer { Id = 3, FirstName = "Carla", LastName = "Lind", PhoneNumber = "0703333333" },
                new Customer { Id = 4, FirstName = "David", LastName = "Ek", PhoneNumber = "0704444444" }
            );

            // Employees
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FirstName = "Elin", LastName = "Frisör", PhoneNumber = "0761234567" },
                new Employee { Id = 2, FirstName = "Farid", LastName = "Barber", PhoneNumber = "0762345678" },
                new Employee { Id = 3, FirstName = "Greta", LastName = "Hud", PhoneNumber = "0763456789" }
            );

            // Services
            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, ServiceName = "Hårklippning", Duration = TimeSpan.FromMinutes(45), Price = 350 },
                new Service { Id = 2, ServiceName = "Skäggklippning", Duration = TimeSpan.FromMinutes(30), Price = 250 },
                new Service { Id = 3, ServiceName = "Färgning", Duration = TimeSpan.FromMinutes(90), Price = 800 },
                new Service { Id = 4, ServiceName = "Massage", Duration = TimeSpan.FromMinutes(60), Price = 600 },
                new Service { Id = 5, ServiceName = "Ansiktsbehandling", Duration = TimeSpan.FromMinutes(40), Price = 500 }
            );

            // Bookings
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    Id = 1,
                    CustomerId = 1,
                    EmployeeId = 1,
                    StartTime = DateTime.Today.AddDays(1).AddHours(10).ToUniversalTime(),
                    EndTime = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(45).ToUniversalTime(),
                    IsCancelled = false
                },
                new Booking
                {
                    Id = 2,
                    CustomerId = 2,
                    EmployeeId = 2,
                    StartTime = DateTime.Today.AddDays(2).AddHours(13).ToUniversalTime(),
                    EndTime = DateTime.Today.AddDays(2).AddHours(13).AddMinutes(30).ToUniversalTime(),
                    IsCancelled = false
                }
            );

            // Booking - Service
            modelBuilder.Entity("BookingService").HasData(
                new { BookingsId = 1, ServicesId = 1 }, // Bokning 1: Hårklippning
                new { BookingsId = 2, ServicesId = 2 }  // Bokning 2: Skäggklippning
            );

            // Employee - Service
            modelBuilder.Entity("EmployeeService").HasData(
                new { EmployeesId = 1, ServicesId = 1 }, // Elin: Hårklippning
                new { EmployeesId = 1, ServicesId = 3 }, // Elin: Färgning
                new { EmployeesId = 2, ServicesId = 2 }, // Farid: Skäggklippning
                new { EmployeesId = 3, ServicesId = 4 }, // Greta: Massage
                new { EmployeesId = 3, ServicesId = 5 }  // Greta: Ansiktsbehandling
            );
        }
    }
}
