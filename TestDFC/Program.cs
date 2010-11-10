using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using DCF.Common;
using DCF.Lib;

namespace TestDFC
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            Logger.TraceWriteLine(String.Format("Starting DFC test at {0}", DateTime.Now));
            List<ITestRuleSupplier> testTypesList = InitTest();
            RunTests(testTypesList);
            Logger.TraceWriteLine(string.Format("Finished DFC test at {0}", DateTime.Now));
        }

        private static void RunTests(List<ITestRuleSupplier> testTypesList)
        {
            foreach (ITestRuleSupplier ts in testTypesList)
            {
                Logger.TraceWriteLine(string.Format("Running cleaning for {0} tests", ts.Name));
                Logger.TraceIndent();
                CleansingManager cm = new CleansingManager(ts);
                cm.cleanData(null);
                if (ts.TestCurrentState())
                {
                    Logger.TraceWriteLine("The test suceeded.");
                }
                else
                {
                    Logger.TraceWriteLine("The test failed.");
                }
                Logger.TraceUnindent();
            }
        }

        private static List<ITestRuleSupplier> InitTest()
        {
            CleaningConfiguration.PopulateFromAppConfig();

            Logger.TraceWriteLine("Tests with TestMaxIterationSupplier");
            List<ITestRuleSupplier> list = new List<ITestRuleSupplier>();
            list.Add(new TestMaxIterationSupplier());
            return list;
        }
    }
}
