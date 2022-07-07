using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.Data
{
    public class WeaponSheet : Sheet<int, WeaponSheet.Row>
    {
        public class Row : SheetRow
        {
            public int Grade { get; set; }
            public int Price { get; set; }
            public int HP { get; set; }
            public int Attack { get; set; }
            public int Defense { get; set; }
            public int Speed { get; set; }
            public int LifeStealPercentage { get; set; }
        }

        protected override Row CreateRow(string[] fields)
        {
            var row = new Row()
            {
                Id = int.Parse(fields[0]),
                Grade = int.Parse(fields[1]),
                Price = int.Parse(fields[2]),
                HP = int.Parse(fields[3]),
                Attack = int.Parse(fields[4]),
                Defense = int.Parse(fields[5]),
                Speed = int.Parse(fields[6]),
                LifeStealPercentage = int.Parse(fields[7]),
            };

            return row;
        }

    }
}