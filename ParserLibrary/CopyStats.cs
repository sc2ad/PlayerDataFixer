using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLibrary
{
    public interface CopyStats
    {
        void DisplayStats(Action<string> logCall);
    }
}
