using AttendanceApi.Data;
using AttendanceApi.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AttendanceApi.Repository
{
    public class CourseRepository:ICourseRepository
    {
        private readonly AttendanceDbContext dbContext;

        public CourseRepository(AttendanceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Course>> GetAllAsync()
        {
            return await dbContext.Courses.Include(x => x.Department).ToListAsync();
        }
        public async Task<List<Course>> GetCourseByDepIdAsync(int departmentId)
        {
            return await dbContext.Courses.Include(x=>x.Department).Where(x => x.DepartmentId == departmentId).ToListAsync();
        }
        public async Task<Course> GetAsync(int id)
        {
            return await dbContext.Courses.Include(x => x.Department).SingleOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Course> CreateAsync(Course module)
        {
            dbContext.Courses.Add(module);
            await dbContext.SaveChangesAsync();
            return module;
        }
        public async Task<Course> UpdateAsync(Course module)
        {
            Course result = await dbContext.Courses.SingleOrDefaultAsync(x => x.Id == module.Id);
            if (result == null)
                return null;

            result.Name = module.Name;
            await dbContext.SaveChangesAsync();
            return module;
        }

        public async Task<Course> DeleteAsync(int id)
        {
            Course module = await dbContext.Courses.SingleOrDefaultAsync(x => x.Id == id);
            if (module == null)
                return null;
            dbContext.Courses.Remove(module);
            await dbContext.SaveChangesAsync();
            return module;
        }
    }
}
