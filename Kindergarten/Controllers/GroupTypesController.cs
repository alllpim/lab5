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
    public class GroupTypesController : Controller
    {
        private readonly kindergartenContext _context;
        private readonly CacheProvider _cache;
        private int pageSize = 10;
        private const string filterKey = "groupTypes";

        public GroupTypesController(kindergartenContext context, CacheProvider cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: GroupTypes
        public async Task<IActionResult> Index(SortState sortState, int page = 1)
        {
            GroupTypesFilterViewModel filter = HttpContext.Session.Get<GroupTypesFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new GroupTypesFilterViewModel
                {
                    Type = string.Empty
                };
                HttpContext.Session.Set(filterKey, filter);
            }

            string key = $"{typeof(GroupType).Name}-{page}-{sortState}-{filter.Type}";

            if (!_cache.TryGetValue(key, out GroupTypesViewModel model))
            {
                model = new GroupTypesViewModel();

                IQueryable<GroupType> groupTypes = GetSortedGroupTypes(sortState, filter.Type);

                int count = groupTypes.Count();

                model.PageViewModel = new PageViewModel(page, count, pageSize);

                model.GroupTypes = count == 0 ? new List<GroupType>() : groupTypes.Skip((model.PageViewModel.PageIndex - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.GroupTypesFilterViewModel = filter;

                _cache.Set(key, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(GroupTypesFilterViewModel filterModel, int page)
        {
            GroupTypesFilterViewModel filter = HttpContext.Session.Get<GroupTypesFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.Type = filterModel.Type;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        private IQueryable<GroupType> GetSortedGroupTypes(SortState sortState, string type)
        {
            IQueryable<GroupType> groupTypes = _context.GroupTypes.AsQueryable();

            switch (sortState)
            {
                case SortState.GtNameAsc:
                    groupTypes = groupTypes.OrderBy(x => x.NameOfType);
                    break;
                case SortState.GtNameDesc:
                    groupTypes = groupTypes.OrderByDescending(x => x.NameOfType);
                    break;
                case SortState.GtNoteAsc:
                    groupTypes = groupTypes.OrderBy(x => x.Note);
                    break;
                case SortState.GtNoteDesc:
                    groupTypes = groupTypes.OrderByDescending(x => x.Note);
                    break;
            }

            if (!string.IsNullOrEmpty(type))
            {
                groupTypes = groupTypes.Where(x => (x.NameOfType).Contains(type))
                    .AsQueryable();
            }

            return groupTypes;
        }
        // GET: GroupTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupType = await _context.GroupTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupType == null)
            {
                return NotFound();
            }

            return View(groupType);
        }

        // GET: GroupTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GroupTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameOfType,Note")] GroupType groupType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(groupType);
                await _context.SaveChangesAsync();
                _cache.Clean();
                return RedirectToAction(nameof(Index));
            }
            return View(groupType);
        }

        // GET: GroupTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupType = await _context.GroupTypes.FindAsync(id);
            if (groupType == null)
            {
                return NotFound();
            }
            return View(groupType);
        }

        // POST: GroupTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameOfType,Note")] GroupType groupType)
        {
            if (id != groupType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groupType);
                    await _context.SaveChangesAsync();
                    _cache.Clean();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupTypeExists(groupType.Id))
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
            return View(groupType);
        }

        // GET: GroupTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupType = await _context.GroupTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupType == null)
            {
                return NotFound();
            }

            return View(groupType);
        }

        // POST: GroupTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupType = await _context.GroupTypes.FindAsync(id);
            _context.GroupTypes.Remove(groupType);
            await _context.SaveChangesAsync();
            _cache.Clean();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupTypeExists(int id)
        {
            return _context.GroupTypes.Any(e => e.Id == id);
        }
    }
}
