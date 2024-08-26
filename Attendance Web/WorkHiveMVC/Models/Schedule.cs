namespace Models.Domain
{
    public class Schedule
    {
        public int Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public int CourseModuleId { get; set; }

        //navigation property
        public CourseModule CourseModule { get; set; }

    }
}
