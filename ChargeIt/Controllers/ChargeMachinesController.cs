using ChargeIt.Data;
using ChargeIt.Data.DbModels;
using ChargeIt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChargeIt.Controllers
{
	public class ChargeMachinesController : Controller
	{
		private readonly ApplicationDbContext _applicationDbContext;

		public ChargeMachinesController(ApplicationDbContext applicationDbContext)
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

			return View(chargeMachineViewModels);
		}

		public IActionResult AddStation()
		{
			var chargeMachineViewModel = new ChargeMachineViewModel();

			return View(chargeMachineViewModel);
		}

		[HttpPost]
		public IActionResult AddNewStation(ChargeMachineViewModel chargeMachineViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View("AddStation", chargeMachineViewModel);
			}

			_applicationDbContext.ChargeMachines.Add(new ChargeMachineDbModel
			{
				City = chargeMachineViewModel.City,
				Code = Guid.NewGuid(),
				Latitude = chargeMachineViewModel.Latitude.Value,
				Longitude = chargeMachineViewModel.Longitude.Value,
				Number = chargeMachineViewModel.Number.Value,
				Street = chargeMachineViewModel.Street
			});

			_applicationDbContext.SaveChanges();

			return RedirectToAction("Index");
		}

		public IActionResult DeleteStation(int id)
		{
			var existingChargeMachine = _applicationDbContext.ChargeMachines.FirstOrDefault(cm => cm.Id == id);

			if (existingChargeMachine != null)
			{
				_applicationDbContext.ChargeMachines.Remove(existingChargeMachine);
				_applicationDbContext.SaveChanges();
			}

			return RedirectToAction("Index");
		}

		public IActionResult EditStation(int id)
		{
			var existingChargeMachine = _applicationDbContext.ChargeMachines.FirstOrDefault(cm => cm.Id == id);

			if (existingChargeMachine == null)
			{
				return RedirectToAction("Index");
			}

			var chargeMachineViewModel = new ChargeMachineViewModel()
			{
				Id = existingChargeMachine.Id,
				City = existingChargeMachine.City,
				Code = existingChargeMachine.Code,
				Latitude = existingChargeMachine.Latitude,
				Longitude = existingChargeMachine.Longitude,
				Number = existingChargeMachine.Number,
				Street = existingChargeMachine.Street
			};

			return View(chargeMachineViewModel);
		}

		[HttpPost]
		public IActionResult EditExistingStation(ChargeMachineViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("EditStation", model);
			}

			var existingStation = _applicationDbContext.ChargeMachines.FirstOrDefault(cm => cm.Id == model.Id);

			existingStation.City = model.City;
			existingStation.Street = model.Street;
			existingStation.Number = model.Number.Value;
			existingStation.Latitude = model.Latitude.Value;
			existingStation.Longitude = model.Longitude.Value;

			_applicationDbContext.SaveChanges();

			return RedirectToAction("Index");
		}

		public IActionResult StationDetails(int id)
		{
			var existingChargeMachine = _applicationDbContext.ChargeMachines.FirstOrDefault(cm => cm.Id == id);

			if (existingChargeMachine == null)
			{
				return RedirectToAction("Index");
			}

			var availableBookings = _applicationDbContext.Bookings
				.Include(b => b.Car)
				.Where(b => b.ChargeMachineId == existingChargeMachine.Id)
				.OrderByDescending(b => b.StartTime)
				.ToList();

			var stationDetailsViewModel = new StationDetailsViewModel() {
				ChargeMachine = new ChargeMachineViewModel()
				{
					Id = existingChargeMachine.Id,
					City = existingChargeMachine.City,
					Code = existingChargeMachine.Code,
					Latitude = existingChargeMachine.Latitude,
					Longitude = existingChargeMachine.Longitude,
					Number = existingChargeMachine.Number,
					Street = existingChargeMachine.Street
				},
				Bookings = availableBookings.Select(ab => new BookingViewModel
                {
					Id = ab.Id,
					Code = ab.Code,
					StartTime = ab.StartTime,
					Car = new CarViewModel
                    {
						Id = ab.Car.Id,
						PlateNumber = ab.Car.PlateNumber
                    }
                }).ToList()
			};

			return View(stationDetailsViewModel);
		}
	}
}
