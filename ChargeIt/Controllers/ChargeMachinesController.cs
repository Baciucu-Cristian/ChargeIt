using ChargeIt.Data;
using ChargeIt.Data.DbModels;
using ChargeIt.Models;
using Microsoft.AspNetCore.Mvc;

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
				Latitude = chargeMachineViewModel.Latitude,
				Longitude = chargeMachineViewModel.Longitude,
				Number = chargeMachineViewModel.Number,
				Street = chargeMachineViewModel.Street
			});

			_applicationDbContext.SaveChanges();

			return RedirectToAction("Index");
		}
	}
}
