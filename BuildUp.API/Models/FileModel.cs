using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models
{
    public class FileModel
    {
        [Required]
        public string Filename { get; set; }

        [Required]
        public byte[] Data { get; set; }
    }
}
