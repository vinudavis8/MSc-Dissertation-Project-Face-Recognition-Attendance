using AttendanceApi.Models.Domain;

namespace AttendanceApi.Models.DTO.Request
{
    public class AddAttendanceRequestDTO
    {
            public int StudentId { get; set; }
            public int CourseModuleId { get; set; }
            public bool IsPresent { get; set; }

            public DateTime AttendanceDate { get; set; }

        }
}
