using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL;
using WebStore.DomainNew.Entities;
using WebStore.Infrastructure.Interfaces;

namespace WebStore.Infrastructure.Implementation
{
    public class SqlEmployeeData : IEmployeesData
    {
        private readonly WebStoreContext _context;

        public SqlEmployeeData(WebStoreContext context)
        {
            _context = context;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _context.Employees;
        }

        public Employee GetById(int id)
        {
            return _context.Employees.FirstOrDefault(t => t.Id.Equals(id));
        }

        public void Commit()
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                _context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].Employees ON");
                _context.SaveChanges();
                _context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].Employees OFF");

                trans.Commit();
            }
        }

        public void Add(Employee model)
        {
            model.Id = _context.Employees.Count() + 1;
            _context.Employees.Add(model);
        }

        public void Delete(int id)
        {
            Employee employee = GetById(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
        }
    }
}