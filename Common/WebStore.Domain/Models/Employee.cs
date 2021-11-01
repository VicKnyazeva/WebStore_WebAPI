using System;

namespace WebStore.Models
{
    /// <summary>
    /// Информация о сотруднике
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime BirthDate { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {LastName} {FirstName} {Patronymic} {BirthDate}";
        }
    }
}
