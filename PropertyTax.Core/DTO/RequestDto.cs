using Microsoft.AspNetCore.Http;

namespace PropertyTax.DTO
{
    public class RequestDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PropertyNumber { get; set; }
        public double AverageMonthlyIncome { get; set; }
        public List<DocDto> Documents { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public double CalculatedArnona { get; set; }
        public double ApprovedArnona { get; set; }
    }
}
