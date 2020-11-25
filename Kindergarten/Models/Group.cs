using System;
using System.Collections.Generic;

#nullable disable

namespace Kindergarten.Models
{
    public partial class Group
    {
        public Group()
        {
            Children = new HashSet<Child>();
        }

        public int Id { get; set; }
        public string GroupName { get; set; }
        public int? StaffId { get; set; }
        public int? CountOfChildren { get; set; }
        public int? YearOfCreation { get; set; }
        public int? TypeId { get; set; }

        public virtual Staff Staff { get; set; }
        public virtual GroupType Type { get; set; }
        public virtual ICollection<Child> Children { get; set; }
    }
}
