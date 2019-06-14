using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using WebStore.DomainNew.Entities;

namespace WebStore.DAL
{
    public static class DbInitializer
    {
        public static void Initialize(WebStoreContext context)
        {
            context.Database.EnsureCreated();

            if (context.Products.Any())
            {
                return;
            }

            var _sections = new List<Section>
            {
                new Section()
                {
                    Id = 1,
                    Name = "Sportswear",
                    Order = 0,
                    ParentId = null
                },
                new Section()
                {
                    Id = 2,
                    Name = "Nike",
                    Order = 0,
                    ParentId = 1
                },
                new Section()
                {
                    Id = 3,
                    Name = "Under Armour",
                    Order = 1,
                    ParentId = 1
                },
                new Section()
                {
                    Id = 4,
                    Name = "Adidas",
                    Order = 2,
                    ParentId = 1
                },
                new Section()
                {
                    Id = 5,
                    Name = "Puma",
                    Order = 3,
                    ParentId = 1
                },
                new Section()
                {
                    Id = 6,
                    Name = "ASICS",
                    Order = 4,
                    ParentId = 1
                },
                new Section()
                {
                    Id = 7,
                    Name = "Mens",
                    Order = 1,
                    ParentId = null
                },
                new Section()
                {
                    Id = 8,
                    Name = "Fendi",
                    Order = 0,
                    ParentId = 7
                },
                new Section()
                {
                    Id = 9,
                    Name = "Guess",
                    Order = 1,
                    ParentId = 7
                },
                new Section()
                {
                    Id = 10,
                    Name = "Valentino",
                    Order = 2,
                    ParentId = 7
                },
                new Section()
                {
                    Id = 11,
                    Name = "Dior",
                    Order = 3,
                    ParentId = 7
                },
                new Section()
                {
                    Id = 12,
                    Name = "Versace",
                    Order = 4,
                    ParentId = 7
                },
                new Section()
                {
                    Id = 13,
                    Name = "Armani",
                    Order = 5,
                    ParentId = 7
                },
                new Section()
                {
                    Id = 14,
                    Name = "Prada",
                    Order = 6,
                    ParentId = 7
                },
                new Section()
                {
                    Id = 15,
                    Name = "Dolce and Gabbana",
                    Order = 7,
                    ParentId = 7
                },
                new Section()
                {
                    Id = 16,
                    Name = "Chanel",
                    Order = 8,
                    ParentId = 7
                },
                new Section()
                {
                    Id = 17,
                    Name = "Gucci",
                    Order = 1,
                    ParentId = 7
                },
                new Section()
                {
                    Id = 18,
                    Name = "Womens",
                    Order = 2,
                    ParentId = null
                },
                new Section()
                {
                    Id = 19,
                    Name = "Fendi",
                    Order = 0,
                    ParentId = 18
                },
                new Section()
                {
                    Id = 20,
                    Name = "Guess",
                    Order = 1,
                    ParentId = 18
                },
                new Section()
                {
                    Id = 21,
                    Name = "Valentino",
                    Order = 2,
                    ParentId = 18
                },
                new Section()
                {
                    Id = 22,
                    Name = "Dior",
                    Order = 3,
                    ParentId = 18
                },
                new Section()
                {
                    Id = 23,
                    Name = "Versace",
                    Order = 4,
                    ParentId = 18
                },
                new Section()
                {
                    Id = 24,
                    Name = "Kids",
                    Order = 3,
                    ParentId = null
                },
                new Section()
                {
                    Id = 25,
                    Name = "Fashion",
                    Order = 4,
                    ParentId = null
                },
                new Section()
                {
                    Id = 26,
                    Name = "Households",
                    Order = 5,
                    ParentId = null
                },
                new Section()
                {
                    Id = 27,
                    Name = "Interiors",
                    Order = 6,
                    ParentId = null
                },
                new Section()
                {
                    Id = 28,
                    Name = "Clothing",
                    Order = 7,
                    ParentId = null
                },
                new Section()
                {
                    Id = 29,
                    Name = "Bags",
                    Order = 8,
                    ParentId = null
                },
                new Section()
                {
                    Id = 30,
                    Name = "Shoes",
                    Order = 9,
                    ParentId = null
                }
            };

            foreach (var section in _sections)
            {
                context.Sections.Add(section);
            }

            using (var trans = context.Database.BeginTransaction())
            {
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].Sections ON");
                context.SaveChanges();
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].Sections OFF");

                trans.Commit();
            }

            var _brands = new List<Brand>()
            {
                new Brand()
                {
                    Id = 1,
                    Name = "Acne",
                    Order = 0
                },
                new Brand()
                {
                    Id = 2,
                    Name = "Grüne Erde",
                    Order = 1
                },
                new Brand()
                {
                    Id = 3,
                    Name = "Albiro",
                    Order = 2
                },
                new Brand()
                {
                    Id = 4,
                    Name = "Ronhill",
                    Order = 3
                },
                new Brand()
                {
                    Id = 5,
                    Name = "Oddmolly",
                    Order = 4
                },
                new Brand()
                {
                    Id = 6,
                    Name = "Boudestijn",
                    Order = 5
                },
                new Brand()
                {
                    Id = 7,
                    Name = "Rösch creative culture",
                    Order = 6
                },
            };
            foreach (var brand in _brands)
            {
                context.Brands.Add(brand);
            }

