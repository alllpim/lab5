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
    public class StaffController : Controller
    {
        private readonly kindergartenContext _context;
        private readonly CacheProvider _cache;
        private int pageSize = 10;
        private const string filterKey = "staff";

        public StaffController(kindergartenContext context, CacheProvider cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: Staff
        public async Task<IActionResult> Index(SortState sortState, int page = 1)
        {
            StaffFilterViewModel filter = HttpContext.Session.Get<StaffFilterViewModel>(filterKey);
            if (filter == null)
            {
                filter = new StaffFilterViewModel
                {
                    GroupName = string.Empty,
                    Info = string.Empty,
                    Reward = string.Empty
                };
                HttpContext.Session.Set(filterKey, filter);
            }

            string key = $"{typeof(Staff).Name}-{page}-{sortState}-{filter.GroupName}-{filter.Info}-{filter.Reward}";

            if (!_cache.TryGetValue(key, out StaffViewModel model))
            {
                model = new StaffViewModel();

                IQueryable<Staff> staff = GetSortedStaff(sortState, filter.GroupName, filter.Info, filter.Reward);

                int count = staff.Count();

                model.PageViewModel = new PageViewModel(page, count, pageSize);

                model.Staffs = count == 0 ? new List<Staff>() : staff.Skip((model.PageViewModel.PageIndex - 1) * pageSize).Take(pageSize).ToList();
                model.SortViewModel = new SortViewModel(sortState);
                model.StaffFilterViewModel = filter;

                _cache.Set(key, model);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(StaffFilterViewModel filterModel, int page)
        {
            StaffFilterViewModel filter = HttpContext.Session.Get<StaffFilterViewModel>(filterKey);
            if (filter != null)
            {
                filter.GroupName = filterModel.GroupName;
                filter.Info = filterModel.Info;
                filter.Reward = filterModel.Reward;

                HttpContext.Session.Remove(filterKey);
                HttpContext.Session.Set(filterKey, filter);
            }

            return RedirectToAction("Index", new { page });
        }

        private IQueryable<Staff> GetSortedStaff(SortState sortState, string name, string info, string reward)
        {
            IQueryable<Staff> staff = _context.Staff.Include(x => x.Position).AsQueryable();

            switch (sortState)
            {
                case SortState.StaffNameAsc:
                    staff = staff.OrderBy(x => x.FullName);
                    break;
                case SortState.StaffNameDesc:
                    staff = staff.OrderByDescending(x => x.FullName);
                    break;
                case SortState.StaffAdressAsc:
                    staff = staff.OrderBy(x => x.Adress);
                    break;
                case SortState.StaffAdressDesc:
                    staff = staff.OrderByDescending(x => x.Adress);
                    break;
                case SortState.StaffInfoAsc:
                    staff = staff.OrderBy(x => x.Info);
                    break;
                case SortState.StaffInfoDesc:
                    staff = staff.OrderByDescending(x => x.Info);
                    break;
                case SortState.StaffPhoneAsc:
                    staff = staff.OrderBy(x => x.Phone);
                    break;
                case SortState.StaffPhoneDesc:
                    staff = staff.OrderByDescending(x => x.Phone);
                    break;
                case SortState.StaffPosAsc:
                    staff = staff.OrderBy(x => x.Position.PositionName);
                    break;
                case SortState.StaffPosDesc:
                    staff = staff.OrderByDescending(x => x.Position.PositionName);
                    break;
                case SortState.StaffRewardAsc:
                    staff = staff.OrderBy(x => x.Reward);
                    break;
                case SortState.StaffRewardDesc:
                    staff = staff.OrderByDescending(x => x.Reward);
                    break;
            }

            if (!string.IsNullOrEmpty(name))
            {
                staff = staff.Where(x => (x.FullName).Contains(name))
                    .AsQueryable();
            }
            if (!string.IsNullOrEmpty(info))
            {
                staff = staff.Where(x => x.Info.Contains(info))
                    .AsQueryable();
            }
            if (!string.IsNullOrEmpty(reward))
            {
                staff = staff.Where(x => x.Reward.Contains(reward))
                    .AsQueryable();
            }

            return staff;
        }
        // GET: Staff/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff
                .Include(s => s.Position)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staff/Create
        public IActionResult Create()
        {
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "PositionName");
            return View();
        }

        // POST: Staff/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Adress,Phone,PositionId,Info,Reward")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staff);
                await _context.SaveChangesAsync();
                _cache.Clean();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Id", staff.PositionId);
            return View(staff);
        }

        // GET: Staff/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "PositionName", staff.PositionId);
            return View(staff);
        }

        // POST: Staff/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Adress,Phone,PositionId,Info,Reward")] Staff staff)
        {
            if (id != staff.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                    _cache.Clean();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.Id))
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
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Id", staff.PositionId);
            return View(staff);
        }

        // GET: Staff/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staff
                .Include(s => s.Position)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // POST: Staff/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            _cache.Clean();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(int id)
        {
            return _context.Staff.Any(e => e.Id == id);
        }
    }
}
