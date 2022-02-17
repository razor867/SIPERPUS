using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIPERPUS.Models
{
    public class KategoriBuku
    {
        public int id { get; set; }
        [Required]
        public string? nama { get; set; }
        public int status_data { get; set; }
        public DateTime created_at { get; set; }
        public string? created_by { get; set; }
        public DateTime updated_at { get; set; }
        public string? updated_by { get; set; }

        public virtual ICollection<Buku>? Buku { get; set; }
    }
}