using AttendanceApi.Models.Domain;
using AttendanceApi.Models.DTO;
using AttendanceApi.Models.DTO.Request;
using AutoMapper;
using System.Reflection;

namespace AttendanceApi.Mappings
{
    public class AutomapperProfiles:Profile
    {
        public AutomapperProfiles()
        {
                CreateMap<Department,DepartmentDTO>().ReverseMap();
            CreateMap<Attendance, AttendanceDTO>().ReverseMap();
            CreateMap<Course, AddCourseRequestDTO>().ReverseMap();
            CreateMap<Course, AddCourseRequestDTO>().ReverseMap();
            CreateMap<CourseModule, AddCourseModuleRequestDTO>().ReverseMap();
            CreateMap<Course, CourseDTO>().ReverseMap();
            CreateMap<Student, StudentDTO>().ReverseMap();
            CreateMap<Student, AddStudentRequestDTO>().ReverseMap();
            CreateMap<Attendance, AddAttendanceRequestDTO>().ReverseMap();
            CreateMap<CourseModule, AddModuleRequestDTO>().ReverseMap();
            CreateMap<CourseModule, CourseModuleDTO>().ReverseMap();
            CreateMap<Schedule, ScheduleDTO>().ReverseMap();
            CreateMap<Schedule, AddScheduleRequestDTO>().ReverseMap();


        }
    }
}
