using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIPERPUS.Models
{
    public class Peminjaman
    {
        public int id { get; set; }
        [Required]
        public int idbuku { get; set; }
        [Required]
        public int idmahasiswa { get; set; }
        [Required]
        public DateTime tgl_pinjam { get; set; }
        [Required]
        public DateTime tgl_pulang { get; set; }
        [Required]
        public int qty { get; set; }
        public int status_data { get; set; }
        public DateTime created_at { get; set; }
        public string? created_by { get; set; }
        public DateTime updated_at { get; set; }
        public string? updated_by { get; set; }

        public virtual Buku? Buku { get; set; }
        public virtual Mahasiswa? Mahasiswa { get; set; }
    }
}