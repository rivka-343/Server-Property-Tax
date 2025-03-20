using Microsoft.AspNetCore.Http;

namespace PropertyTax.DTO
{
    public class RequestCreateDto
    {
        public string PropertyNumber { get; set; }
        public double AverageMonthlyIncome { get; set; }
        public List<DocDto> DocumentUploads { get; set; }
    }
}
