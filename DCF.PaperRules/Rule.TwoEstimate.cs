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
    public class TwoEstimate : DcfRule
    {
        public TwoEstimate(MySqlUtils sqlUtils, IRuleSupplier ruleSupplier, string category) :
            base(sqlUtils, ruleSupplier, category)
        {
            m_ruleExecuter += new RuleExecuterDelegate(TwoEstimateMethod);
            m_ruleInitializer += new RuleExecuterDelegate(internalInit);
            Id = TwoEstimateStr;
            InvolvedTables = new List<string>(new string[] { TableConstants.UserScores, TableConstants.BelievedUsers });
            AffectedTables = new List<string>(new string[] { TableConstants.BelievedUsers });
        }
        void internalInit(Dictionary<string, object> dataHashTable)
        {
            PrepareDb();
        }
        private void PrepareDb()
        {
            RepairKeySample.PrepareDb(SqlUtils, Category);

            // create helping view of facts scores and items mentions
            SqlUtils.ExecuteNonQuery(string.Format(
                "CREATE OR REPLACE VIEW {0} AS " +
                "SELECT im.ID, im.UserId, im.ItemId, sf.Score, sf.Category " +
                "FROM {1} im, {2} sf WHERE sf.ItemId = im.ItemId",
                TableConstants.ScoredFactsUsersView, TableConstants.ItemsMentions,
                TableConstants.ScoredFacts));
            // initiallize the belief of all the users to 0
            SqlUtils.ExecuteNonQuery(string.Format(
                "UPDATE {0} SET Belief=0", TableConstants.UserScores));
        }

        void TwoEstimateMethod(Dictionary<string, object> data)
        {
            using (new PerformanceCounter(RulesLogger))
            using (new PerformanceCounter(TwoEstimateStr))
            {
                string factsStmnt = string.Format(
                    "UPDATE {0} sf SET sf.Score = " +
                    "( (SELECT SUM(us2.Belief) FROM {1} us2) - " +
                    "IFNULL((SELECT SUM(2*us1.Belief-1) FROM {1} us1, {2} im " +
                    "WHERE us1.UserId=im.UserId AND im.ItemId=sf.ItemId),0) )/" +
                    "(SELECT COUNT(*) FROM {1}) " +
                    "WHERE Category='{3}'",
                    TableConstants.ScoredFacts, TableConstants.UserScores, 
                    TableConstants.ItemsMentions, Category);
                string usersStmnt = string.Format(
                    "UPDATE {0} us SET us.Belief = " +
                    "((SELECT SUM(sf2.Score) FROM {1} sf2 WHERE sf2.Category='{3}') - " +
                    "(SELECT SUM(2*sfu1.Score-1) FROM {2} sfu1 WHERE sfu1.UserId = us.UserId)) / " +
                    // norm
                    "(SELECT COUNT(*) FROM {1} sf WHERE sf.Category='{3}')",
                    TableConstants.UserScores, TableConstants.ScoredFacts, 
                    TableConstants.ScoredFactsUsersView, Category);

                SqlUtils.ExecuteNonQuery(factsStmnt);
                SqlUtils.ExecuteNonQuery(usersStmnt);
            }
        }

        public const string TwoEstimateStr = "TwoEstimates";
   }
}
