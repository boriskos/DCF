using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.DataLayerAwareLib;
using DCF.DataLayer;
using DCF.Lib;
using DCF.Common;
using System.Data;

namespace DCF.DemoRules
{
    public class MultipleAnswersSample: MySqlRule
    {
        public MultipleAnswersSample(MySqlUtils sqlUtils, IRuleSupplier ruleSupplier, string category) :
            base(sqlUtils, ruleSupplier)
        {
            m_ruleExecuter += new RuleExecuterDelegate(SampleWithJoin);
            m_ruleInitializer += new RuleExecuterDelegate(internalInit);
            Id = "MultipleAnswersSample";
            InvolvedTables = new List<string>();
            AffectedTables = new List<string>();
            PrerequisiteRules = new List<DCF.Lib.Rule>();
            Category = category;
        }
        public const double BeliefInitialValue = 0.2;

        public string Category { get; private set; }

        void internalInit(Dictionary<string, object> dataHashTable)
        {
            PrepareDb();
        }

        private void PrepareDb()
        {
            RepairKeySample.PrepareDb(SqlUtils, Category, TopicType.MultipleAnswers);
            //////////////////////////////////////////////
            // create temporary table for repair key operation resluts
            SqlUtils.ExecuteNonQuery(string.Format(
                "CREATE TABLE IF NOT EXISTS {0} ( " +
                "FactId int(11) unsigned NOT NULL) ENGINE = MEMORY",
                TableConstants.RepKeyResults));

        }

        public virtual void SampleWithJoin(Dictionary<string, object> data)
        {
            using (new PerformanceCounter(RulesLogger))
            using (new PerformanceCounter(Id))
            {
                Func<int> curIterationGetter = data["CurrentIteration"] as Func<int>;

                Logger.DebugWriteLine("In " + Id, Logger.RulesStr);
                Logger.DebugIndent();

                // compute fact scores and normalize them
                using (new PerformanceCounter(Id + "_Update"))
                {
                    RepairKeySample.CalculateFactScores(SqlUtils, Category, TopicType.MultipleAnswers);
                }

                DataSet scoredFactsDs = new DataSet();
                using (new PerformanceCounter(Id + "_Fetch"))
                {
                    // join these scores with encoded facts by user
                    SqlUtils.ExecuteQuery(string.Format(
                        "SELECT sf.ItemId, sf.TopicId, sf.Score, sf.Factor, c.Correctness "+
                        "FROM {0} sf, {1} t, (select 0 as Correctness union select 1 as Correctness) c " +
                        "WHERE sf.Category='{2}' AND sf.TopicId=t.TopicId AND t.TopicType={3}",
                        TableConstants.ScoredFacts, TableConstants.Topics, Category, (int)TopicType.MultipleAnswers ), 
                        scoredFactsDs);
                }

                // at random (by the fact score) believe or disbelieve the fact
                var repairedFacts = scoredFactsDs.Tables[0].AsEnumerable().RepairKey(
                    row => row.Field<UInt32>("ItemId")*1000 + row.Field<UInt32>("TopicId"),
                    row => row.Field<Int64>("Correctness")==0 ? row.Field<double>("Score") : (1-row.Field<double>("Score")));
                repairedFacts = repairedFacts.Where(row => row.Field<Int64>("Correctness") == 0);
                int count = repairedFacts.Count();
                Logger.DebugWriteLine(string.Format("Repairing {0} facts.", count));
                if (count > 0)
                {
                    using (new PerformanceCounter(Id + "_Update"))
                    {
                        using (new PerformanceCounter(Id + "_Update_Insert"))
                        {
                            // populate RepairKeyTable in the DB with the results of this repair key operation
                            DataTable tbl = new DataTable(TableConstants.RepKeyResults);
                            tbl.Columns.Add("FactId", typeof(UInt32));
                            var projectedResults = from fact in repairedFacts
                                                   select fact.Field<UInt32>("ItemId");
                            foreach (var val in projectedResults)
                            {
                                tbl.Rows.Add(val);
                            }
                            SqlUtils.RePopulateExistingTable(tbl);
                            //SqlUtils.ManualRePopulateExistingTable(tbl, transaction);
                        }

                        // update users with the proportion of facts that survived this repair key operation
                        //RepairKeySample.NewNormalizedScoreCalculation(SqlUtils);
                        RepairKeySample.NewScoreCalculation(SqlUtils);
                    }
                }

                Logger.DebugUnindent();
            }
        }

    }
}
