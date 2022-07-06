using ChargeIt.Data;
using ChargeIt.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChargeIt.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public BookingsController (ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
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
    }
}
