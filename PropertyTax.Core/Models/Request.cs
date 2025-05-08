using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Models
{
    public class Request
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FName { get; set; }
        public string LFName { get; set; }
        public string Gmail { get; set; }
        public string HomeNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public int PropertyNumber { get; set; }
        public double AverageMonthlyIncome { get; set; }
        public List<Doc> Documents { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public double CalculatedArnona { get; set; }
        public double ApprovedArnona { get; set; }
    }
}
