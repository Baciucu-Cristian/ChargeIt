using ChargeIt.Data;
using ChargeIt.Data.DbModels;
using ChargeIt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var chargeMachineViewModels = _applicationDbContext.ChargeMachines.Select(cm => new DropDownViewModel
            {
                Id = cm.Id,
                Value = $"{cm.Code}, {cm.City}, {cm.Street}, {cm.Number}"
            }).ToList();

            var carViewModels = _applicationDbContext.Cars.Include(c => c.Owner).Select(c => new DropDownViewModel
            {
                Id = c.Id,
                Value = $"{c.PlateNumber}, {c.Owner.Name} - {c.Owner.Email}"
            }).ToList();

            var bookingsViewModel = new BookingsViewModel()
            {
                ChargeMachines = chargeMachineViewModels,
                Cars = carViewModels
            };

            return View(bookingsViewModel);
        }

        public IActionResult AddNewBooking(BookingsViewModel bookingsViewModel)
        {
            if (ModelState.IsValid)
            {
                var startTime = bookingsViewModel.Date.Value.AddHours(bookingsViewModel.IntervalHour.Value);
                var endTime = startTime.AddMinutes(59).AddSeconds(59);

                if (_applicationDbContext.Bookings.FirstOrDefault(b => b.ChargeMachineId == bookingsViewModel.ChargeMachineId && b.StartTime == startTime) != null)
                {
                    ModelState.AddModelError(nameof(BookingsViewModel.IntervalHour), "There is an already allocated interval for the selected machine for the selected interval");
                }

                if (_applicationDbContext.Bookings.FirstOrDefault(b => b.CarId == bookingsViewModel.CarId && b.StartTime == startTime) != null)
                {
                    ModelState.AddModelError(nameof(BookingsViewModel.IntervalHour), "This car has been already allocated to the selected interval on a different charge machine");
                }

                if (ModelState.IsValid)
                {
                    var booking = new BookingDbModel()
                    {
                        ChargeMachineId = bookingsViewModel.ChargeMachineId.Value,
                        CarId = bookingsViewModel.CarId.Value,
                        StartTime = startTime,
                        EndTime = endTime,
                        Code = Guid.NewGuid()
                    };

                    _applicationDbContext.Bookings.Add(booking);
                    _applicationDbContext.SaveChanges();
                }
            }

            if (!ModelState.IsValid)
            {
                var chargeMachineViewModels = _applicationDbContext.ChargeMachines.Select(cm => new DropDownViewModel
                {
                    Id = cm.Id,
                    Value = $"{cm.Code}, {cm.City}, {cm.Street}, {cm.Number}"
                }).ToList();

                var carViewModels = _applicationDbContext.Cars.Include(c => c.Owner).Select(c => new DropDownViewModel
                {
                    Id = c.Id,
                    Value = $"{c.PlateNumber}, {c.Owner.Name} - {c.Owner.Email}"
                }).ToList();

                bookingsViewModel.ChargeMachines = chargeMachineViewModels;
                bookingsViewModel.Cars = carViewModels;
                bookingsViewModel.IntervalHour = null;

                return View("Index", bookingsViewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpGet("Bookings/GetAvailableIntervals")]
        public ActionResult<List<int>> GetAvailableIntervals(int chargeMachineId, DateTime date)
        {
            var notAvailableHours = _applicationDbContext.Bookings.Where(b => b.ChargeMachineId == chargeMachineId && b.StartTime >= date && b.StartTime <= date.AddHours(23).AddMinutes(59).AddSeconds(59)).Select(b => b.StartTime.Hour).ToList();

            var availableHours = _totalAvailableHours.Except(notAvailableHours).ToList();

            return availableHours;
        }

        [HttpGet("Bookings/DeleteBooking/{id}/{entityId}/{sourceAction}")]
        public IActionResult DeleteBooking(int id, int entityId, string sourceAction)
        {
            var existingBooking = _applicationDbContext.Bookings.FirstOrDefault(b => b.Id == id);

            if (existingBooking != null && existingBooking.StartTime >= DateTime.Now)
            {
                _applicationDbContext.Bookings.Remove(existingBooking);
                _applicationDbContext.SaveChanges();
            }

            if (sourceAction == "CarDetails")
            {
                return RedirectToAction("CarDetails", "Cars", new { id = entityId});
            }
            else
            {
                return RedirectToAction("StationDetails", "ChargeMachines", new { id = entityId });
            }    
        }
    }
}
