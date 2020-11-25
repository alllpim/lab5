using System;
using System.Collections.Generic;

#nullable disable

namespace Kindergarten.Models
{
    public partial class Position
    {
        public Position()
        {
            Staff = new HashSet<Staff>();
        }

        public int Id { get; set; }
        public string PositionName { get; set; }

        public virtual ICollection<Staff> Staff { get; set; }
    }
}
