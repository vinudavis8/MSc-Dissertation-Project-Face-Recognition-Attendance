using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceRegisterMAUIapp.Models
{

    public class VerificationRepsonse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ThumbnailImg> Thumbnails { get; set; }
    }
    public class VerificationResponseAPI
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }

    }
    public class ThumbnailImg
    {
        public string Id { get; set; }
        public string Thumbnail { get; set; }
    }
}
