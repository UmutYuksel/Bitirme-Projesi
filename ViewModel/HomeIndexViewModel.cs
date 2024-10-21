using BitirmeProjesi.Models;

namespace BitirmeProjesi.ViewModel
{
    public class HomeIndexViewModel
    {
        public IEnumerable<CategoryWithVehicleCountViewModel> Categories { get; set; }
        public IEnumerable<CarCreate>? CarList { get; set; }
    }
}