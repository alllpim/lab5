using System;
using System.Collections.Generic;

#nullable disable

namespace Kindergarten.Models
{
    public partial class Parent
    {
        public Parent()
        {
            Children = new HashSet<Child>();
        }

        public int Id { get; set; }
        public string Mfullname { get; set; }
        public string Ffullname { get; set; }

        public virtual ICollection<Child> Children { get; set; }
    }
}
