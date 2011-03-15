using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DCF.Lib;
using DCF.Common;

namespace DCF.DemoRules.Test
{
    class CountinousThreadState
    {
        public CountinousThreadState(string[] args_p)
        {
            m_args = args_p;
        }

        public void thread_main()
        {
            try
            {
                DateTime timestamp = DateTime.MinValue;
                DatabaseCleaningManager dcm = new DatabaseCleaningManager();
                dcm.ParseArgs(m_args.Skip(1).ToArray());
                dcm.InitFlow();
                int iteration = 0;
                CleansingManager offline =
                    new CleansingManager(new OfflineCleaningRuleProvider(dcm.SqlUtils));
                while (m_exit_thread == false)
                {
                    Logger.DebugWrite(string.Format("Thread is going to sleep at {0}...", 
                        DateTime.Now.ToLongTimeString()));
                    Thread.Sleep(1000);
                    Logger.DebugWriteLine(string.Format("back at {0}", DateTime.Now.ToLongTimeString()));
                    if (m_exit_thread) break;

                    // always clean: offline.cleanData(null);

                    if (0 == iteration++ % 10) // 2 minutes
                    { // full cleanup

                        timestamp = DateTime.Now;
                        Logger.DebugWriteLine("Full cleaning");
                        offline.cleanData(null);
                    }
                    else
                    { // incremental cleanup
                        DateTime cur = DateTime.Now;
                        object res = dcm.SqlUtils.ExecuteScalar(string.Format(
                            "select count(*) from itemsmentions where time > timestamp('{0}')",
                            timestamp.ToString("s")));
                        long num = (long)res;
                        if (num > 0)
                        {
                            Logger.TraceWriteLine("Incremental cleaning");
                            timestamp = cur;
                            //CleansingManager online =
                            //    new CleansingManager(new OnlineCleaningRuleProvider(dcm.SqlUtils));
                            //online.cleanData(null);
                            offline.cleanData(null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DCF.Common.Logger.TraceWriteLine("Caught exception " + ex.Message);
            }
        }

        internal void StopThread()
        {
            m_exit_thread = true;
        }

        private bool m_exit_thread = false;
        private string[] m_args;
    }
}
