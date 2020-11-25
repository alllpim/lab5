using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kindergarten.ViewModels
{
    public class PageViewModel
    {
        public int PageIndex { get; set; }

        public int PageCount { get; set; }

        public PageViewModel(int page, int count, int pageSize = 10)
        {
            PageIndex = page;
            PageCount = (int)Math.Ceiling(count / (double)pageSize);

            if (PageIndex <= 0)
                PageIndex = 1;

            if (PageIndex > PageCount)
                PageIndex = PageCount;
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < PageCount);
            }
        }
    }
}
