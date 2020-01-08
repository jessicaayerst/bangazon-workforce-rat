﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BangazonWorkforce.Models;

namespace BangazonWorkforce.Controllers
{
    public class EmployeesController : Controller
    {
        // GET: Employee
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Employees
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                     SELECT e.Id,
                     e.FirstName,
					 e.LastName,
                    d.Name
                    as 'Department'

                    FROM Employee e LEFT JOIN Department d ON e.DepartmentId = d.id";
;
                    //SELECT e.Id,
                    // e.FirstName,
                    //e.LastName
                    //FROM Employee e

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),

                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("Department")))
                        {
                            Department department = new Department()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),

                            };
                            if (employees.Any(e => e.Id == employee.Id))
                            {
                                Employee employeeToReference = employees.Where(e => e.Id == employee.Id).FirstOrDefault();
                                if (!employeeToReference.CurrentDepartment.Any(s => s.Id == department.Id))

                                    employeeToReference.CurrentDepartment.Add(department);
                                }
                            }
                            else
                            {
                                employee.CurrentDepartment.Add(department);
                                employees.Add(employee);
                            }
                        }



                    //List<Department> departments = new List<Department>();
                    //while (reader.Read())
                    //{
                    //    Department department = new Department
                    //    {
                    //        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    //        Name = reader.GetString(reader.GetOrdinal("Name")),
                    //    };

                    //    departments.Add(department);
                   //}

                    reader.Close();

                    return View(employees);
                }
            }
        }
    



        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {


             using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                         SELECT Employee.Id, Employee.FirstName, Employee.LastName, Employee.IsSupervisor, Employee.DepartmentId AS 'Department Id', ComputerEmployee.AssignDate, ComputerEmployee.UnassignDate, Computer.Make, Computer.Manufacturer, TrainingProgram.[Name] AS 'Training Program Name', TrainingProgram.Id AS 'Training Program Id'
FROM Employee

 LEFT JOIN EmployeeTraining on EmployeeTraining.EmployeeId = Employee.Id
 LEFT JOIN TrainingProgram on EmployeeTraining.TrainingProgramId = TrainingProgram.Id
LEFT JOIN ComputerEmployee on ComputerEmployee.EmployeeId = Employee.Id
LEFT JOIN Computer ON ComputerEmployee.ComputerId = Computer.Id
WHERE Employee.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;

                    while (reader.Read())
                    {


                        if (employee == null)
                        {

                            employee = new Employee()

                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("Department Id")),
                                TrainingPrograms = new List<TrainingProgram>()
                            };
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("UnassignDate")))
                        {
                            employee.CurrentComputer = new Computer()
                            {
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            };
                        }
                        else
                        {
                            employee.CurrentComputer = null;
                        }



                            if (!reader.IsDBNull(reader.GetOrdinal("Training Program Id")))
                            {

                                TrainingProgram trainingProgram = new TrainingProgram()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Training Program Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Training Program Name"))
                                };

                            if (!employee.TrainingPrograms.Any(e => e.Id == trainingProgram.Id))
                            {
                                employee.TrainingPrograms.Add(trainingProgram);
                            }
                            }





                    }

                        reader.Close();

                        return View(employee);
                    }
                }
            }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
