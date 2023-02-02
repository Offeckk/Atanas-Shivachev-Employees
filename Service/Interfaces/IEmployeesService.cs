using Employees.Data.Entitites;

namespace Employees.Service.Interfaces
{
    public interface IEmployeesService
    {
        public void ImportEmployeesFromCSV(ref List<Employee> employees, IFormFile file);

        public Tuple<int, int, int> FindLongestWorkingEmployeesPair(List<Employee> employees);
    }
}
