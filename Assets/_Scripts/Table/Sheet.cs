using System;
using System.Collections.Generic;
using System.Linq;

namespace Flawless.Data
{
    public abstract class Sheet<K, V> : Dictionary<K, V> where V : Sheet<K, V>.SheetRow, new()
    {
        public abstract class SheetRow
        {
            public K Id { get; set; }
        }

        public virtual void Set(string csv)
        {
            var rows = csv.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.None)
                .Skip(1);

            foreach (var rowString in rows)
            {
                if (string.IsNullOrEmpty(rowString))
                    continue;

                var fields = GetFields(rowString);
                var row = CreateRow(fields);
                this[row.Id] = row;
            }
        }

        protected virtual string[] GetFields(string line)
        {
            return line.Split('\u002C');
        }

        protected abstract V CreateRow(string[] fields);
    }
}
