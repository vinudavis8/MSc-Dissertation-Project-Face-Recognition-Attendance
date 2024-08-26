using AttendanceApi.Models.Domain;
using AttendanceApi.Models.DTO.Request;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceApi.Repository
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync();
        Task<Department> GetAsync(int id);
        Task<Department> CreateAsync(Department department);
        Task<Department> UpdateAsync(Department department);
        Task<Department> DeleteAsync(int id);

    }
}
