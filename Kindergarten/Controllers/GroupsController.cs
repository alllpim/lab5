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
    public class GroupsController : Controller
    {
        private readonly kindergartenContext _context;
        private readonly CacheProvider _cache;
        private int pageSize = 10;
        private const string filterKey = "groups";

        public GroupsController(kindergartenContext context, CacheProvider cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: Groups
        public async Task<IActionResult> Index(SortState sortState, int page = 1)
        {
            GroupsFilterViewModel filter = HttpContext.Session.Get<GroupsFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new GroupsFilterViewModel
                {
                    Name = string.Empty,
                    Year = null,
                    Count = null
                };
                HttpContext.Session.Set(filterKey, filter);
            }

            string key = $"{typeof(Group).Name}-{page}-{sortState}-{filter.Name}-{filter.Year}-{filter.Count}";

            if (!_cache.TryGetValue(key, out GroupsViewModel model))
            {
                model = new GroupsViewModel();

                IQueryable<Group> groups = GetSortedGroups(sortState, filter.Name, filter.Year, filter.Count);

                int count = groups.Count();

                model.PageViewModel = new PageViewModel(page, count, pageSize);

                model.Groups = count == 0 ? new List<Group>() : groups.Skip((model.PageViewModel.PageIndex - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.GroupsFilterViewModel = filter;

                _cache.Set(key, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(GroupsFilterViewModel filterModel, int page)
        {
            GroupsFilterViewModel filter = HttpContext.Session.Get<GroupsFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.Name = filterModel.Name;
                filter.Year = filterModel.Year;
                filter.Count = filterModel.Count;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        private IQueryable<Group> GetSortedGroups(SortState sortState, string name, int? year, int? count)
        {
            IQueryable<Group> groups = _context.Groups.Include(x => x.Type).Include(x => x.Staff).AsQueryable();

            switch (sortState)
            {
                case SortState.GroupNameAsc:
                    groups = groups.OrderBy(x => x.GroupName);
                    break;
                case SortState.GroupNameDesc:
                    groups = groups.OrderByDescending(x => x.GroupName);
                    break;
                case SortState.GroupCountAsc:
                    groups = groups.OrderBy(x => x.CountOfChildren);
                    break;
                case SortState.GroupCountDesc:
                    groups = groups.OrderByDescending(x => x.CountOfChildren);
                    break;
                case SortState.GroupStaffAsc:
                    groups = groups.OrderBy(x => x.Staff.FullName);
                    break;
                case SortState.GroupStaffDesc:
                    groups = groups.OrderByDescending(x => x.Staff.FullName);
                    break;
                case SortState.GroupTypeAsc:
                    groups = groups.OrderBy(x => x.Type.NameOfType);
                    break;
                case SortState.GroupTypeDesc:
                    groups = groups.OrderByDescending(x => x.Type.NameOfType);
                    break;
                case SortState.GroupYearAsc:
                    groups = groups.OrderBy(x => x.YearOfCreation);
                    break;
                case SortState.GroupYearDesc:
                    groups = groups.OrderByDescending(x => x.YearOfCreation);
                    break;
            }

            if (!string.IsNullOrEmpty(name))
            {
                groups = groups.Where(x => (x.Type.NameOfType).Contains(name))
                    .AsQueryable();
            }
            if (year != null)
            {
                groups = groups.Where(x => x.YearOfCreation == year)
                    .AsQueryable();
            }
            if (count != null)
            {
                groups = groups.Where(x => x.CountOfChildren == count)
                    .AsQueryable();
            }

            return groups;
        }
        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .Include(k => k.Staff)
                .Include(k => k.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "FullName");
            ViewData["TypeId"] = new SelectList(_context.GroupTypes, "Id", "NameOfType");
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GroupName,StaffId,CountOfChildren,YearOfCreation,TypeId")] Group @group)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@group);
                await _context.SaveChangesAsync();
                _cache.Clean();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Id", @group.StaffId);
            ViewData["TypeId"] = new SelectList(_context.GroupTypes, "Id", "Id", @group.TypeId);
            return View(@group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "FullName", @group.StaffId);
            ViewData["TypeId"] = new SelectList(_context.GroupTypes, "Id", "NameOFType", @group.TypeId);
            return View(@group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GroupName,StaffId,CountOfChildren,YearOfCreation,TypeId")] Group @group)
        {
            if (id != @group.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@group);
                    await _context.SaveChangesAsync();
                    _cache.Clean();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.Id))
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
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Id", @group.StaffId);
            ViewData["TypeId"] = new SelectList(_context.GroupTypes, "Id", "Id", @group.TypeId);
            return View(@group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .Include(k => k.Staff)
                .Include(k => k.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @group = await _context.Groups.FindAsync(id);
            _context.Groups.Remove(@group);
            await _context.SaveChangesAsync();
            _cache.Clean();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
