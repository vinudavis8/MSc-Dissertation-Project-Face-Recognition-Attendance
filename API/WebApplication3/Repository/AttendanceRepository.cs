using AttendanceApi.Data;
using AttendanceApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApi.Repository
{
    public class AttendanceRepository:IAttendanceRepository
    {
        private readonly AttendanceDbContext dbContext;

        public AttendanceRepository(AttendanceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Attendance>> GetAllAsync(int studentId)
        {
            var query = from a in dbContext.Attendances
                        join s in dbContext.Schedule
                        on new { a.CourseModuleId, AttendanceDate = a.AttendanceDate }
                        equals new { s.CourseModuleId, AttendanceDate = s.From } into sa
                        from schedule in sa.DefaultIfEmpty()
                        where a.StudentId == studentId
                        select new Attendance
                        {
                            //AttendanceId = a.Id,
                            StudentId = a.StudentId,
                            CourseModuleId = a.CourseModuleId,
                            AttendanceDate = a.AttendanceDate,
                            IsPresent = a.IsPresent,
                            CourseModule = a.CourseModule,
                            //ScheduleFrom = schedule != null ? schedule.From : (DateTime?)null,
                            //ScheduleTo = schedule != null ? schedule.To : (DateTime?)null
                        };

            return await query.ToListAsync();
            //query.Include(x => x.CourseModule).ToListAsync();
            //return await dbContext.Attendances.Include((x => x.CourseModule)).Where(x => x.StudentId == studentId).ToListAsync();
        }
        
        public async Task<Attendance> CreateAsync(Attendance attendance)
        {
            dbContext.Attendances.Add(attendance);
            await dbContext.SaveChangesAsync();
            return attendance;
        }

    }
}
