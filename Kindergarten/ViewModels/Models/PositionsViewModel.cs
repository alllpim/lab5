using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kindergarten.Models;
using Kindergarten.ViewModels.FilterViewModels;

namespace Kindergarten.ViewModels.Models
{
    public class PositionsViewModel
    {
        public IEnumerable<Position> Positions { get; set; }

        public Position Position { get; set; }

        public PageViewModel PageViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public PositionsFilterViewModel PositionsFilterViewModel { get; set; }
    }
}
