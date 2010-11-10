using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.Lib;

namespace TestDFC
{
    interface ITestRuleSupplier : IRuleSupplier
    {
        bool TestCurrentState();
        string Name { get; }
    }
}
