﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceRegisterMAUIapp.Models.Request
{
    public class TrainModelRequest
    {
        public List<string> Base64Image { get; set; }
        public string StudentId { get; set; }
    }
}
