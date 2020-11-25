using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kindergarten.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kindergarten.Data;
using Kindergarten.Infrastructure;
using Kindergarten.Models;
using Kindergarten.ViewModels;
using Kindergarten.ViewModels.FilterViewModels;
using Kindergarten.ViewModels.Models;
using Microsoft.AspNetCore.Authorization;

namespace Kindergarten.Controllers
{
    [Authorize]
    public class PositionsController : Controller
    {
        private readonly kindergartenContext _context;
        private readonly CacheProvider _cache;
        private int pageSize = 10;
        private const string filterKey = "Positions";

        public PositionsController(kindergartenContext context, CacheProvider cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: Positions
        public async Task<IActionResult> Index(SortState sortState, int page = 1)
        {
            PositionsFilterViewModel filter = HttpContext.Session.Get<PositionsFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new PositionsFilterViewModel
                {
                    Name = string.Empty
                };
                HttpContext.Session.Set(filterKey, filter);
            }

            string key = $"{typeof(Position).Name}-{page}-{sortState}-{filter.Name}";

            if (!_cache.TryGetValue(key, out PositionsViewModel model))
            {
                model = new PositionsViewModel();

                IQueryable<Position> positions = GetSortedPositions(sortState, filter.Name);

                int count = positions.Count();

                model.PageViewModel = new PageViewModel(page, count, pageSize);

                model.Positions = count == 0 ? new List<Position>() : positions.Skip((model.PageViewModel.PageIndex - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.PositionsFilterViewModel = filter;

                _cache.Set(key, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(PositionsFilterViewModel filterModel, int page)
        {
            PositionsFilterViewModel filter = HttpContext.Session.Get<PositionsFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.Name = filterModel.Name;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        private IQueryable<Position> GetSortedPositions(SortState sortState, string name)
        {
            IQueryable<Position> positions = _context.Positions.AsQueryable();

            switch (sortState)
            {
                case SortState.PosNameAsc:
                    positions = positions.OrderBy(x => x.PositionName);
                    break;
                case SortState.PosNameDesc:
                    positions = positions.OrderByDescending(x => x.PositionName);
                    break;
            }

            if (!string.IsNullOrEmpty(name))
            {
                positions = positions.Where(x => (x.PositionName).Contains(name))
                    .AsQueryable();
            }

            return positions;
        }
        // GET: Positions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // GET: Positions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Positions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PositionName")] Position position)
        {
            if (ModelState.IsValid)
            {
                _context.Add(position);
                await _context.SaveChangesAsync();
                _cache.Clean();
                return RedirectToAction(nameof(Index));
            }
            return View(position);
        }

        // GET: Positions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }
            return View(position);
        }

        // POST: Positions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PositionName")] Position position)
        {
            if (id != position.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(position);
                    await _context.SaveChangesAsync();
                    _cache.Clean();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionExists(position.Id))
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
            return View(position);
        }

        // GET: Positions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var position = await _context.Positions.FindAsync(id);
            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();
            _cache.Clean();
            return RedirectToAction(nameof(Index));
        }

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.Id == id);
        }
    }
}
