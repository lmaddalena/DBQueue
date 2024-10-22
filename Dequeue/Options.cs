using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dequeue
{
    internal class Options
    {
        public string? Tag { get; set; } = null;
        public int NumberOfMessages { get; set; } = -1;
        public bool KeepJournal { get; set; }
    }
}
