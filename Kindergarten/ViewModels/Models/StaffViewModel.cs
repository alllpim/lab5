using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kindergarten.Models;
using Kindergarten.ViewModels.FilterViewModels;

namespace Kindergarten.ViewModels.Models
{
    public class StaffViewModel
    {
        public IEnumerable<Staff> Staffs { get; set; }

        public Staff Staff { get; set; }

        public PageViewModel PageViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public StaffFilterViewModel StaffFilterViewModel { get; set; }
    }
}
