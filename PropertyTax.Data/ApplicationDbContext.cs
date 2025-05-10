using Microsoft.EntityFrameworkCore;
using PropertyTax.Core.Models;
using PropertyTax.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Doc> Documents { get; set; }
        public DbSet<PropertyBaseData> PropertyBaseData { get; set; }
        public DbSet<IncomeDiscountTier> IncomeDiscountTiers { get; set; }
        public DbSet<SocioEconomicPricing> SocioEconomicPricings { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }


    }
}
