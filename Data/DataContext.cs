using BitirmeProjesi.Models;
using Microsoft.EntityFrameworkCore;

namespace BitirmeProjesi.Data {

    public class DataContext : DbContext {
        public DataContext(DbContextOptions<DataContext>options):base(options){}
        
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<BrandModel> BrandModels => Set<BrandModel>();
        public DbSet<ModelDetails> ModelDetails => Set<ModelDetails>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<CarCreate> CarCreates => Set<CarCreate>();
    }
}