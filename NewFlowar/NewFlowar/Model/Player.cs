using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewFlowar.Model;

namespace NewFlowar
{
    public class Player
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public float Speed { get; set; }
        public CellBase CellBase { get; set; }
        public int Experience { get; set; }
        public List<PlayerCard> ListCard { get; set; }
    }
}
