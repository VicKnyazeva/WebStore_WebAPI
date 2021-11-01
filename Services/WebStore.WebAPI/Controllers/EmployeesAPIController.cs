using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebStore.Interfaces;
using WebStore.Interfaces.Services;
using WebStore.Models;

namespace WebStore.WebAPI.Controllers
{
    /// <summary>
    /// Управление сотрудниками
    /// </summary>
    [Route(WebAPIAddresses.Employees)]
    [ApiController]
    public class EmployeesAPIController : ControllerBase
    {
        private readonly IEmployeesData _EmployeesData;
        private readonly ILogger<EmployeesAPIController> _Logger;

        public EmployeesAPIController(IEmployeesData EmployeesData, ILogger<EmployeesAPIController> Logger)
        {
            _EmployeesData = EmployeesData;
            _Logger = Logger;
        }

        /// <summary>
        /// Получение всех сотрудников
        /// </summary>
        /// <returns>Список сотрудников</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var employees = _EmployeesData.GetAll();
            _Logger.LogInformation("Получение списка всех сотрудников");
            return Ok(employees);
        }

        /// <summary>
        /// Получение сотрудника по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Сотрудник с указанным id</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Employee))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var employee = _EmployeesData.GetById(id);
            if (employee is null)
            {
                _Logger.LogWarning("Пользователь с id {0} не найден", id);
                return NotFound();
            }

            return Ok(employee);
        }
                

        [HttpPut]
        public IActionResult Update(Employee employee)
        {
            _EmployeesData.Update(employee);
            return Ok();
        }

        [HttpPost]
        public IActionResult Add(Employee employee)
        {
            var id = _EmployeesData.Add(employee);
            _Logger.LogInformation("Пользователь успешно создан ");
            return CreatedAtAction(nameof(GetById), new { id }, employee);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _EmployeesData.Delete(id);
            if (result)
            {
                _Logger.LogInformation("Пользователь с id {0} успешно удален из системы", id);
                return Ok(true);
            }
            else
            {
                _Logger.LogWarning("Пользователь с id {0} в системе не найден", id);
                return NotFound(false);
            }
        }
    }
}
