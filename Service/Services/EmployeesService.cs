using CsvHelper;
using CsvHelper.Configuration;
using Employees.Data.Entitites;
using Employees.Service.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Formats.Asn1;
using System.Globalization;

namespace Employees.Service.Services
{
    public class EmployeesService : IEmployeesService
    {
        public Tuple<int, int, int> FindLongestWorkingEmployeesPair(List<Employee> employees)
        {
            Dictionary<Tuple<int, int, int>, int> durations = new Dictionary<Tuple<int, int, int>, int>();

            foreach (var item in employees)
            {
                foreach (var item2 in employees)
                {
                    if (item.Id == item2.Id || item.ProjectId != item2.ProjectId)
                        continue;

                    int duration = 0;
                    if (item.DateFrom < item2.DateFrom)
                    {
                        if (item.DateTo == null || item.DateTo > item2.DateFrom)
                            duration = item2.DateTo == null ? (int)(DateTime.Now - item2.DateFrom).TotalDays : (int)(item2.DateTo.Value - item2.DateFrom).TotalDays;
                    }
                    else
                    {
                        if (item2.DateTo == null || item2.DateTo > item.DateFrom)
                            duration = item.DateTo == null ? (int)(DateTime.Now - item.DateFrom).TotalDays : (int)(item.DateTo.Value - item.DateFrom).TotalDays;
                    }

                    if (duration > 0)
                    {
                        var empPair = Tuple.Create(Math.Min(item.Id, item2.Id), Math.Max(item.Id, item2.Id), item.ProjectId);
                        if (!durations.ContainsKey(empPair))
                            durations[empPair] = duration;
                    }
                }
            }
            int maxEmp1, maxEmp2, maxDuration;
            Dictionary<Tuple<int, int>, int> totalDurations = new Dictionary<Tuple<int, int>, int>();

            foreach (var pair in durations)
            {
                var currentEmployeePair = Tuple.Create(Math.Min(pair.Key.Item1, pair.Key.Item2), Math.Max(pair.Key.Item1, pair.Key.Item2));

                if (totalDurations.ContainsKey(currentEmployeePair))
                {
                    totalDurations[currentEmployeePair] += pair.Value;
                }
                else
                {
                    totalDurations[currentEmployeePair] = pair.Value;
                }
            }

            var longestDurationPair =
            totalDurations.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            maxEmp1 = longestDurationPair.Item1;
            maxEmp2 = longestDurationPair.Item2;
            maxDuration = totalDurations[longestDurationPair];
            return Tuple.Create(maxEmp1, maxEmp2, maxDuration);
        }

        public void ImportEmployeesFromCSV(ref List<Employee> employees, IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    DateTime currentDateFrom;
                    DateTime currentDateTo;

                    if (values[2].IsNullOrEmpty() || values[2].ToLower() == "null")
                    {
                        currentDateFrom = DateTime.Now;
                    }
                    else
                    {
                        currentDateFrom = DateTime.Parse(values[2]);
                    }

                    if (values[3].IsNullOrEmpty() || values[3].ToLower() == "null")
                    {
                        currentDateTo = DateTime.Now;
                    }
                    else
                    {
                        currentDateTo = DateTime.Parse(values[3]);
                    }

                    employees.Add(new Employee
                    {
                        Id = int.Parse(values[0]),
                        ProjectId = int.Parse(values[1]),
                        DateFrom = currentDateFrom,
                        DateTo = currentDateTo
                    });
                }
            }
        }

        private static DateTime? Max(DateTime? a, DateTime? b)
        {
            return a > b ? a : b;
        }

        private static DateTime? Min(DateTime? a, DateTime? b)
        {
            return a > b ? b : a;
        }
    }
}
