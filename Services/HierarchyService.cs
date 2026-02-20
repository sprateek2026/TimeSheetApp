using Microsoft.EntityFrameworkCore;
using Timesheet.Api.Data;

namespace Timesheet.Api.Services
{
    public class HierarchyService
    {
        private readonly TimesheetDbContext _context;

        public HierarchyService(TimesheetDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetAllSubordinatesAsync(int managerId)
        {
            var result = new List<int>();
            await LoadSubordinates(managerId, result);
            return result;
        }

        private async Task LoadSubordinates(int managerId, List<int> result)
        {
            var subordinates = await _context.AppUsers
                .Where(u => u.ManagerId == managerId && u.IsActive)
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var empId in subordinates)
            {
                if (!result.Contains(empId))
                {
                    result.Add(empId);
                    await LoadSubordinates(empId, result);
                }
            }
        }
    }
}
