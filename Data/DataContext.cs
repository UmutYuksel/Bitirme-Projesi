using BitirmeProjesi.Models;
using Microsoft.EntityFrameworkCore;

namespace BitirmeProjesi.Data
{


    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {}
        public DbSet<CarCreate> Cars { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<BrandModel> BrandModels { get; set; }
        public DbSet<Category> Categories { get; set; }
    }

}