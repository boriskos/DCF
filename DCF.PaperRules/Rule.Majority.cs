using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.DataLayerAwareLib;
using DCF.DataLayer;
using DCF.Lib;
using DCF.Common;
using DCF.DemoRules;

namespace DCF.PaperRules
{
    public class Majority : DcfRule
    {
        public Majority(MySqlUtils sqlUtils, IRuleSupplier ruleSupplier, string category) :
            base(sqlUtils, ruleSupplier, category)
        {
            m_ruleInitializer += new RuleExecuterDelegate(internalInit);
            m_ruleExecuter += new RuleExecuterDelegate(MajorityFunc);
            Id = "MajorityRule";
            PrerequisiteRules = new List<Rule>(0);
        }

        void internalInit(Dictionary<string, object> dataHashTable)
        {
            RepairKeySample.PrepareDb(SqlUtils, Category);
        }

        void MajorityFunc(Dictionary<string, object> data)
        {
            using (new PerformanceCounter(RulesLogger))
            using (new PerformanceCounter(Id))
            {
                Logger.DebugWriteLine("In MajorityRepairKey.", Logger.RulesStr);
                Logger.DebugIndent();

                RepairKeySample.CalculateFactScores(SqlUtils, Category);

                OnStopCleaningProcess();
                Logger.DebugUnindent();
            }
        }
    }
}