            using (var trans = context.Database.BeginTransaction())
            {
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].Brands ON");
                context.SaveChanges();
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].Brands OFF");

                trans.Commit();
            }

            var _products = new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    Name = "Easy Polo Black Edition",
                    Price = 1025,
                    ImageUrl = "product1.jpg",
                    Order = 0,
                    SectionId = 2,
                    BrandId = 1
                },
                new Product()
                {
                    Id = 2,
                    Name = "Easy Polo Black Edition",
                    Price = 1025,
                    ImageUrl = "product2.jpg",
                    Order = 1,
                    SectionId = 2,
                    BrandId = 1
                },
                new Product()
                {
                    Id = 3,
                    Name = "Easy Polo Black Edition",
                    Price = 1025,
                    ImageUrl = "product3.jpg",
                    Order = 2,
                    SectionId = 2,
                    BrandId = 1
                },
                new Product()
                {
                    Id = 4,
                    Name = "Easy Polo Black Edition",
                    Price = 1025,
                    ImageUrl = "product4.jpg",
                    Order = 3,
                    SectionId = 2,
                    BrandId = 1
                },
                new Product()
                {
                    Id = 5,
                    Name = "Easy Polo Black Edition",
                    Price = 1025,
                    ImageUrl = "product5.jpg",
                    Order = 4,
                    SectionId = 2,
                    BrandId = 2
                },
                new Product()
                {
                    Id = 6,
                    Name = "Easy Polo Black Edition",
                    Price = 1025,
                    ImageUrl = "product6.jpg",
                    Order = 5,
                    SectionId = 2,
                    BrandId = 2
                },
                new Product()
                {
                    Id = 7,
                    Name = "Easy Polo Black Edition",
                    Price = 1025,
                    ImageUrl = "product7.jpg",
                    Order = 6,
                    SectionId = 2,
                    BrandId = 2
                },
                new Product()
                {
                    Id = 8,
                    Name = "Easy Polo Black Edition",
                    Price = 1025,
                    ImageUrl = "product8.jpg",
                    Order = 7,
                    SectionId = 25,
                    BrandId = 2
                },
                new Product()
                {
                    Id = 9,
                    Name = "Easy Polo Black Edition",
                    Price = 1025,
                    ImageUrl = "product9.jpg",
                    Order = 8,
                    SectionId = 25,
                    BrandId = 2
                },
                new Product()
                {
                    Id = 10,
                    Name = "Easy Polo Black Edition",
                    Price = 1025,
                    ImageUrl = "product10.jpg",
                    Order = 9,
                    SectionId = 25,
                    BrandId = 3
                },
                new Product()
                {
                    Id = 11,
                    Name = "Easy Polo Black Edition",
                    Price = 1025,
                    ImageUrl = "product11.jpg",
                    Order = 10,
                    SectionId = 25,
                    BrandId = 3
                },
                new Product()
                {
                    Id = 12,
                    Name = "Easy Polo Black Edition",
                    Price = 1025,
                    ImageUrl = "product12.jpg",
                    Order = 11,
                    SectionId = 25,
                    BrandId = 3
                },
            };
            foreach (var prod in _products)
            {
                context.Products.Add(prod);
            }

            using (var trans = context.Database.BeginTransaction())
            {
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].Products ON");
                context.SaveChanges();
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].Products OFF");

                trans.Commit();
            }
        }

        public static void InitializeEmployers(WebStoreContext context)
        {
            context.Database.EnsureCreated();

            if (context.Employees.Any())
            {
                return;
            }

            var _employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    FirstName = "Иван",
                    SurName = "Иванов",
                    Patronymic = "Иванович",
                    Age = 22,
                    Position = "Директор"
                },
                new Employee
                {
                    Id = 2,
                    FirstName = "Владислав",
                    SurName = "Петров",
                    Patronymic = "Иванович",
                    Age = 35,
                    Position = "Зам. Директора"
                }
            };

            foreach (var employee in _employees)
            {
                context.Employees.Add(employee);
            }

            using (var trans = context.Database.BeginTransaction())
            {
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].Employees ON");
                context.SaveChanges();
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].Employees OFF");

                trans.Commit();
            }
        }

        public static void InitializeUsers(IServiceProvider service)
        {
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            EnsureRole(roleManager, "User");
            EnsureRole(roleManager, "Administrator");

            EnsureRoleToUser(service, "AdminMain", "Administrator", "Admin@123");
        }

        private static void EnsureRoleToUser(IServiceProvider service, string userName, string roleName, string password)
        {
            var userManager = service.GetService<UserManager<User>>();
            var users = service.GetService<IUserStore<User>>();

            if (users.FindByNameAsync(userName, CancellationToken.None).Result != null)
                return;

            var admin = new User
            {
                UserName = userName,
                Email = $"{userName}@server.com"
            };

            if (userManager.CreateAsync(admin, password).Result.Succeeded) // добавляем пользователя в БД
                userManager.AddToRoleAsync(admin, roleName).Wait(); // даем ему роль админа
        }

        private static void EnsureRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!roleManager.RoleExistsAsync(roleName).Result)
                roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
        }
    }
}