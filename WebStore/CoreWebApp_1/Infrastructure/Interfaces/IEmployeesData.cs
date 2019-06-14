using System.Collections.Generic;
using WebStore.DomainNew.Entities;

namespace WebStore.Infrastructure.Interfaces
{
    /// <summary>
    /// Интерфейс для работы с сотрудниками
    /// </summary>
    public interface IEmployeesData
    {
        /// <summary>
        /// Получение списка сотрудников
        /// </summary>
        /// <returns></returns>
        IEnumerable<Employee> GetAll();

        /// <summary>
        /// Получение сотрудника по id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        Employee GetById(int id);

        /// <summary>
        /// отправить в хранилище
        /// </summary>
        void Commit();

        /// <summary>
        /// Добавить нового сотрудника
        /// </summary>
        /// <param name="model"></param>
        void Add(Employee model);

        /// <summary>
        /// Удалить
        /// </summary>
        /// <param name="id"></param>
        void Delete(int id);
    }
}