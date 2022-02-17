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
    public class MahasiswaController : Controller
    {
        private SIPERPUSContext _context;
        public MahasiswaController(SIPERPUSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string Search)
        {
            var resMahasiswa = from m in _context.Mahasiswa.Include(x => x.Jurusan).Where(m => m.status_data == 1) select m;
            if (!String.IsNullOrEmpty(Search))
            {
                long numVal;
                bool isNumeric = long.TryParse(Search, out numVal);
                resMahasiswa = resMahasiswa.AsQueryable();
                if (numVal > 0)
                {
                    resMahasiswa = resMahasiswa.Where(x => x.npm == numVal);
                }
                else
                {
                    resMahasiswa = resMahasiswa.Where(x => x.Jurusan.nama.Contains(Search) || x.nama.Contains(Search)).AsQueryable();
                }
            }
            var mahasiswaVM = new MahasiswaViewModel
            {
                Mahasiswas = await resMahasiswa.ToListAsync(),
                JurusanName = resMahasiswa.Select(x => x.Jurusan.nama).ToList()
            };
            ViewData["title"] = "Mahasiswa";
            ViewData["title_page"] = "Data Mahasiswa";
            ViewBag.Alert = TempData["alert"] != null ? TempData["alert"] : "";
            return View(mahasiswaVM);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mahasiswa = await _context.Mahasiswa.Include(x => x.Jurusan)
                .FirstOrDefaultAsync(m => m.id == id);
            if (mahasiswa == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Detail Mahasiswa";
            ViewData["title_page"] = "Detail Mahasiswa";
            ViewData["jurusan_name"] = mahasiswa.Jurusan.nama;
            return View(mahasiswa);
        }
        public IActionResult Create()
        {
            var resJurusan = from m in _context.Jurusan.Where(m => m.status_data == 1) select m;
            List<SelectListItem> dropdowns = new List<SelectListItem>();
            foreach (var item in resJurusan)
            {
                dropdowns.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama });
            }
            ViewData["title"] = "Add Mahasiswa";
            ViewData["title_page"] = "Add Mahasiswa";
            ViewBag.jurusans = dropdowns;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,nama,jurusan,npm,alamat")] Mahasiswa mahasiswa)
        {
            if (ModelState.IsValid)
            {
                mahasiswa.status_data = 1;
                mahasiswa.created_by = "Wahyu";
                mahasiswa.created_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                mahasiswa.updated_by = "Wahyu";
                mahasiswa.updated_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                _context.Add(mahasiswa);
                await _context.SaveChangesAsync();
                TempData["alert"] = CommonMethod.Alert("success", "Berhasil menyimpan data");
                return RedirectToAction(nameof(Index));
            }
            return View(mahasiswa);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mahasiswa = await _context.Mahasiswa.FindAsync(id);
            if (mahasiswa == null)
            {
                return NotFound();
            }

            var resJurusan = from m in _context.Jurusan.Where(m => m.status_data == 1) select m;
            List<SelectListItem> dropdowns = new List<SelectListItem>();
            foreach (var item in resJurusan)
            {
                dropdowns.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama });
            }
            ViewBag.jurusans = dropdowns;

            ViewData["title"] = "Edit Mahasiswa";
            ViewData["title_page"] = "Edit Mahasiswa";
            return View(mahasiswa);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,nama,jurusan,npm,alamat")] Mahasiswa mahasiswa)
        {
            if (id != mahasiswa.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var dataMahasiswa = await _context.Mahasiswa.FindAsync(id);
                try
                {
                    dataMahasiswa.nama = mahasiswa.nama;
                    dataMahasiswa.jurusan = mahasiswa.jurusan;
                    dataMahasiswa.npm = mahasiswa.npm;
                    dataMahasiswa.alamat = mahasiswa.alamat;
                    dataMahasiswa.updated_by = "Wahyu";
                    dataMahasiswa.updated_at = CommonMethod.JakartaTimeZone(DateTime.Now);

                    _context.Update(dataMahasiswa);
                    TempData["alert"] = CommonMethod.Alert("success", "Berhasil merubah data");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MahasiswaExist(mahasiswa.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(mahasiswa);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mahasiswa = await _context.Mahasiswa.Include(x => x.Jurusan)
                .FirstOrDefaultAsync(m => m.id == id);
            if (mahasiswa == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Delete Mahasiswa";
            ViewData["title_page"] = "Delete Mahasiswa";
            ViewData["jurusan_name"] = mahasiswa.Jurusan.nama;
            return View(mahasiswa);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mahasiswa = await _context.Mahasiswa.FindAsync(id);
            mahasiswa.status_data = 0;
            _context.Update(mahasiswa);
            await _context.SaveChangesAsync();
            TempData["alert"] = CommonMethod.Alert("success", "Berhasil menghapus data");
            return RedirectToAction(nameof(Index));
        }
        private bool MahasiswaExist(int id)
        {
            return _context.Mahasiswa.Any(e => e.id == id);
        }
    }
}