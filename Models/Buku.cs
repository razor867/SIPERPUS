using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIPERPUS.Models
{
    public class Buku
    {
        public int id { get; set; }
        [Required]
        public string? nama { get; set; }
        [Required]
        public string? penerbit { get; set; }
        [Required]
        public string? pengarang { get; set; }
        [Required]
        public int tahun { get; set; }
        [Required]
        public int kategori { get; set; }
        [Required]
        public string? lokasi_rak { get; set; }
        [Required]
        public int stok { get; set; }
        public int status_data { get; set; }
        public DateTime created_at { get; set; }
        public string? created_by { get; set; }
        public DateTime updated_at { get; set; }
        public string? updated_by { get; set; }

        public virtual KategoriBuku? KategoriBuku { get; set; }
    }
}