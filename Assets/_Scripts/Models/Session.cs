using System.Collections.Generic;
using System.Linq;

namespace Flawless.Models
{
    public struct Session
    {
        public Section(IEnumerable<Stage> stages) => Stages = stages.ToList();

        public List<Stage> Stages { get; private set; }
    }
}
