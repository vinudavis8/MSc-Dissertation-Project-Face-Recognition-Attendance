using AttendanceApi.Models.Domain;

namespace AttendanceApi.Repository
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllAsync();
        Task<Student> GetAsync(int id);
        Task<Student> CreateAsync(Student student);
        Task<Student> UpdateAsync(Student student);
        Task<Student> DeleteAsync(int id);
    }
}
