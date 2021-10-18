using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using WebStore.Models;
using WebStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebStore.ViewModels
{
    public class EmployeeViewModel
    {
        public EmployeeViewModel()
        {
        }
        public EmployeeViewModel(Employee employee)
        {
            Id = employee.Id;
            Name = employee.FirstName;
            LastName = employee.LastName;
            Patronymic = employee.Patronymic;
            BirthDate = employee.BirthDate;
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [DisplayName("Имя"), Required(ErrorMessage = "Имя не указано"), StringLength(200, MinimumLength = 2, ErrorMessage = "Длина строки должна быть минимум 2 символа")]
        [RegularExpression(@"([А-ЯЁ][а-яё]+|[A-Z][a-z]+)", ErrorMessage = "Неверный формат")]
        public string Name { get; set; }

        [DisplayName("Фамилия"), Required(ErrorMessage = "Фамилия не указана"), StringLength(200, MinimumLength = 2, ErrorMessage = "Длина строки должна быть минимум 2 символа")]
        [RegularExpression(@"([А-ЯЁ][а-яё]+|[A-Z][a-z]+)", ErrorMessage = "Неверный формат")]
        public string LastName { get; set; }

        [DisplayName("Отчество")]
        public string Patronymic { get; set; }

        [DisplayName("Возраст")]
        public int Age
        {
            get { return AgeHelper.AgeFromBirthDate(BirthDate); }
        }

        [DisplayName("Дата рождения"), Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}
