using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flawless.Data
{
    public class SkillSheet : Sheet<string, SkillSheet.Row>
    {
        public class Row : SheetRow
        {
            public int Speed { get; set; }
            public double ATKCoefficient { get; set; }
            public double DEXCoefficient { get; set; }
            public double INTCoefficient { get; set; }
            public PoseType FinishPose { get; set; }
            public List<PoseType> AvailablePoses { get; set; }
        }

        protected override Row CreateRow(string[] fields)
        {
            var row = new Row()
            {
                Id = fields[0],
                Speed = int.Parse(fields[1]),
                ATKCoefficient = double.Parse(fields[2]),
                DEXCoefficient = double.Parse(fields[3]),
                INTCoefficient = double.Parse(fields[4]),
                FinishPose = Enum.Parse<PoseType>(fields[5])
            };

            row.AvailablePoses = new List<PoseType>();
            for (var i = 6; i < fields.Length; ++i)
            {
                var type = Enum.Parse<PoseType>(fields[i]);
                row.AvailablePoses.Add(type); 
            }

            return row;
        }
    }
}
