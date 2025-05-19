using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fjordingarnas_Bokningssystem.Models
{
    public class Service
    {
        public int Id { get; set; }
        [Required]
        public string? ServiceName { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal Price { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
