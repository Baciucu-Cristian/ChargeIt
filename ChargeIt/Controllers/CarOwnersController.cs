using ChargeIt.Data;
using ChargeIt.Data.DbModels;
using ChargeIt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChargeIt.Controllers
{
    [Authorize]
    public class CarOwnersController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CarOwnersController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            var carOwnerViewModels = _applicationDbContext.CarOwners.Select(co => new CarOwnerViewModel
            {
                Id = co.Id,
                Name = co.Name,
                Email = co.Email,
            }).ToList();

            return View(carOwnerViewModels);
        }

        public IActionResult AddCarOwner()
        {
            var carOwnerViewModel = new CarOwnerViewModel();

            return View(carOwnerViewModel);
        }

        [HttpPost]
        public IActionResult AddNewCarOwner(CarOwnerViewModel carOwnerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("AddCarOwner", carOwnerViewModel);
            }

            var existingCarOwner = _applicationDbContext.CarOwners.FirstOrDefault(co => co.Email == carOwnerViewModel.Email);

            if (existingCarOwner != null)
            {
                ModelState.AddModelError("Email", "There is an already existing car owner with the same email");
                return View("AddCarOwner", carOwnerViewModel);
            }

            _applicationDbContext.CarOwners.Add(new CarOwnerDbModel
            {
                Name = carOwnerViewModel.Name,
                Email = carOwnerViewModel.Email
            });

            _applicationDbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult EditCarOwner(int id)
        {
            var existingCarOwner = _applicationDbContext.CarOwners.FirstOrDefault(co => co.Id == id);

            if (existingCarOwner == null)
            {
                return RedirectToAction("Index");
            }

            var carOwnerViewModel = new CarOwnerViewModel
            {
                Id = existingCarOwner.Id,
                Name = existingCarOwner.Name,
                Email = existingCarOwner.Email,
            };

            return View(carOwnerViewModel);
        }

        [HttpPost]
        public IActionResult EditExistingCarOwner(CarOwnerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditCarOwner", model);
            }

            var carOwnerAvailable = _applicationDbContext.CarOwners.Any(co => co.Email == model.Email &&
                                                                                        co.Id != model.Id);

            if (carOwnerAvailable)
            {
                ModelState.AddModelError("Email", "There is an already existing car owner with the same email");
                return View("EditCarOwner", model);
            }

            var existingCarOwner = _applicationDbContext.CarOwners.FirstOrDefault(co => co.Id == model.Id);

            if (existingCarOwner != null)
            {
                existingCarOwner.Name = model.Name;
                existingCarOwner.Email = model.Email;

                _applicationDbContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult DeleteCarOwner(int id)
        {
            var existingCarOwner = _applicationDbContext.CarOwners.FirstOrDefault(co => co.Id == id);

            if (existingCarOwner != null)
            {
                _applicationDbContext.CarOwners.Remove(existingCarOwner);
                _applicationDbContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult CarOwnerDetails(int id)
        {
            var existingCarOwner = _applicationDbContext.CarOwners.Include(co => co.Cars).FirstOrDefault(co => co.Id == id);

            if (existingCarOwner == null)
            {
                return RedirectToAction("Index");
            }

            var carOwnerViewModel = new CarOwnerDetailsViewModel
            {
                Id = existingCarOwner.Id,
                Name = existingCarOwner.Name,
                Email = existingCarOwner.Email,
                Cars = existingCarOwner.Cars.Select(c => new CarViewModel
                {
                    PlateNumber = c.PlateNumber
                }).ToList()
            };

            return View(carOwnerViewModel);
        }
    }
}
