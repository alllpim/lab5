using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kindergarten.Models;
using Kindergarten.ViewModels.FilterViewModels;

namespace Kindergarten.ViewModels.Models
{
    public class GroupTypesViewModel
    {
        public IEnumerable<GroupType> GroupTypes { get; set; }

        public GroupType GroupType { get; set; }

        public PageViewModel PageViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public GroupTypesFilterViewModel GroupTypesFilterViewModel { get; set; }
    }
}
