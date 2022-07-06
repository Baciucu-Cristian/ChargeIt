using ChargeIt.Data;
using ChargeIt.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChargeIt.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly List<int> _totalAvailableHours;
        private const int TotalAvailableHoursInADay = 24;

        public BookingsController (ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;

            _totalAvailableHours = new List<int>();

            for (var hour = 0; hour < TotalAvailableHoursInADay; hour++)
            {
                _totalAvailableHours.Add(hour);
            }
        }

        public IActionResult Index()
        {
            var chargeMachineViewModels = _applicationDbContext.ChargeMachines.Select(cm => new ChargeMachineViewModel
            {
                Id = cm.Id,
                City = cm.City,
                Code = cm.Code,
                Latitude = cm.Latitude,
                Longitude = cm.Longitude,
                Number = cm.Number,
                Street = cm.Street
            }).ToList();

            var carViewModels = _applicationDbContext.Cars.Select(c => new CarViewModel
            {
                Id = c.Id,
                PlateNumber = c.PlateNumber
            }).ToList();

            var bookingsViewModel = new BookingsViewModel()
            {
                ChargeMachines = chargeMachineViewModels,
                Cars = carViewModels
            };

            return View(bookingsViewModel);
        }

        public IActionResult AddNewBooking()
        {
            return null;
        }

        [HttpGet("Bookings/GetAvailableIntervals")]
        public ActionResult<List<int>> GetAvailableIntervals(int chargeMachineId, DateTime date)
        {
            var notAvailableHours = _applicationDbContext.Bookings.Where(b => b.ChargeMachineId == chargeMachineId && b.StartTime >= date && b.StartTime <= date.AddHours(23).AddMinutes(59).AddSeconds(59)).Select(b => b.StartTime.Hour).ToList();

            var availableHours = _totalAvailableHours.Except(notAvailableHours).ToList();

            return availableHours;
        }
    }
}
