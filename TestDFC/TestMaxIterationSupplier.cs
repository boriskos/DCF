using System;
using System.Collections.Generic;
using System.Text;
using DCF.Lib;
using DCF.Common;

namespace TestDFC
{
    public class TestMaxIterationSupplier : ITestRuleSupplier
    {
        public TestMaxIterationSupplier()
        {
            m_cleaningRule = new RuleTestMaxIteration(this);
            m_sampleRule = new RuleTestMaxIteration(this);
            m_numCleaningRulesCreated = m_numSamplingRulesCreated = 0;
        }

        #region ITestRuleSupplier Members

        public bool TestCurrentState()
        {
            if (m_numCleaningRulesCreated!=1)
            {
                Logger.TraceWriteLine(string.Format("Get Cleaning rules is called {0} times", m_numCleaningRulesCreated));
                return false;
            }
            if (m_numCleaningRulesCreated!=1)
            {
                Logger.TraceWriteLine(string.Format("Get Sampling rules is called {0} times", m_numSamplingRulesCreated));
                return false;
            }

            if (m_cleaningRule.CurIteration != CleaningConfiguration.MaxSampleIterations)
            {
                Logger.TraceWriteLine(string.Format(
                    "Cleaning rules were called {0} times out of {1} sample iterations", 
                    m_cleaningRule.CurIteration, CleaningConfiguration.MaxSampleIterations));
                return false;
            }
            if (m_sampleRule.CurIteration != CleaningConfiguration.MaxSampleIterations)
            {
                Logger.TraceWriteLine(string.Format(
                    "Sample rules were called {0} times out of {1} sample iterations",
                    m_sampleRule.CurIteration, CleaningConfiguration.MaxSampleIterations));
                return false;
            }
            return true;
        }

        public string Name { get { return "TestMaxIterationSupplier"; } }
        #endregion

        #region IRuleSupplier Members

        public IList<Rule> GetCleaningRules()
        {
            m_numCleaningRulesCreated++;
            return new List<DCF.Lib.Rule>() { m_cleaningRule };
        }

        public IList<Rule> GetSampleRules()
        {
            return new List<DCF.Lib.Rule>() { m_sampleRule };
        }

        #endregion

        private RuleTestMaxIteration m_cleaningRule;
        private RuleTestMaxIteration m_sampleRule;
        private int m_numCleaningRulesCreated;
        private int m_numSamplingRulesCreated;
    }
}
