using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIPERPUS.Models;
using SIPERPUS.ViewModel;
using SIPERPUS.Helper;

namespace SIPERPUS.Controllers
{
    public class PengembalianController : Controller
    {
        private SIPERPUSContext _context;
        public PengembalianController(SIPERPUSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string Search)
        {
            var query = from m in _context.Peminjaman.Include(x => x.Buku).Include(x => x.Mahasiswa)
                        where m.status_data == 1 && m.status_kembali == 1
                        select m;
            if (!String.IsNullOrEmpty(Search))
            {
                query = query.Where(x => x.Buku.nama.Contains(Search) || x.Mahasiswa.nama.Contains(Search)).AsQueryable();
            }

            var PengembalianVM = new PengembalianViewModel
            {
                Pengembalians = await query.ToListAsync(),
                BukuName = query.Select(x => x.Buku.nama).ToList(),
                MahasiswaName = query.Select(x => x.Mahasiswa.nama).ToList()
            };

            ViewData["title"] = "Pengembalian";
            ViewData["title_page"] = "Data Pengembalian";
            ViewBag.Alert = TempData["alert"] != null ? TempData["alert"] : "";

            return View("/Views/Return/Index.cshtml", PengembalianVM);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pengembalian = await _context.Peminjaman.Include(x => x.Buku).Include(x => x.Mahasiswa)
                .FirstOrDefaultAsync(m => m.id == id);
            if (pengembalian == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Detail Pengembalian";
            ViewData["title_page"] = "Detail Pengembalian";
            ViewData["buku_name"] = pengembalian.Buku.nama;
            ViewData["mahasiswa_name"] = pengembalian.Mahasiswa.nama;

            return View("/Views/Return/Details.cshtml", pengembalian);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pengembalian = await _context.Peminjaman.Include(x => x.Buku).Include(x => x.Mahasiswa)
                .FirstOrDefaultAsync(m => m.id == id);
            if (pengembalian == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Delete Pengembalian";
            ViewData["title_page"] = "Delete Pengembalian";
            ViewData["buku_name"] = pengembalian.Buku.nama;
            ViewData["mahasiswa_name"] = pengembalian.Mahasiswa.nama;

            return View("/Views/Return/Delete.cshtml", pengembalian);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pengembalian = await _context.Peminjaman.FindAsync(id);
            pengembalian.status_data = 0;
            _context.Update(pengembalian);
            await _context.SaveChangesAsync();
            TempData["alert"] = CommonMethod.Alert("success", "Berhasil menghapus data");

            return RedirectToAction("Index", "Pengembalian");
        }
    }
}