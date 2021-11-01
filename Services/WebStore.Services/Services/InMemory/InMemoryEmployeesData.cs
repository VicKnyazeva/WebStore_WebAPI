using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;

using WebStore.Services.Data;
using WebStore.Models;
using WebStore.Interfaces.Services;

namespace WebStore.Services.Services.InMemory
{
    public class InMemoryEmployeesData : IEmployeesData
    {
        private readonly ILogger<InMemoryEmployeesData> _Logger;

        private int _CurrentMaxId;

        public InMemoryEmployeesData(ILogger<InMemoryEmployeesData> Logger)
        {
            _Logger = Logger;
            _CurrentMaxId = TestData.Employees.Max(i => i.Id);
        }
                

        public int Add(Employee employee)
        {
            if (employee is null) 
                throw new ArgumentNullException(nameof(employee));

            if (TestData.Employees.Contains(employee)) 
                return employee.Id;

            employee.Id = ++_CurrentMaxId;
            TestData.Employees.Add(employee);

            _Logger.LogInformation("Сотрудник {0} успешно добавлен", employee);

            return employee.Id;
        }

        public bool Delete(int id)
        {
            var db_employee = GetById(id);
            if (db_employee is null)
            {
                _Logger.LogInformation("При попытке удаления информация о сотруднике {0} не найдена", db_employee);
                return false;
            }    

            TestData.Employees.Remove(db_employee);
            _Logger.LogInformation("Информация о сотруднике {0} успешно удалена", db_employee);

            return true;
        }

        public IEnumerable<Employee> GetAll()
        {
            return TestData.Employees;
        }

        public Employee GetById(int id)
        {
            return TestData.Employees.SingleOrDefault(i => i.Id == id);
        }

        public void Update(Employee employee)
        {
            if (employee is null)
                throw new ArgumentNullException(nameof(employee));

            if (TestData.Employees.Contains(employee))
                return;

            var db_employee = GetById(employee.Id);
            if (db_employee is null)
                return;

            db_employee.LastName = employee.LastName;
            db_employee.FirstName = employee.FirstName;
            db_employee.Patronymic = employee.Patronymic;
            db_employee.BirthDate = employee.BirthDate;

            _Logger.LogInformation("Информация о сотруднике {0} успешно обновлена", employee);
        }
    }
}
