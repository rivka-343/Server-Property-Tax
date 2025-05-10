using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Models
{
    // מודל להגדרות המערכת
    public class SystemSettings
    {
        public int Id { get; set; }

        [Required]
        public string SettingName { get; set; }

        public DateTime LastUpdated { get; set; }

        public string UpdatedBy { get; set; }
    }
}
