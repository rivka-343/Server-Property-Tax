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
        public string PropertyNumber { get; set; }
        public double AverageMonthlyIncome { get; set; }
        public List<Doc> Documents { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public double CalculatedArnona { get; set; }
        public double ApprovedArnona { get; set; }
    }
}
