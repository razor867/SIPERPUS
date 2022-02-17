using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIPERPUS.Models;

namespace SIPERPUS.ViewModel
{
    public class BukuViewModel
    {
        public List<Buku>? Bukus { get; set; }
        public List<string>? KategoriBukuName { get; set; }
        public string? Search { get; set; }
        public int KategoriBukuSelected { get; set; }
    }
}