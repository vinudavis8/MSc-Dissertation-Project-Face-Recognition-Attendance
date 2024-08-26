using AttendanceApi.Data;
using AttendanceApi.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AttendanceApi.Repository
{
    public class CourseModuleRepository : ICourseModuleRepository
    {
        private readonly AttendanceDbContext dbContext;

        public CourseModuleRepository(AttendanceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<CourseModule>> GetAllAsync()
        {
            var vv= dbContext.CourseModule.Include(x => x.CourseId).ToListAsync();
            return await dbContext.CourseModule.Include(x => x.Course).ToListAsync();
        }
        public async Task<CourseModule> GetAsync(int id)
        {
            return await dbContext.CourseModule.Include(x => x.Course).SingleOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<Schedule>> GetSheduleAsync(int courseId)
        {
            // LINQ query
            var result = from s in dbContext.Schedule
                         join cm in dbContext.CourseModule on s.CourseModuleId equals cm.Id
                         where cm.CourseId == courseId
                         select new Schedule
                         {
                             Id = s.Id,
                             From = s.From,
                             To = s.To,
                             CourseModuleId = s.CourseModuleId,
                             CourseModule = cm
                         };
            var schedule= await result.ToListAsync();
            return schedule; 
        }
        public async Task<CourseModule> CreateAsync(CourseModule module)
        {
            dbContext.CourseModule.Add(module);
            await dbContext.SaveChangesAsync();
            return module;
        }
        public async Task<Schedule> CreateScheduleAsync(Schedule schedule)
        {
            dbContext.Schedule.Add(schedule);
            await dbContext.SaveChangesAsync();
            return schedule;
        }
        public async Task<CourseModule> UpdateAsync(CourseModule module)
        {
            CourseModule result = await dbContext.CourseModule.SingleOrDefaultAsync(x => x.Id == module.Id);
            if (result == null)
                return null;

            result.Name = module.Name;
            await dbContext.SaveChangesAsync();
            return module;
        }

        public async Task<CourseModule> DeleteAsync(int id)
        {
            CourseModule module = await dbContext.CourseModule.SingleOrDefaultAsync(x => x.Id == id);
            if (module == null)
                return null;
            dbContext.CourseModule.Remove(module);
            await dbContext.SaveChangesAsync();
            return module;
        }
    }
}
