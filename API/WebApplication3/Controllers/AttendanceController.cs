using AttendanceApi.Models.Domain;
using AttendanceApi.Models.DTO;
using AttendanceApi.Models.DTO.Request;
using AttendanceApi.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAttendanceRepository attendanceRepository;

        public AttendanceController(IAttendanceRepository attendanceRepository, IMapper mapper)
        {
            this.mapper = mapper;
            this.attendanceRepository = attendanceRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAttendanceHistory(int studentId)
        {
            var attendance = await attendanceRepository.GetAllAsync(studentId);
            var attendanceDTO = mapper.Map<List<AttendanceDTO>>(attendance);
            return Ok(attendanceDTO);
        }
        [HttpPost]
        public async Task<IActionResult> MarkAttendance([FromBody] AddAttendanceRequestDTO addAttendanceRequestDTO)
        {
            addAttendanceRequestDTO.AttendanceDate = DateTime.Now;
            var attendance = mapper.Map<Attendance>(addAttendanceRequestDTO);
            var result = await attendanceRepository.CreateAsync(attendance);
            return Ok(attendance.Id);
        }
    }
}


