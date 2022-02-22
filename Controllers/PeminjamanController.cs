using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIPERPUS.Models;
using SIPERPUS.ViewModel;
using SIPERPUS.Helper;
using Newtonsoft.Json;

namespace SIPERPUS.Controllers
{
    public class PeminjamanController : Controller
    {
        private SIPERPUSContext _context;
        public PeminjamanController(SIPERPUSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> index()
        {
            var query = from m in _context.Peminjaman.Include(x => x.Buku).Include(x => x.Mahasiswa)
                        where m.status_data == 1
                        select m;

            var peminjamanVM = new PeminjamanViewModel
            {
                Peminjamans = await query.ToListAsync(),
                BukuName = query.Select(x => x.Buku.nama).ToList(),
                MahasiswaName = query.Select(x => x.Mahasiswa.nama).ToList()
            };

            ViewData["title"] = "Peminjaman";
            ViewData["title_page"] = "Data Peminjaman";
            ViewBag.Alert = TempData["alert"] != null ? TempData["alert"] : "";

            return View(peminjamanVM);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var peminjaman = await _context.Peminjaman.Include(x => x.Buku).Include(x => x.Mahasiswa)
                .FirstOrDefaultAsync(m => m.id == id);
            if (peminjaman == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Detail Peminjaman";
            ViewData["title_page"] = "Detail Peminjaman";
            ViewData["buku_name"] = peminjaman.Buku.nama;
            ViewData["mahasiswa_name"] = peminjaman.Mahasiswa.nama;

            return View(peminjaman);
        }
        public IActionResult Create()
        {
            var resBuku = from m in _context.Buku.Where(m => m.status_data == 1) select m;
            var resMahasiswa = from m in _context.Mahasiswa.Where(m => m.status_data == 1) select m;

            List<SelectListItem> dropdownsBuku = new List<SelectListItem>();
            List<SelectListItem> dropdownsMhs = new List<SelectListItem>();
            foreach (var item in resBuku)
            {
                dropdownsBuku.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama + " (" + item.stok + ")" });
            }
            foreach (var item in resMahasiswa)
            {
                dropdownsMhs.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama });
            }

