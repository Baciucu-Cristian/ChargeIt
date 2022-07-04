using ChargeIt.Data;
using ChargeIt.Data.DbModels;
using ChargeIt.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChargeIt.Controllers
{
	public class CarsController : Controller
	{
		private readonly ApplicationDbContext _applicationDbContext;

		public CarsController(ApplicationDbContext applicationDbContext)
		{
			_applicationDbContext = applicationDbContext;
		}

		public IActionResult Index()
		{
			 var carViewModels = _applicationDbContext.Cars.Select(c => new CarViewModel
			{
				Id = c.Id,
				PlateNumber = c.PlateNumber
			}).ToList();

			return View(carViewModels);
		}

		public IActionResult AddCar()
		{
			var carViewModel = new CarViewModel();

			return View(carViewModel);
		}

		[HttpPost]
		public IActionResult AddNewCar(CarViewModel carViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View("AddCar", carViewModel);
			}

			var existingCar = _applicationDbContext.Cars.FirstOrDefault(c => c.PlateNumber == carViewModel.PlateNumber);

			if (existingCar != null)
			{
				ModelState.AddModelError("PlateNumber", "There is an already existing car with the same plate number");
				return View("AddCar", carViewModel);
			}

			_applicationDbContext.Cars.Add(new CarDbModel
			{
				PlateNumber = carViewModel.PlateNumber,
			});

			_applicationDbContext.SaveChanges();

			return RedirectToAction("Index");
		}
	}
}
