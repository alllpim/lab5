using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kindergarten.Models;
using Kindergarten.ViewModels.FilterViewModels;

namespace Kindergarten.ViewModels.Models
{
    public class GroupsViewModel
    {
        public IEnumerable<Group> Groups { get; set; }

        public Group Group { get; set; }

        public PageViewModel PageViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public GroupsFilterViewModel GroupsFilterViewModel { get; set; }
    }
}
