using Microsoft.AspNetCore.Mvc;
using TPFinal_LuzziLuca.Models;
using TPFinal_LuzziLuca.Services;

namespace TPFinal_LuzziLuca.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IRepositoryEmployees repositoryEmployees;

        public EmployeesController(IRepositoryEmployees repositoryEmployees)
        {
            this.repositoryEmployees = repositoryEmployees;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await repositoryEmployees.GetEmployees();
            return View(employees);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            bool alreadyExist = await repositoryEmployees.Exist(employee.Fullname);

            if (alreadyExist)
            {
                ModelState.AddModelError(nameof(employee.Fullname), $"Fullname already exist");

                return View(employee);
            }

            await repositoryEmployees.insertNewEmployee(employee);
            await repositoryEmployees.InsertJobAndSalary(employee.Fullname, employee.JobName, employee.Salary);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var emp = await repositoryEmployees.GetEmployeeById(id);
            var empJob = await repositoryEmployees.GetJobById_Employee(id);

            if (emp.Age == 0)
            {
                return RedirectToAction("NotFound", "Home");
            }

            Employee employee = new Employee();
            employee.Id = id;
            employee.Fullname = emp.Fullname;
            employee.Email = emp.Email;
            employee.Age = emp.Age;
            employee.JobName = empJob.Name;
            employee.Salary = empJob.Salary;

            return View(employee);

        }

        [HttpPost]
        public async Task<ActionResult> Edit(Employee employee)
        {
            await repositoryEmployees.UpdateEmployee(employee);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var emp = await repositoryEmployees.GetEmployeeById(id);
            var empJob = await repositoryEmployees.GetJobById_Employee(id);

            if (emp.Age == 0)
            {
                return RedirectToAction("NotFound", "Home");
            }


            Employee employee = new Employee();
            employee.Id = id;
            employee.Fullname = emp.Fullname;
            employee.Email = emp.Email;
            employee.Age = emp.Age;
            employee.JobName = empJob.Name;
            employee.Salary = empJob.Salary;

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var emp = await repositoryEmployees.GetEmployeeById(id);

            if (emp.Age == 0)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await repositoryEmployees.DeleteEmployee(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyExistEmployee(string Fullname)
        {
            bool alreadyExist = await repositoryEmployees.Exist(Fullname);

            if (alreadyExist)
            {
                return Json($"The Fullname {Fullname} already exist.");
            }

            return Json(true);
        }
    }
}
