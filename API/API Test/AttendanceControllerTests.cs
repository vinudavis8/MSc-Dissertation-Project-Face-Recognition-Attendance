using AttendanceApi.Controllers;
using AttendanceApi.Models.Domain;
using AttendanceApi.Models.DTO;
using AttendanceApi.Models.DTO.Request;
using AttendanceApi.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Controllers
{
    public class AttendanceControllerTests
    {
        private Mock<IAttendanceRepository> _mockAttendanceRepository;
        private Mock<IMapper> _mockMapper;
        private AttendanceController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockAttendanceRepository = new Mock<IAttendanceRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new AttendanceController(_mockAttendanceRepository.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetAttendanceHistory_ReturnsOkResult_WithAttendanceDTOs()
        {
            // Arrange
            int studentId = 1;
            var attendances = new List<Attendance> { new Attendance { Id = 1, StudentId = studentId, AttendanceDate = DateTime.Now } };
            var attendanceDTOs = new List<AttendanceDTO> { new AttendanceDTO { Id = 1, StudentId = studentId } };

            _mockAttendanceRepository.Setup(repo => repo.GetAllAsync(studentId)).ReturnsAsync(attendances);
            _mockMapper.Setup(m => m.Map<List<AttendanceDTO>>(attendances)).Returns(attendanceDTOs);

            // Act
            var result = await _controller.GetAttendanceHistory(studentId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnValue = okResult.Value as List<AttendanceDTO>;
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(attendanceDTOs, returnValue);
        }

        [Test]
        public async Task MarkAttendance_ReturnsOkResult_WithNewAttendanceId()
        {
            // Arrange
            var addAttendanceRequestDTO = new AddAttendanceRequestDTO { StudentId = 1 };
            var attendance = new Attendance { Id = 1, StudentId = addAttendanceRequestDTO.StudentId, AttendanceDate = DateTime.Now };

            _mockMapper.Setup(m => m.Map<Attendance>(addAttendanceRequestDTO)).Returns(attendance);
            _mockAttendanceRepository.Setup(repo => repo.CreateAsync(attendance)).ReturnsAsync(attendance);

            // Act
            var result = await _controller.MarkAttendance(addAttendanceRequestDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnValue = okResult.Value as int?;
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(attendance.Id, returnValue);
        }
    }
}
