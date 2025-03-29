using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Models
{

    public enum DocumentType
    {
        IDWithSpousePage,   // צילום ת"ז כולל ספח
        BankStatement,      // דוח בנק
        PayslipSpouse1,     // תלושי משכורת של בן/בת זוג 1
        PayslipSpouse2      // תלושי משכורת של בן/בת זוג 2
    }
    public class Doc
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public int RequestId { get; set; }
        public Request Request { get; set; }
        public string S3Url { get; set; }
        public DocumentType Type { get; set; }
    }
}
