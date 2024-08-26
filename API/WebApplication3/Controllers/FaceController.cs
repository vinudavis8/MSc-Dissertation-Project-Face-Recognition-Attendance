using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.AspNetCore.Mvc;
using Emgu.CV.Face;
using FaceClass;
using Azure.Core;
using System.Diagnostics;
using AttendanceApi.Models.Domain;
using AttendanceApi.Models.DTO.Request;
using AttendanceApi.Repository;
using AutoMapper;
using AttendanceApi.Models.DTO;
using WebApplication3.Models.DTO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using Bogus;

namespace WebApplication3.Controllers
{
    public class TrainModelRequest
    {
        public List<string> Base64Image { get; set; }
        public string StudentId { get; set; }
    }
    [ApiController]
    [Route("api/[controller]")]
    public class FaceController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<FaceController> _logger;
        private readonly IMapper mapper;
        private readonly IAttendanceRepository attendanceRepository;
        private readonly ICourseModuleRepository moduleRepository;
        private readonly IStudentRepository studentRepository;
        private readonly ICourseRepository courseRepository;


        public FaceController(ILogger<FaceController> logger, IAttendanceRepository attendanceRepository, IMapper mapper, ICourseModuleRepository moduleRepository, IStudentRepository studentRepository,ICourseRepository courseRepository)
        {
            _logger = logger;
            this.mapper = mapper;
            this.attendanceRepository = attendanceRepository;
            this.moduleRepository = moduleRepository;
            this.studentRepository = studentRepository;
            this.courseRepository = courseRepository;
        }

        [HttpPost("AutoTrainModel")]
        public async Task<IActionResult> AutoTrainModel()
        {
            var stopwatch = Stopwatch.StartNew();
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string trainingSet = Path.Combine(assemblyDirectory, CConfig.TrainingSetPath);

            List<string> base64imageList= new List<string>();
            LBPHFace lbphf = new LBPHFace();
            string previousId = "000";
            int conter = 1;
            var fileRepo = Directory.GetFiles(trainingSet);
            foreach (var file in fileRepo)
            {
                string fileName = Path.GetFileName(file);
                // Convert file to Base64 string
                string base64String = ConvertFileToBase64(file);

                if (fileName.Contains("608"))
                {
                    int is0 = 0;
                }
               // byte[] fileBytes = File.ReadAllBytes(file);
               //  Convert.ToBase64String(fileBytes);
                var digitsOnly = new Regex(@"\D+");
                string id = digitsOnly.Replace(fileName, "");
               // if(id == previousId)
                //{
                    base64imageList.Add(base64String);
               // }
                //else
                //{

                    Student student = await GenerateRandomStudent();
                    var result = await studentRepository.CreateAsync(student);

                    foreach (string str in base64imageList)
                    {
                        byte[] imageData = Convert.FromBase64String(str);
                        Image<Bgr, byte> bgrFrame;
                        using (Mat imageMat = new Mat())
                        {
                            CvInvoke.Imdecode(imageData, ImreadModes.Color, imageMat);
                            bgrFrame = imageMat.ToImage<Bgr, byte>();
                        }
                        lbphf.TrainModel(student.Id.ToString(), bgrFrame);
                    }

                    //clear list and add new 
                    base64imageList.Clear();
                    //base64imageList.Add(base64String);
                   // previousId = id;
               // }

                conter++;
            }
            stopwatch.Stop();
            var totalExecutionTime = stopwatch.Elapsed.TotalSeconds;
            string log = string.Format("{0} images trained in {1} seconds",conter-1, totalExecutionTime);
            return Ok(log);
        }

