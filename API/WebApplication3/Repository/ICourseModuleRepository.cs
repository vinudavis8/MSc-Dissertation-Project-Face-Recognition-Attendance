using AttendanceApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApi.Repository
{
    public interface ICourseModuleRepository
    {
        Task<List<CourseModule>> GetAllAsync();
        Task<CourseModule> GetAsync(int id);
        Task<List<Schedule>> GetSheduleAsync(int courseId);
        Task<CourseModule> CreateAsync(CourseModule module);
        Task<Schedule> CreateScheduleAsync(Schedule schedule);

        Task<CourseModule> UpdateAsync(CourseModule module);
        Task<CourseModule> DeleteAsync(int id);
    }
}
