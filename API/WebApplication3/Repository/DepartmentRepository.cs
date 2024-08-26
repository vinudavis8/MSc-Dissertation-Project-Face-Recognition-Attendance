using AttendanceApi.Data;
using AttendanceApi.Models.Domain;
using AttendanceApi.Models.DTO.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApi.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AttendanceDbContext dbContext;

        public DepartmentRepository(AttendanceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Department>> GetAllAsync()
        {
            return await dbContext.Departments.ToListAsync();
        }
        public async Task<Department> GetAsync(int id)
        {
            return await dbContext.Departments.SingleOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Department> CreateAsync(Department department)
        {
            dbContext.Departments.Add(department);
            await dbContext.SaveChangesAsync();
            return department;
        }
        public async Task<Department> UpdateAsync(Department department)
        {
            Department result = await dbContext.Departments.SingleOrDefaultAsync(x => x.Id == department.Id);
            if (result == null)
                return null;

            result.Name = department.Name;
            await dbContext.SaveChangesAsync();
            return department;
        }

        public async Task<Department> DeleteAsync(int id)
        {
            Department department = await dbContext.Departments.SingleOrDefaultAsync(x => x.Id == id);
            if (department == null)
                return null;
            dbContext.Departments.Remove(department);
            await dbContext.SaveChangesAsync();
            return department;
        }
    }
}
