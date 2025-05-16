using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fjordingarnas_Bokningssystem.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool isCancelled { get; set; } = false;


        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = new Customer();

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = new Employee();

        public int ServiceId { get; set; }
        public Service Service { get; set; } = new Service();
    }
}
