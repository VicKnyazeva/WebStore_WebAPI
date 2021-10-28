using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

using WebStore.Interfaces.Services;
using WebStore.Models;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Employees
{
    public class EmployeesClient : BaseClient, IEmployeesData
    {
        public EmployeesClient(HttpClient Client) 
            : base(Client, "api/employees") { }


        public IEnumerable<Employee> GetAll()
        {
            var employees = Get<IEnumerable<Employee>>(Address);

            return employees;
        }

        public Employee GetById(int id)
        {
            var result = Get<Employee>($"{Address}/{id}");
            
            return result;
        }

        public int Add(Employee employee)
        {
            var response = Post(Address, employee);
            var addedEmployee = response.Content.ReadFromJsonAsync<Employee>().Result;
            
            if (addedEmployee is null)
                return -1;

            var id = addedEmployee.Id;

            return id;
        }

        public void Update(Employee employee)
        {
            Put(Address, employee);
        }

        public bool Delete(int id)
        {
            var response = Delete($"{Address}/{id}");
            var success = response.IsSuccessStatusCode;

            return success;
        }
    }
}
