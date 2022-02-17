using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIPERPUS.Models;

namespace SIPERPUS.ViewModel
{
    public class MahasiswaViewModel
    {
        public List<Mahasiswa>? Mahasiswas { get; set; }
        public List<string>? JurusanName { get; set; }
        public string? Search { get; set; }
    }
}