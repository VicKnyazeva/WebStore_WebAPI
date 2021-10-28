using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using WebStore.Domain.Entities.Identity;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.Models;

namespace WebStore.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeesData _EmployessData;
        private readonly ILogger<EmployeesController> _Logger;

        public EmployeesController(IEmployeesData EmployessData, ILogger<EmployeesController> Logger)
        {
            _EmployessData = EmployessData;
            _Logger = Logger;
        }

        public IActionResult Index()
        {
            return View(_EmployessData.GetAll()
                .Select(employee => new EmployeeViewModel(employee)));
        }

        public IActionResult Details(int id)
        {
            var employee = _EmployessData.GetById(id);
            if (employee is null)
                return NotFound();

            return View(new EmployeeViewModel(employee));
        }

        #region Create

        [Authorize(Roles = Role.Administrators)]
        public IActionResult Create() => View("Edit", new EmployeeViewModel());

        //[HttpPost]
        //public IActionResult Create(EmployeeViewModel viewModel)
        //{
        //    if (!ModelState.IsValid)
        //        return View(viewModel);

        //    var employee = new Employee
        //    {
        //        FirstName = viewModel.Name,
        //        LastName = viewModel.LastName,
        //        Patronymic = viewModel.Patronymic,
        //        BirthDate = viewModel.BirthDate
        //    };
        //    _EmployessData.Add(employee);

        //    return RedirectToAction(nameof(Index));
        //}

        #endregion

        #region Edit
        [HttpGet]
        [Authorize(Roles = Role.Administrators)]
        public IActionResult Edit(int? id)
        {
            if (id is null)
                return View(new EmployeeViewModel());

            var employee = _EmployessData.GetById((int)id);
            if (employee is null)
                return NotFound();

            var viewModel = new EmployeeViewModel(employee);

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = Role.Administrators)]
        public IActionResult Edit(EmployeeViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var employee = new Employee
            {
                Id = viewModel.Id,
                FirstName = viewModel.Name,
                LastName = viewModel.LastName,
                Patronymic = viewModel.Patronymic,
                BirthDate = viewModel.BirthDate
            };

            if (employee.Id == 0)
                _EmployessData.Add(employee);
            else
                _EmployessData.Update(employee);

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Delete
        
        [Authorize(Roles = Role.Administrators)]
        public IActionResult Delete(int id)
        {
            if (id < 0)
                return BadRequest();

            var employee = _EmployessData.GetById(id);
            if (employee is null)
                return NotFound();

            var viewModel = new EmployeeViewModel(employee);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = Role.Administrators)]
        public IActionResult DeleteConfirmed(int id)
        {
            //another yet checking if employee still exist,
            //because for current point of time it may be deleted already
            var employee = _EmployessData.GetById(id);
            if (employee is null)
                return NotFound();

            _EmployessData.Delete(employee.Id);

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