        private async Task<Student> GenerateRandomStudent()
        {
            var modules = await courseRepository.GetAllAsync();
           // var moduleDTO = mapper.Map<List<CourseDTO>>(modules);
            // Extract the CourseIds from the moduleDTO list
            var courseIds = modules.Select(m => m.Id).ToList();

            var faker = new Faker<Student>()
                .RuleFor(s => s.FirstName, f => f.Name.FirstName())
                .RuleFor(s => s.LastName, f => f.Name.LastName())
                .RuleFor(s => s.DateOfBirth, f => f.Date.Past(18, DateTime.Now.AddYears(-22))) // Assuming students are aged 18-22
                .RuleFor(s => s.Email, (f, s) => f.Internet.Email(s.FirstName, s.LastName))
                .RuleFor(s => s.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(s => s.Address, f => f.Address.FullAddress())
                .RuleFor(s => s.GradeLevel, f => f.Random.Int(6, 12)) // Assuming Grade Levels 1-12
                .RuleFor(s => s.CourseId, 1); // Assuming Course IDs 1-100

            return faker.Generate();
        }


        private string ConvertFileToBase64(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(fileBytes);
                }
            }
        }

        [HttpPost("Recognize")]
        public async Task<IActionResult> Recognize([FromBody] string base64Image)
        {
            try
            {
                VerificationResponseDTO response = new VerificationResponseDTO();
                byte[] imageData = Convert.FromBase64String(base64Image);
                //FaceClass.FaceClassR c = new FaceClass.FaceClassR();
                //string face = c.ProcessFrame(imageData);
                Image<Bgr, byte> bgrFrame;
                using (Mat imageMat = new Mat())
                {
                    CvInvoke.Imdecode(imageData, ImreadModes.Color, imageMat);
                    bgrFrame = imageMat.ToImage<Bgr, byte>();
                }
                LBPHFace lbphf = new LBPHFace();
                string label = lbphf.RecognizeFace(bgrFrame);
                response.Id = Convert.ToInt32(label);

                if (response.Id > 0)
                {
                    //get student details
                    Student student = await studentRepository.GetAsync(Convert.ToInt32(label));

                    //check timetable
                    var schedule = await moduleRepository.GetSheduleAsync(student.CourseId);
                    var currentTime = DateTime.Now;

                    
                    var currentSchedule = schedule.FirstOrDefault(schedule =>
                        schedule.From.AddMinutes(-30) <= currentTime && schedule.To >= currentTime);

                    if (currentSchedule != null)
                    {
                        Attendance addAttendance = new Attendance();
                        addAttendance.CourseModuleId = currentSchedule.CourseModuleId;
                        addAttendance.AttendanceDate = DateTime.Now;
                        addAttendance.StudentId = student.Id;
                        addAttendance.IsPresent = true;

                        var result = await attendanceRepository.CreateAsync(addAttendance);
                        response.Message = "Success";
                    }
                    else
                    {
                        response.Message = string.Format("Welcome {0} {1}. No Class Found to Register", student.FirstName, student.LastName);
                    }
                    var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var searchPattern = $"face_{label}_*.bmp";
                    var facePhotosPath = Path.Combine(assemblyDirectory, CConfig.FacePhotosPath);
                    var files = Directory.GetFiles(facePhotosPath, searchPattern);
                    if (files.Length > 0)
                    {
                       
                   
                    byte[] imageBytes = System.IO.File .ReadAllBytes(files[0]);
                    string base64String = Convert.ToBase64String(imageBytes);
                    response.Image = base64String;
                    }
                }
                else
                if (response.Id == 0)
                { response.Message = "Unknown Face Detected"; }
                else if (response.Id == -1)
                { response.Message = "No Face Detected"; }

                return Ok(response);
            }
            catch (Exception ex)
            {
                string errorLog = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}";
                System.IO.File.AppendAllText(CConfig.FaceListTextFile, errorLog + Environment.NewLine);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error RFecognizing model: {ex.StackTrace}");
            }
        }

        [HttpPost("TrainModel")]
        public IActionResult Train(TrainModelRequest request)
        {
            try
            {
                List<string> list = request.Base64Image;
                foreach (string str in list)
                {
                    byte[] imageData = Convert.FromBase64String(str);
                    //FaceClass.FaceClassR c = new FaceClass.FaceClassR();
                    //string face = c.ProcessFrame(imageData, request.StudentId);

                    Image<Bgr, byte> bgrFrame;
                    using (Mat imageMat = new Mat())
                    {
                        CvInvoke.Imdecode(imageData, ImreadModes.Color, imageMat);
                        bgrFrame = imageMat.ToImage<Bgr, byte>();
                    }
                    LBPHFace lbphf = new LBPHFace();
                    lbphf.TrainModel(request.StudentId, bgrFrame);
                }

                return Ok("success");
            }
            catch (Exception ex)
            {
                string errorLog = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}";
                System.IO.File.AppendAllText(CConfig.errorFilePath, errorLog + Environment.NewLine);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error training model: {ex.StackTrace}");
            }
        }
    }
}