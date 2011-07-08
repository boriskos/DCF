using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using DCF.Common;

namespace DCF.Lib
{
    public class CleansingManager
    {
        public CleansingManager(IRuleSupplier ruleSupplier)
        {
            m_ruleSupplier = ruleSupplier;
        }
        public void cleanData(IEnumerable<string> involvedTableNames)
        {
            m_stopSampling = false;
            m_dataIsClean = false;
            int prevIteration = CurrentIteration;
            for (int i = 0; i < CleaningConfiguration.Instance.MaxSampleIterations && !m_stopSampling; i++)
            {
                CurrentIteration = prevIteration + i;
                Logger.DebugWriteLine("Sample " + CurrentIteration.ToString(), Logger.CleaningDataStr);
                cleanDataSample(involvedTableNames);
                Logger.DebugWriteLine("");
                Logger.DebugFlush();
            }
            CurrentIteration = CurrentIteration + 1;
            Logger.TraceWriteLine(string.Format("The cleaning finished after {0} iterations", CurrentIteration - prevIteration));
        }

        public int CurrentIteration { get; private set; }

        #region Protected Methods
        protected void cleanDataSample(IEnumerable<string> involvedTableNames)
        {
            using (new PerformanceCounter("CleansingManager.cleanData"))
            {
                for (int i = 0;
                    i < CleaningConfiguration.Instance.MaxCleaningIterationsPerSample && !m_dataIsClean && !m_stopSampling;
                    i++)
                {
                    Logger.DebugWriteLine(string.Format("Cleaning iteration {0}", i), Logger.CleaningDataStr);
                    IEnumerable<Rule> rules = SelectRules(involvedTableNames);
                    if (rules == null || rules.Count()==0)
                    {
                        break;
                    }

                    ApplyRules(rules);
                    Logger.DebugWriteLine("");
                }
                CompleteCleaningCycle(m_dataIsClean, involvedTableNames);
            }
        }

        protected IEnumerable<Rule> SelectRules(IEnumerable<string> involvedTableNames)
        {
            if (m_cleaningRulesList == null)
            {
                m_cleaningRulesList = m_ruleSupplier.GetCleaningRules();

                if (m_cleaningRulesList != null && m_cleaningRulesList.Count != 0)
                {
                    Logger.DebugWriteLine("Selected cleaning rules:", Logger.CleaningDataStr);
                    Logger.DebugIndent();
                    using (new PerformanceCounter("All rules initiallization"))
                    {
                        foreach (Rule rule in m_cleaningRulesList)
                        {
                            rule.StopCleaningProcess += new EventHandler(SetStopSamping);
                            rule.DataIsClean += new Rule.RuleFinishedDelegate(SetDataIsCleanEventHandler);
                            using (new PerformanceCounter("Initializing rule " + rule.Id))
                            {
                                rule.init(null);
                            } 
                            Logger.DebugWriteLine(rule.Id); ;
                        }
                    }
                    Logger.DebugUnindent();
                }
                else
                {
                    Logger.DebugWriteLine("Selected no cleaning rules!", Logger.CleaningDataStr);
                }
            }
            return m_cleaningRulesList;
        }
        protected void ApplyRules(IEnumerable<Rule> rulesList)
        {
            using (new PerformanceCounter("All rules execution"))
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["CurrentIteration"] = new Func<int>(() => CurrentIteration);
                foreach (Rule r in rulesList)
                {
                    using (new PerformanceCounter("Execution of rule " + r.Id))
                    {
                        r.execute(data);
                    }
                }
            }
        }

        #endregion
        #region Private methods
        private void CompleteCleaningCycle(bool isDataClean, IEnumerable<string> involvedTableNames)
        {
            using (new PerformanceCounter("CleansingManager.CompleteCleaningCycle"))
            {
                m_dataIsClean = false;
                IEnumerable<Rule> rules = SelectRulesForCompletion(involvedTableNames);
                if (rules != null)
                {
                    ApplyRules(rules);
                }
            }
        }

        private IEnumerable<Rule> SelectRulesForCompletion(IEnumerable<string> involvedTableNames)
        {
            if (m_samplingRulesList == null)
            {
                m_samplingRulesList = m_ruleSupplier.GetSampleRules();
                if (m_samplingRulesList != null && m_samplingRulesList.Count!=0)
                {
                    Logger.DebugWriteLine("Selected sampling rules:", Logger.CleaningDataStr);
                    Logger.DebugIndent();
                    using (new PerformanceCounter("All rules initiallization"))
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        data["CurrentIteration"] = new Func<int>(() => CurrentIteration);

                        foreach (Rule rule in m_samplingRulesList)
                        {
                            rule.StopCleaningProcess += new EventHandler(SetStopSamping);
                            rule.DataIsClean += new Rule.RuleFinishedDelegate(SetDataIsCleanEventHandler);

                            using (new PerformanceCounter("Initializing rule " + rule.Id))
                            {
                                rule.init(data);
                            }
                            Logger.DebugWriteLine(rule.Id);
                        }
                    }
                    Logger.DebugUnindent();
                }
                else
                {
                    Logger.DebugWriteLine("Selected no sampling rules!", Logger.CleaningDataStr);
                }
            }

            return m_samplingRulesList;
        }
        

        private void SetDataIsCleanEventHandler(bool success, object data)
        {
            if (success && !m_dataIsClean) m_dataIsClean = true;
        }

        private void SetStopSamping(object sender, EventArgs e)
        {
            m_stopSampling = true;
        }

        #endregion Private methods

        #region Private members
        private bool m_dataIsClean = false;
        private IList<Rule> m_cleaningRulesList = null;
        private IList<Rule> m_samplingRulesList = null;
        private bool m_stopSampling = false;
        private IRuleSupplier m_ruleSupplier = null;
        #endregion
    }
}