            ViewData["title"] = "Add Peminjaman";
            ViewData["title_page"] = "Add Peminjaman";
            ViewBag.dropdownBuku = dropdownsBuku;
            ViewBag.dropdownMhs = dropdownsMhs;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,idbuku,idmahasiswa,tgl_pinjam,tgl_pulang,qty")] Peminjaman peminjaman)
        {
            var buku = await _context.Buku.FindAsync(peminjaman.idbuku);
            bool err = false;

            if (!ModelState.IsValid)
            {
                err = true;
            }
            if (peminjaman.qty > buku.stok)
            {
                TempData["alert"] = CommonMethod.Alert("fail", "Stok buku tidak mencukupi!");
                err = true;
            }
            if (peminjaman.tgl_pulang <= peminjaman.tgl_pinjam)
            {
                TempData["alert"] = CommonMethod.Alert("fail", "Tanggal pulang harus lebih dari tanggal pinjam!");
                err = true;
            }

            if (!err)
            {
                buku.stok = buku.stok - peminjaman.qty;
                _context.Update(buku);
                if (await _context.SaveChangesAsync() > 0)
                {
                    peminjaman.status_data = 1;
                    peminjaman.created_by = "Wahyu";
                    peminjaman.created_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                    peminjaman.updated_by = "Wahyu";
                    peminjaman.updated_at = CommonMethod.JakartaTimeZone(DateTime.Now);
                    _context.Add(peminjaman);
                    await _context.SaveChangesAsync();
                    TempData["alert"] = CommonMethod.Alert("success", "Berhasil menyimpan data");
                }
            }
            else
            {
                var resBuku = from m in _context.Buku.Where(m => m.status_data == 1) select m;
                var resMahasiswa = from m in _context.Mahasiswa.Where(m => m.status_data == 1) select m;

                List<SelectListItem> dropdownsBuku = new List<SelectListItem>();
                List<SelectListItem> dropdownsMhs = new List<SelectListItem>();
                foreach (var item in resBuku)
                {
                    dropdownsBuku.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama + " (" + item.stok + ")" });
                }
                foreach (var item in resMahasiswa)
                {
                    dropdownsMhs.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama });
                }

                ViewData["title"] = "Add Peminjaman";
                ViewData["title_page"] = "Add Peminjaman";
                ViewBag.dropdownBuku = dropdownsBuku;
                ViewBag.dropdownMhs = dropdownsMhs;
                ViewBag.Alert = TempData["alert"] != null ? TempData["alert"] : "";
            }

            return err ? View(peminjaman) : RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var peminjaman = await _context.Peminjaman.FindAsync(id);
            if (peminjaman == null)
            {
                return NotFound();
            }

            var resBuku = from m in _context.Buku.Where(m => m.status_data == 1) select m;
            var resMahasiswa = from m in _context.Mahasiswa.Where(m => m.status_data == 1) select m;

            List<SelectListItem> dropdownsBuku = new List<SelectListItem>();
            List<SelectListItem> dropdownsMhs = new List<SelectListItem>();
            foreach (var item in resBuku)
            {
                dropdownsBuku.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama + " (" + item.stok + ")" });
            }
            foreach (var item in resMahasiswa)
            {
                dropdownsMhs.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama });
            }
            ViewBag.dropdownBuku = dropdownsBuku;
            ViewBag.dropdownMhs = dropdownsMhs;

            ViewData["title"] = "Edit Peminjaman";
            ViewData["title_page"] = "Edit Peminjaman";
            return View(peminjaman);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,idbuku,idmahasiswa,tgl_pinjam,tgl_pulang,qty")] Peminjaman peminjaman)
        {
            var buku = await _context.Buku.FindAsync(peminjaman.idbuku);
            var dataPeminjaman = await _context.Peminjaman.FindAsync(id);
            bool err = false;
            bool end = false;

            do
            {
                if (id != peminjaman.id)
                {
                    TempData["alert"] = CommonMethod.Alert("fail", "Terjadi kesalahan tidak dapat memproses data!");
                    err = true;
                    break;
                }
                if (!ModelState.IsValid)
                {
                    TempData["alert"] = CommonMethod.Alert("fail", "Terjadi kesalahan tidak dapat memproses data!");
                    err = true;
                    break;
                }
                if (peminjaman.qty == 0)
                {
                    TempData["alert"] = CommonMethod.Alert("fail", "Qty tida boleh 0!");
                    err = true;
                    break;
                }
                if (peminjaman.tgl_pulang <= peminjaman.tgl_pinjam)
                {
                    TempData["alert"] = CommonMethod.Alert("fail", "Tanggal pulang harus lebih dari tanggal pinjam!");
                    err = true;
                    break;
                }

                if (dataPeminjaman.idbuku != peminjaman.idbuku)
                {
                    if (peminjaman.qty > buku.stok)
                    {
                        TempData["alert"] = CommonMethod.Alert("fail", "Stok buku tidak cukup!");
                        err = true;
                        break;
                    }
                    else
                    {
                        var bukuLama = await _context.Buku.FindAsync(dataPeminjaman.idbuku);
                        bukuLama.stok = bukuLama.stok + dataPeminjaman.qty;
                        _context.Update(bukuLama);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            buku.stok = buku.stok - peminjaman.qty;
                            _context.Update(buku);
                        }
                        
                    }
                } else
                {
                    int selisihQty = peminjaman.qty > dataPeminjaman.qty ? peminjaman.qty - dataPeminjaman.qty : dataPeminjaman.qty - peminjaman.qty;
                    if (selisihQty > buku.stok)
                    {
                        TempData["alert"] = CommonMethod.Alert("fail", "Stok buku tidak cukup!");
                        err = true;
                        break;
                    }
                    else
                    {
                        buku.stok = peminjaman.qty > dataPeminjaman.qty ? buku.stok - selisihQty : buku.stok + selisihQty;
                    }
                    _context.Update(buku);
                }
                end = true;
            } while (!end);

            if (!err)
            {
                if (await _context.SaveChangesAsync() > 0)
                {
                    dataPeminjaman.idbuku = peminjaman.idbuku;
                    dataPeminjaman.idmahasiswa = peminjaman.idmahasiswa;
                    dataPeminjaman.tgl_pinjam = peminjaman.tgl_pinjam;
                    dataPeminjaman.tgl_pulang = peminjaman.tgl_pulang;
                    dataPeminjaman.qty = peminjaman.qty;
                    dataPeminjaman.updated_by = "Wahyu";
                    dataPeminjaman.updated_at = CommonMethod.JakartaTimeZone(DateTime.Now);

                    _context.Update(dataPeminjaman);
                    await _context.SaveChangesAsync();
                    TempData["alert"] = CommonMethod.Alert("success", "Berhasil merubah data");
                }
            }
            else
            {
                var resBuku = from m in _context.Buku.Where(m => m.status_data == 1) select m;
                var resMahasiswa = from m in _context.Mahasiswa.Where(m => m.status_data == 1) select m;

                List<SelectListItem> dropdownsBuku = new List<SelectListItem>();
                List<SelectListItem> dropdownsMhs = new List<SelectListItem>();
                foreach (var item in resBuku)
                {
                    dropdownsBuku.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama + " (" + item.stok + ")" });
                }
                foreach (var item in resMahasiswa)
                {
                    dropdownsMhs.Add(new SelectListItem { Value = item.id.ToString(), Text = item.nama });
                }
                ViewBag.dropdownBuku = dropdownsBuku;
                ViewBag.dropdownMhs = dropdownsMhs;

                ViewData["title"] = "Edit Peminjaman";
                ViewData["title_page"] = "Edit Peminjaman";
                ViewBag.Alert = TempData["alert"] != null ? TempData["alert"] : "";
            }

            return err ? View(peminjaman) : RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var peminjaman = await _context.Peminjaman.Include(x => x.Buku).Include(x => x.Mahasiswa)
                .FirstOrDefaultAsync(m => m.id == id);
            if (peminjaman == null)
            {
                return NotFound();
            }
            ViewData["title"] = "Delete Peminjaman";
            ViewData["title_page"] = "Delete Peminjaman";
            ViewData["buku_name"] = peminjaman.Buku.nama;
            ViewData["mahasiswa_name"] = peminjaman.Mahasiswa.nama;

            return View(peminjaman);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var peminjaman = await _context.Peminjaman.FindAsync(id);
            peminjaman.status_data = 0;
            _context.Update(peminjaman);
            await _context.SaveChangesAsync();
            TempData["alert"] = CommonMethod.Alert("success", "Berhasil menghapus data");

            return RedirectToAction(nameof(Index));
        }
        private bool PeminjamanExist(int id)
        {
            return _context.Peminjaman.Any(e => e.id == id);
        }
    }
}