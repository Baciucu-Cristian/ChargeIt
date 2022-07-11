using ChargeIt.Data;
using ChargeIt.Data.DbModels;
using ChargeIt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChargeIt.Controllers
{
    [Authorize]
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CarsController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            var carViewModels = _applicationDbContext.Cars.Include(c => c.Owner).Select(c => new CarViewModel
            {
                Id = c.Id,
                PlateNumber = c.PlateNumber,
                CarOwner = $"{c.Owner.Name} {c.Owner.Email}"
            }).ToList();

            return View(carViewModels);
        }

        public IActionResult AddCar()
        {
            var carOwnerViewModels = _applicationDbContext.CarOwners.Select(co => new DropDownViewModel
            {
                Id = co.Id,
                Value = $"{co.Name} {co.Email}"
            }).ToList();

            var carViewModel = new CarViewModel()
            {
                CarOwners = carOwnerViewModels
            };

            return View(carViewModel);
        }

        [HttpPost]
        public IActionResult AddNewCar(CarViewModel carViewModel)
        {
            if (!ModelState.IsValid)
            {
                var carOwnerViewModels = _applicationDbContext.CarOwners.Select(co => new DropDownViewModel
                {
                    Id = co.Id,
                    Value = $"{co.Name} {co.Email}"
                }).ToList();

                carViewModel.CarOwners = carOwnerViewModels;

                return View("AddCar", carViewModel);
            }

            var existingCar = _applicationDbContext.Cars.FirstOrDefault(c => c.PlateNumber == carViewModel.PlateNumber);

            if (existingCar != null)
            {
                ModelState.AddModelError("PlateNumber", "There is an already existing car with the same plate number");

                var carOwnerViewModels = _applicationDbContext.CarOwners.Select(co => new DropDownViewModel
                {
                    Id = co.Id,
                    Value = $"{co.Name} {co.Email}"
                }).ToList();

                carViewModel.CarOwners = carOwnerViewModels;

                return View("AddCar", carViewModel);
            }

            _applicationDbContext.Cars.Add(new CarDbModel
            {
                PlateNumber = carViewModel.PlateNumber,
                OwnerId = carViewModel.CarOwnerId
            });

            _applicationDbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult EditCar(int id)
        {
            var existingCar = _applicationDbContext.Cars.FirstOrDefault(cm => cm.Id == id);

            if (existingCar == null)
            {
                return RedirectToAction("Index");
            }

            var carOwnerViewModels = _applicationDbContext.CarOwners.Select(co => new DropDownViewModel
            {
                Id = co.Id,
                Value = $"{co.Name} {co.Email}"
            }).ToList();

            var carViewModel = new CarViewModel
            {
                Id = existingCar.Id,
                PlateNumber = existingCar.PlateNumber,
                CarOwnerId = existingCar.OwnerId,
                CarOwners = carOwnerViewModels
            };

            return View(carViewModel);
        }

        [HttpPost]
        public IActionResult EditExistingCar(CarViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var carOwnerViewModels = _applicationDbContext.CarOwners.Select(co => new DropDownViewModel
                {
                    Id = co.Id,
                    Value = $"{co.Name} {co.Email}"
                }).ToList();

                model.CarOwners = carOwnerViewModels;

                return View("EditCar", model);
            }

            var plateNumberAvailable = _applicationDbContext.Cars.Any(c => c.PlateNumber == model.PlateNumber &&
                                                                                        c.Id != model.Id);

            if (plateNumberAvailable)
            {
                ModelState.AddModelError("PlateNumber", "There is an already existing car with the same plate number");

                var carOwnerViewModels = _applicationDbContext.CarOwners.Select(co => new DropDownViewModel
                {
                    Id = co.Id,
                    Value = $"{co.Name} {co.Email}"
                }).ToList();

                model.CarOwners = carOwnerViewModels;

                return View("EditCar", model);
            }

            var existingCar = _applicationDbContext.Cars.FirstOrDefault(cm => cm.Id == model.Id);

            if (existingCar != null)
            {
                existingCar.PlateNumber = model.PlateNumber;
                existingCar.OwnerId = model.CarOwnerId;

                _applicationDbContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult DeleteCar(int id)
        {
            var existingCar = _applicationDbContext.Cars.FirstOrDefault(cm => cm.Id == id);

            if (existingCar != null)
            {
                _applicationDbContext.Cars.Remove(existingCar);
                _applicationDbContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult CarDetails(int id)
        {
            var existingCar = _applicationDbContext.Cars.Include(c => c.Owner).FirstOrDefault(c => c.Id == id);

            if (existingCar == null)
            {
                return RedirectToAction("Index");
            }

            var availableBookings = _applicationDbContext.Bookings
                .Include(b => b.ChargeMachine)
                .Where(b => b.CarId == existingCar.Id)
                .OrderByDescending(b => b.StartTime)
                .ToList();

            var carDetailsViewModel = new CarDetailsViewModel()
            {
                Car = new CarViewModel()
                {
                    Id = existingCar.Id,
                    PlateNumber = existingCar.PlateNumber,
                    CarOwner = $"{existingCar.Owner.Name} {existingCar.Owner.Email}"
                },
                Bookings = availableBookings.Select(ab => new BookingViewModel
                {
                    Id = ab.Id,
                    Code = ab.Code,
                    StartTime = ab.StartTime,
                    ChargeMachine = new ChargeMachineViewModel()
                    {
                        Id = ab.ChargeMachine.Id,
                        City = ab.ChargeMachine.City,
                        Code = ab.ChargeMachine.Code,
                        Latitude = ab.ChargeMachine.Latitude,
                        Longitude = ab.ChargeMachine.Longitude,
                        Number = ab.ChargeMachine.Number,
                        Street = ab.ChargeMachine.Street
                    }
                }).ToList()
            };

            return View(carDetailsViewModel);
        }
    }
}
