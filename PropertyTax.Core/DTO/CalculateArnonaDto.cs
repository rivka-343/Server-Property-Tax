using PropertyTax.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.DTO
{
    public class CalculateArnonaDto
    {
        public double AverageMonthlyIncome { get; set; }
        public string Status { get; set; }
        public double CalculatedArnona { get; set; }
        public double ApprovedArnona { get; set; }
    }
}
