using AttendanceApi.Models.Domain;

namespace AttendanceApi.Repository
{
    public interface IAttendanceRepository
    {
        Task<List<Attendance>> GetAllAsync(int id);
        Task<Attendance> CreateAsync(Attendance attendance);
        //Task<Attendance> UpdateAsync(Attendance attendance);
        //Task<Attendance> DeleteAsync(int id);
    }
}
