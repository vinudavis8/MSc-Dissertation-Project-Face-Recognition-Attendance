using AttendanceApi.Data;
using AttendanceApi.Models.Domain;
using AttendanceApi.Models.DTO;
using AttendanceApi.Models.DTO.Request;
using AttendanceApi.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository departmentRepository;
        private readonly IMapper mapper;

        public DepartmentController(IDepartmentRepository repository, IMapper mapper)
        {
                this.departmentRepository = repository;
            this.mapper= mapper;
        }
        // GET: api/department/
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            
            var departments =await departmentRepository.GetAllAsync();
            //var departmentDTO = new List<DepartmentDTO>();
            //foreach (var department in departments)
            //{
            //    departmentDTO.Add(new DepartmentDTO { 
            //        Name = department.Name,
            //        Id = department.Id});
            //}
            var departmentDTO= mapper.Map<List<DepartmentDTO>>(departments);
            return Ok(departmentDTO);

        }
       

        // GET api/<DepartmentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Department department=await departmentRepository.GetAsync(id);
            if (department == null)
                return NotFound();

            //DepartmentDTO departmentDTO = new DepartmentDTO();
            //departmentDTO.Name = department.Name;
            //departmentDTO.Id = department.Id;
            var departmentDTO = mapper.Map<DepartmentDTO>(department);

            return Ok(departmentDTO);
        }

        // POST api/<DepartmentController>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddDepartmentRequestDTO addDepartmentRequestDTO)
        {
            Department department = new Department { Name = addDepartmentRequestDTO.Name };
            var result=await departmentRepository.CreateAsync(department);
            return Ok(department.Id);
        }

        // PUT api/<DepartmentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentRequestDTO updateDepartmentRequestDTO)
        {
            Department department=new Department { Id=id,Name=updateDepartmentRequestDTO.Name };
            var result = await departmentRepository.UpdateAsync(department);
            if (department == null)
                return NotFound();
            return Ok(department.Id);
        }

        // DELETE api/<DepartmentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Department department =await  departmentRepository.DeleteAsync(id);
            if (department == null)
                return NotFound(); 
            return Ok();
        }
    }
}
