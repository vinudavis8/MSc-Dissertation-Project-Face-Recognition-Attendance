using AttendanceApi.Models.Domain;

namespace AttendanceApi.Repository
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllAsync();
        Task<Course> GetAsync(int id);
        Task<List<Course>> GetCourseByDepIdAsync(int departmentId);

        Task<Course> CreateAsync(Course module);
        Task<Course> UpdateAsync(Course module);
        Task<Course> DeleteAsync(int id);
    }
}
