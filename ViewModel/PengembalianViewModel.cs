using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIPERPUS.Models;

namespace SIPERPUS.ViewModel
{
    public class PengembalianViewModel
    {
        public List<Peminjaman>? Pengembalians { get; set; }
        public List<string>? BukuName { get; set; }
        public List<string>? MahasiswaName { get; set; }
        public string? Search { get; set; }
    }
}