using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.Data
{
    public class SkillPresetSheet : Sheet<int, SkillPresetSheet.Row>
    {
        public class Row : SheetRow
        {
            public int Stage { get; set; }
            public List<string> Skills { get; set; }
        }

        protected override Row CreateRow(string[] fields)
        {
            var row = new Row()
            {
                Id = int.Parse(fields[0]),
                Stage = int.Parse(fields[1]),
            };

            row.Skills = new List<string>();
            for (var i = 2; i < fields.Length; ++i)
            {
                row.Skills.Add(fields[i]); 
            }

            return row;
        }
    }
}
