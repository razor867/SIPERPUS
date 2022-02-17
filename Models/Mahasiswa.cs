using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIPERPUS.Models
{
    public class Mahasiswa
    {
        public int id { get; set; }
        [Required]
        public string? nama { get; set; }
        [Required]
        public int jurusan { get; set; }
        [Required]
        public long npm { get; set; }
        [Required]
        public string? alamat { get; set; }
        public int status_data { get; set; }
        public DateTime created_at { get; set; }
        public string? created_by { get; set; }
        public DateTime updated_at { get; set; }
        public string? updated_by { get; set; }

        public virtual Jurusan? Jurusan { get; set; }
    }
}