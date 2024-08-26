using AttendanceApi.Controllers;
using AttendanceApi.Models.Domain;
using AttendanceApi.Models.DTO;
using AttendanceApi.Models.DTO.Request;
using AttendanceApi.Repository;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Controllers
{
    [TestFixture]
    public class CourseControllerTests
    {
        private Mock<ICourseRepository> _courseRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private CourseController _controller;

        [SetUp]
        public void Setup()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _mapperMock = new Mock<IMapper>();
            _controller = new CourseController(_courseRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetAll_ShouldReturnOkResultWithCourses()
        {
            // Arrange
            var courses = new List<Course> { new Course { Id = 1, Name = "Course1" } };
            var courseDTOs = new List<CourseDTO> { new CourseDTO { Id = 1, Name = "Course1" } };

            _courseRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(courses);
            _mapperMock.Setup(m => m.Map<List<CourseDTO>>(courses)).Returns(courseDTOs);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(courseDTOs, okResult.Value);
        }

        [Test]
        public async Task Get_ShouldReturnOkResultWithCourse()
        {
            // Arrange
            var course = new Course { Id = 1, Name = "Course1" };
            var courseDTO = new CourseDTO { Id = 1, Name = "Course1" };

            _courseRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync(course);
            _mapperMock.Setup(m => m.Map<CourseDTO>(course)).Returns(courseDTO);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(courseDTO, okResult.Value);
        }

        [Test]
        public async Task Get_ShouldReturnNotFound_WhenCourseDoesNotExist()
        {
            // Arrange
            _courseRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync((Course)null);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task Create_ShouldReturnOkResultWithNewCourseId()
        {
            // Arrange
            var addCourseRequestDTO = new AddCourseRequestDTO { Name = "Course1", DepartmentId = 1 };
            var course = new Course { Id = 1, Name = "Course1", DepartmentId = 1 };

            _mapperMock.Setup(m => m.Map<Course>(addCourseRequestDTO)).Returns(course);
            _courseRepositoryMock.Setup(repo => repo.CreateAsync(course)).ReturnsAsync(course);

            // Act
            var result = await _controller.Create(addCourseRequestDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(course.Id, okResult.Value);
        }

        [Test]
        public async Task Delete_ShouldReturnOkResult_WhenCourseIsDeleted()
        {
            // Arrange
            var course = new Course { Id = 1, Name = "Course1" };
            _courseRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).ReturnsAsync(course);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task Delete_ShouldReturnNotFound_WhenCourseDoesNotExist()
        {
            // Arrange
            _courseRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).ReturnsAsync((Course)null);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }
    }
}
