using Microsoft.AspNetCore.Authorization;
using WebStore.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebStore.DomainNew.Entities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebStore.Controllers
{
    [Route("users")]
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeesData _employeesData;

        public EmployeesController(IEmployeesData employeesData)
        {
            _employeesData = employeesData;
        }    

        /// <summary>
        /// Вывод списка
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View(_employeesData.GetAll());
        }

        /// <summary>
        /// Детали о сотруднике
        /// </summary>
        /// <param name="id">Id сотрудника</param>
        /// <returns></returns>
        [Route("details")]
        public IActionResult Details(int id)
        {
            var employee = _employeesData.GetById(id);

            if (ReferenceEquals(employee, null))
                return NotFound();

            return View(employee);
        }

        /// <summary>
        /// Добавление или редактирование сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("edit/{id?}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(int? id)
        {
            Employee model;
            if(id.HasValue)
            {
                model = _employeesData.GetById(id.Value);
                if(ReferenceEquals(model, null))
                    return NotFound();
            }
            else
            {
                model = new Employee();
            }

            return View(model);
        }

        [HttpPost]
        [Route("edit/{id?}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(Employee model)
        {
            if (IsFormCorrect(model) == false)
            {
                ModelState.AddModelError("Age", "Ошибка возраста!");
            }

            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    var dbItem = _employeesData.GetById(model.Id);

                    if (ReferenceEquals(dbItem, null))
                        return NotFound();

                    dbItem.FirstName = model.FirstName;
                    dbItem.SurName = model.SurName;
                    dbItem.Age = model.Age;
                    dbItem.Patronymic = model.Patronymic;
                    dbItem.Position = model.Position;
                }
                else
                {
                    _employeesData.Add(model);
                }
                _employeesData.Commit();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        /// <summary>
        /// Удаление сотрудника
        /// </summary>
        /// <param name="id">Id сотрудника</param>
        /// <returns></returns>
        [Route("delete/{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete (int id)
        {
            _employeesData.Delete(id);
            _employeesData.Commit();
            return RedirectToAction(nameof(Index));
        }

        private bool IsFormCorrect(Employee model)
        {
            if (model.Age < 18 || model.Age > 75)
            {
                return false;
            }

            return true;
        }
    }
}
