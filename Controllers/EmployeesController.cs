using Employees.Data.Entitites;
using Employees.Service.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employees.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IEmployeesService _employeesService;

        private List<Employee> _employees = new List<Employee>();

        public EmployeesController(IConfiguration configuration, IEmployeesService employeesService)
        {
            _configuration = configuration;
            _employeesService = employeesService;
        }

        [HttpPost]
        [Route("ImportFile")]
        public Tuple<int, int, int> UploadFile([FromForm] IFormFile file)
        {
            _employeesService.ImportEmployeesFromCSV(ref _employees, file);
            var result = _employeesService.FindLongestWorkingEmployeesPair(_employees);
            return result;
        }
    }
}
