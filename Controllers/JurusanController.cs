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
    public class JurusanController : Controller
    {
        private SIPERPUSContext _context;
        public JurusanController(SIPERPUSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var resJurusan = from m in _context.Jurusan.Where(m => m.status_data == 1) select m;
            var jurusanVM = new JurusanViewModel
            {
                Jurusans = await resJurusan.ToListAsync()
            };
            ViewData["title"] = "Jurusan";
            ViewData["title_page"] = "Data Jurusan";
            ViewBag.Alert = TempData["alert"] != null ? TempData["alert"] : "";
            return View(jurusanVM);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jurusan = await _context.Jurusan
                .FirstOrDefaultAsync(m => m.id == id);
            if (jurusan == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Detail Jurusan";
            ViewData["title_page"] = "Detail Jurusan";
            return View(jurusan);
        }
        public IActionResult Create()
        {
            ViewData["title"] = "Add Jurusan";
            ViewData["title_page"] = "Add Jurusan";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,nama")] Jurusan jurusan)
        {
            if (ModelState.IsValid)
            {
                jurusan.status_data = 1;
                jurusan.created_by = "Wahyu";
                jurusan.created_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                jurusan.updated_by = "Wahyu";
                jurusan.updated_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                _context.Add(jurusan);
                await _context.SaveChangesAsync();
                TempData["alert"] = CommonMethod.Alert("success", "Berhasil menyimpan data");
                return RedirectToAction(nameof(Index));
            }
            return View(jurusan);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jurusan = await _context.Jurusan.FindAsync(id);
            if (jurusan == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Edit Jurusan";
            ViewData["title_page"] = "Edit Jurusan";
            return View(jurusan);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,nama")] Jurusan jurusan)
        {
            if (id != jurusan.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var dataJurusan = await _context.Jurusan.FindAsync(id);
                try
                {
                    dataJurusan.nama = jurusan.nama;
                    dataJurusan.updated_by = "Wahyu";
                    dataJurusan.updated_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                    _context.Update(dataJurusan);
                    TempData["alert"] = CommonMethod.Alert("success", "Berhasil merubah data");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JurusanExist(jurusan.id))
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
            return View(jurusan);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jurusan = await _context.Jurusan
                .FirstOrDefaultAsync(m => m.id == id);
            if (jurusan == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Delete Jurusan";
            ViewData["title_page"] = "Delete Jurusan";
            return View(jurusan);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jurusan = await _context.Jurusan.FindAsync(id);
            jurusan.status_data = 0;
            _context.Update(jurusan);
            await _context.SaveChangesAsync();
            TempData["alert"] = CommonMethod.Alert("success", "Berhasil menghapus data");
            return RedirectToAction(nameof(Index));
        }
        private bool JurusanExist(int id)
        {
            return _context.Jurusan.Any(e => e.id == id);
        }
    }
}