using BitirmeProjesi.Models;

namespace BitirmeProjesi.ViewModel
{
    public class CreateViewModel
    {
        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<CarCreate>? CarList { get; set; }
        public IEnumerable<Brand>? Brands { get; set; }
        public IEnumerable<BrandModel>? BrandModels { get; set; }
    }
}