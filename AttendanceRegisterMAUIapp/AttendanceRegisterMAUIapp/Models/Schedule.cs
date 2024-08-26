using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceRegisterMAUIapp.Models
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
