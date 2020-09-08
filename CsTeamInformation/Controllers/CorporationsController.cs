using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CsTeamInformation;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;

namespace CsTeamInformation.Controllers
{
    public class CorporationsController : Controller
    {
        private readonly MyFirstDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CorporationsController(MyFirstDBContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Corporations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Corporations.ToListAsync());
        }

        // GET: Corporations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var corporations = await _context.Corporations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (corporations == null)
            {
                return NotFound();
            }

            return View(corporations);
        }

        // GET: Corporations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Corporations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImageFile")] Corporations corporations)
        {
            if (ModelState.IsValid)
            {
                string wwwrootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(corporations.ImageFile.FileName);
                string extension = Path.GetExtension(corporations.ImageFile.FileName);
                corporations.Emblem = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwrootPath + "/Image/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await corporations.ImageFile.CopyToAsync(fileStream);
                }


                _context.Add(corporations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(corporations);
        }
        // GET: Corporations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var corporations = await _context.Corporations.FindAsync(id);
            if (corporations == null)
            {
                return NotFound();
            }
            return View(corporations);
        }

        // POST: Corporations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Emblem,ImageFile")] Corporations corporations)
        {
            if (id != corporations.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (corporations.ImageFile != null) 
                    {
                        var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Image", corporations.Emblem);
                        if (System.IO.File.Exists(imagePath))
                            System.IO.File.Delete(imagePath);

                        string wwwrootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(corporations.ImageFile.FileName);
                        string extension = Path.GetExtension(corporations.ImageFile.FileName);
                        corporations.Emblem = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwrootPath + "/Image/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await corporations.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                    _context.Update(corporations);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CorporationsExists(corporations.Id))
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
            return View(corporations);
        }

        // GET: Corporations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var corporations = await _context.Corporations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (corporations == null)
            {
                return NotFound();
            }

            return View(corporations);
        }

        // POST: Corporations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var corporations = await _context.Corporations.FindAsync(id);


            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", corporations.Emblem);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            var teams = from t in _context.Teams
                         where t.CorporationId == id
                         select t;
            foreach (var t in teams)
            {
                var players = from p in _context.Players
                              where p.TeamId == t.Id
                              select p;
                foreach (var p in players)
                {
                    var imagePathP = Path.Combine(_hostEnvironment.WebRootPath, "photo", p.Photo);
                    if (System.IO.File.Exists(imagePathP))
                        System.IO.File.Delete(imagePathP);

                    _context.Players.Remove(p);
                }
                var imagePathT = Path.Combine(_hostEnvironment.WebRootPath, "icon", t.Icon);
                if (System.IO.File.Exists(imagePathT))
                    System.IO.File.Delete(imagePathT);

                _context.Teams.Remove(t);
            }

            await _context.SaveChangesAsync();

            _context.Corporations.Remove(corporations);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CorporationsExists(int id)
        {
            return _context.Corporations.Any(e => e.Id == id);
        }
    }
}
