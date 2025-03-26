using PropertyTax.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.DTO
{
    public class RequestMinimalDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
    }
}
