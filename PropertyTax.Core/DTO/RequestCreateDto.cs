using Microsoft.AspNetCore.Http;

namespace PropertyTax.DTO
{
    public class RequestCreateDto
    {
        public string FName { get; set; }
        public string LFName { get; set; }
        public string Gmail { get; set; }
        public string HomeNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PropertyNumber { get; set; }
        public double AverageMonthlyIncome { get; set; }
        public List<DocDto> DocumentUploads { get; set; }
    }
}
