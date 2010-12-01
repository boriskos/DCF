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
        public const int MaxSampleIterationsConst = 50;

        public CleansingManager(IRuleSupplier ruleSupplier)
        {
            m_ruleSupplier = ruleSupplier;
        }
        public void cleanData(IEnumerable<string> involvedTableNames)
        {
            for (int i = 0; i < CleaningConfiguration.Instance.MaxSampleIterations && !m_stopSampling; i++)
            {
                CurrentIteration = i;
                Logger.DebugWriteLine("Sample " + i.ToString(), Logger.CleaningDataStr);
                cleanDataSample(involvedTableNames);
                Logger.DebugWriteLine("");
                Logger.DebugFlush();
            }
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
                    if (rules == null)
                    {
                        Logger.DebugWriteLine("Selected no rules!", Logger.CleaningDataStr);
                        break;
                    }

                    Logger.DebugWriteLine("Selected rules:", Logger.CleaningDataStr);
                    Logger.DebugIndent();
                    foreach (var rule in rules) Logger.DebugWriteLine(rule.Id); ;
                    Logger.DebugUnindent();

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

                if (m_cleaningRulesList != null)
                {
                    foreach (Rule rule in m_cleaningRulesList)
                    {
                        rule.StopCleaningProcess += new EventHandler(SetStopSamping);
                        rule.DataIsClean += new Rule.RuleFinishedDelegate(SetDataIsCleanEventHandler);
                        rule.init(null);
                    }
                }
            }
            return m_cleaningRulesList;
        }
        protected void ApplyRules(IEnumerable<Rule> rulesList)
        {
            using (new PerformanceCounter("CleansingManager.ApplyRules"))
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["CheckCleanCitiesConsistency"] = new DCF.Lib.Rule.RuleFinishedDelegate(SetDataIsCleanEventHandler);
                data["CurrentIteration"] = new Func<int>(() => CurrentIteration);

                foreach (Rule r in rulesList)
                {
                    r.execute(data);
                }
            }
        }

        #endregion
        #region Private methods
        private void CompleteCleaningCycle(bool isDataClean, IEnumerable<string> involvedTableNames)
        {
            using (new PerformanceCounter("CleansingManager.CompleteCleaningCycle"))
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
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
                if (m_samplingRulesList != null)
                {
                    foreach (Rule rule in m_samplingRulesList)
                    {
                        rule.StopCleaningProcess += new EventHandler(SetStopSamping);
                        rule.DataIsClean += new Rule.RuleFinishedDelegate(SetDataIsCleanEventHandler);
                        rule.init(null);
                    }
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
