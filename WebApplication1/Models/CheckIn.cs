using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class CheckIn
    {
        public int ID { get; set; }
        public string IDStudent { get; set; }

        public string NameStudent { get; set; }

        public string IDGiangVien { get; set; }
        public string IDSubjects { get; set; }
        public DateTime DateCheckIn { get; set; }
    }
}