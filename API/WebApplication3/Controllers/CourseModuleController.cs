using AttendanceApi.Data;
using AttendanceApi.Models.Domain;
using AttendanceApi.Models.DTO;
using AttendanceApi.Models.DTO.Request;
using AttendanceApi.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseModuleController : ControllerBase
    {
        private readonly ICourseModuleRepository moduleRepository;
        private readonly IMapper mapper;

        public CourseModuleController(ICourseModuleRepository repository, IMapper mapper)
        {
            this.moduleRepository = repository;
            this.mapper = mapper;
        }
        // GET: api/department/
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var modules = await moduleRepository.GetAllAsync();
            var moduleDTO = mapper.Map<List<CourseModuleDTO>>(modules);
            return Ok(moduleDTO);
        }


        // GET api/<DepartmentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
           CourseModule module = await moduleRepository.GetAsync(id);
            if (module == null)
                return NotFound();
            var moduleDTO = mapper.Map<CourseModuleDTO>(module);

            return Ok(moduleDTO);
        }

        [HttpGet("GetSchedule")]
        public async Task<IActionResult> GetSchedule(int courseId)
        {
            var result = await moduleRepository.GetSheduleAsync(courseId);
            var scheduleDTO = mapper.Map<List<ScheduleDTO>>(result);
            return Ok(scheduleDTO);
        }

        // POST api/<DepartmentController>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCourseModuleRequestDTO addModuleRequestDTO)
        {
            var module = mapper.Map<CourseModule>(addModuleRequestDTO);
            var result = await moduleRepository.CreateAsync(module);
            return Ok(module.Id);
        }


        [HttpPost("CreateSchedule")]
        public async Task<IActionResult> CreateSchedule([FromBody] AddScheduleRequestDTO addScheduleRequestDTO)
        {
            var module = mapper.Map<Schedule>(addScheduleRequestDTO);
            var result = await moduleRepository.CreateScheduleAsync(module);
            return Ok(module.Id);
        }

        //// PUT api/<DepartmentController>/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] UpdateModuleRequestDTO updateModuleRequestDTO)
        //{
        //    CourseModule module = new Department { Id = id, Name = updateModuleRequestDTO.Name };
        //    var result = await moduleRepository.UpdateAsync(module);
        //    if (module == null)
        //        return NotFound();
        //    return Ok(module.Id);
        //}

        // DELETE api/<DepartmentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            CourseModule module = await moduleRepository.DeleteAsync(id);
            if (module == null)
                return NotFound();
            return Ok();
        }
    }
}
