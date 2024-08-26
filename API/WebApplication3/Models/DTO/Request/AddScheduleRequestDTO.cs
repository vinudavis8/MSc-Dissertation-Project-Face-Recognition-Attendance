namespace AttendanceApi.Models.DTO.Request
{
    public class AddScheduleRequestDTO
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public int CourseModuleId { get; set; }

    }
}
