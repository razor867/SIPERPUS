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
    public class BukuController : Controller
    {
        private SIPERPUSContext _context;
        public BukuController(SIPERPUSContext context)
        {
            _context = context;
        }

        #region Buku
        public async Task<IActionResult> index(string Search, int KategoriBukuSelected)
        {
            var resKategoriBuku = from m in _context.KategoriBuku.Where(m => m.status_data == 1) select m;
            List<SelectListItem> dropdowns = new List<SelectListItem>();
            foreach (var item in resKategoriBuku)
            {
                dropdowns.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama });
            }

            var query = from m in _context.Buku.Include(x => x.KategoriBuku)
                        where m.status_data == 1
                        select m;
            if (!String.IsNullOrEmpty(Search))
            {
                query = query.Where(x => x.nama.Contains(Search) || x.pengarang.Contains(Search)).AsQueryable();
            }
            if (KategoriBukuSelected > 0)
            {
                query = query.Where(x => x.KategoriBuku.id == KategoriBukuSelected).AsQueryable();
            }
            var BukuVM = new BukuViewModel
            {
                Bukus = await query.ToListAsync(),
                KategoriBukuName = query.Select(x => x.KategoriBuku.nama).ToList(),
                KategoriBukuSelected = KategoriBukuSelected > 0 ? KategoriBukuSelected : 0
            };

            ViewData["title"] = "Buku";
            ViewData["title_page"] = "Data Buku";
            ViewBag.Alert = TempData["alert"] != null ? TempData["alert"] : "";
            ViewBag.kategoriBukus = dropdowns;

            return View(BukuVM);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buku = await _context.Buku.Include(x => x.KategoriBuku)
                .FirstOrDefaultAsync(m => m.id == id);
            if (buku == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Detail Buku";
            ViewData["title_page"] = "Detail Buku";
            ViewData["kategori_buku_name"] = buku.KategoriBuku.nama;
            return View(buku);
        }
        public IActionResult Create()
        {
            var resKategoriBuku = from m in _context.KategoriBuku.Where(m => m.status_data == 1) select m;
            List<SelectListItem> dropdowns = new List<SelectListItem>();
            foreach (var item in resKategoriBuku)
            {
                dropdowns.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama });
            }
            ViewData["title"] = "Add Buku";
            ViewData["title_page"] = "Add Buku";
            ViewBag.kategoriBukus = dropdowns;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,nama,penerbit,pengarang,tahun,kategori,lokasi_rak,stok")] Buku buku)
        {
            if (ModelState.IsValid)
            {
                buku.status_data = 1;
                buku.created_by = "Wahyu";
                buku.created_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                buku.updated_by = "Wahyu";
                buku.updated_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                _context.Add(buku);
                await _context.SaveChangesAsync();
                TempData["alert"] = CommonMethod.Alert("success", "Berhasil menyimpan data");

                return RedirectToAction(nameof(Index));
            }
            return View(buku);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buku = await _context.Buku.FindAsync(id);
            if (buku == null)
            {
                return NotFound();
            }

            var resKategoriBuku = from m in _context.KategoriBuku.Where(m => m.status_data == 1) select m;
            List<SelectListItem> dropdowns = new List<SelectListItem>();
            foreach (var item in resKategoriBuku)
            {
                dropdowns.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama });
            }
            ViewBag.kategoriBukus = dropdowns;

            ViewData["title"] = "Edit Buku";
            ViewData["title_page"] = "Edit Buku";
            return View(buku);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,nama,penerbit,pengarang,tahun,kategori,lokasi_rak,stok")] Buku buku)
        {
            if (id != buku.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var dataBuku = await _context.Buku.FindAsync(id);
                try
                {
                    dataBuku.nama = buku.nama;
                    dataBuku.penerbit = buku.penerbit;
                    dataBuku.pengarang = buku.pengarang;
                    dataBuku.tahun = buku.tahun;
                    dataBuku.kategori = buku.kategori;
                    dataBuku.lokasi_rak = buku.lokasi_rak;
                    dataBuku.stok = buku.stok;
                    dataBuku.updated_by = "Wahyu";
                    dataBuku.updated_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                    
                    _context.Update(dataBuku);
                    await _context.SaveChangesAsync();
                    TempData["alert"] = CommonMethod.Alert("success", "Berhasil merubah data");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BukuExist(buku.id))
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
            return View(buku);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buku = await _context.Buku.Include(x => x.KategoriBuku)
                .FirstOrDefaultAsync(m => m.id == id);
            if (buku == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Delete Buku";
            ViewData["title_page"] = "Delete Buku";
            ViewData["kategori_buku_name"] = buku.KategoriBuku.nama;
            return View(buku);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var buku = await _context.Buku.FindAsync(id);
            buku.status_data = 0;
            _context.Update(buku);
            await _context.SaveChangesAsync();
            TempData["alert"] = CommonMethod.Alert("success", "Berhasil menghapus data");

            return RedirectToAction(nameof(Index));
        }
        private bool BukuExist(int id)
        {
            return _context.Buku.Any(e => e.id == id);
        }
        #endregion

        #region kategori buku
        public async Task<IActionResult> Kategori()
        {
            var query = from m in _context.KategoriBuku
                        where m.status_data == 1
                        select m;
            var KategoriBukuVM = new KategoriBukuViewModel
            {
                KategoriBukus = await query.ToListAsync()
            };
            ViewData["title"] = "Kategori Buku";
            ViewData["title_page"] = "Data Kategori Buku";
            ViewBag.Alert = TempData["alert"] != null ? TempData["alert"] : "";
            return View("/Views/KategoriBuku/Index.cshtml", KategoriBukuVM);
        }
        public async Task<IActionResult> KategoriDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategoriBuku = await _context.KategoriBuku
                .FirstOrDefaultAsync(m => m.id == id);
            if (kategoriBuku == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Detail Kategori Buku";
            ViewData["title_page"] = "Detail Kategori Buku";
            return View("/Views/KategoriBuku/Details.cshtml", kategoriBuku);
        }
        public IActionResult KategoriCreate()
        {
            ViewData["title"] = "Add Kategori Buku";
            ViewData["title_page"] = "Add Kategori Buku";
            return View("/Views/KategoriBuku/Create.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KategoriCreate([Bind("id,nama")] KategoriBuku kategoriBuku)
        {
            if (ModelState.IsValid)
            {
                kategoriBuku.status_data = 1;
                kategoriBuku.created_by = "Wahyu";
                kategoriBuku.created_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                kategoriBuku.updated_by = "Wahyu";
                kategoriBuku.updated_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                _context.Add(kategoriBuku);
                await _context.SaveChangesAsync();
                TempData["alert"] = CommonMethod.Alert("success", "Berhasil menyimpan data");

                return RedirectToAction(nameof(Kategori));
            }
            return View(kategoriBuku);
        }
        public async Task<IActionResult> KategoriEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategoriBuku = await _context.KategoriBuku.FindAsync(id);
            if (kategoriBuku == null)
            {
                return NotFound();
            }

            ViewData["title"] = "Edit Kategori Buku";
            ViewData["title_page"] = "Edit Kategori Buku";
            return View("/Views/KategoriBuku/Edit.cshtml", kategoriBuku);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KategoriEdit(int id, [Bind("id,nama")] KategoriBuku kategoriBuku)
        {
            if (id != kategoriBuku.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var dataKategoriBuku = await _context.KategoriBuku.FindAsync(id);
                try
                {
                    dataKategoriBuku.nama = kategoriBuku.nama;
                    dataKategoriBuku.updated_by = "Wahyu";
                    dataKategoriBuku.updated_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                    _context.Update(dataKategoriBuku);
                    TempData["alert"] = CommonMethod.Alert("success", "Berhasil merubah data");

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KategoriBukuExist(kategoriBuku.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Kategori));
            }
            return View(kategoriBuku);
        }
        public async Task<IActionResult> KategoriDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategoriBuku = await _context.KategoriBuku
                .FirstOrDefaultAsync(m => m.id == id);
            if (kategoriBuku == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Delete Kategori Buku";
            ViewData["title_page"] = "Delete Kategori Buku";
            return View("/Views/KategoriBuku/Delete.cshtml", kategoriBuku);
        }
        [HttpPost, ActionName("KategoriDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KategoriDeleteConfirmed(int id)
        {
            var kategoriBuku = await _context.KategoriBuku.FindAsync(id);
            kategoriBuku.status_data = 0;
            _context.Update(kategoriBuku);
            await _context.SaveChangesAsync();
            TempData["alert"] = CommonMethod.Alert("success", "Berhasil menghapus data");

            return RedirectToAction(nameof(Kategori));
        }
        private bool KategoriBukuExist(int id)
        {
            return _context.Buku.Any(e => e.id == id);
        }
        #endregion
    }
}