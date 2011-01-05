using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.DataLayerAwareLib;
using DCF.DataLayer;
using DCF.Common;
using DCF.DemoRules;
using DCF.Lib;

namespace DCF.PaperRules
{
    public class Cosine : DcfRule
    {
        public Cosine(MySqlUtils sqlUtils, IRuleSupplier ruleSupplier, string category) :
            base(sqlUtils, ruleSupplier, category)
        {
            m_ruleExecuter += new RuleExecuterDelegate(CosineMethod);
            m_ruleInitializer += new RuleExecuterDelegate(internalInit);
            Id = CosineStr;
            InvolvedTables = new List<string>(new string[] { TableConstants.UserScores });
            AffectedTables = new List<string>();
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

            // initiate all the facts score to 1
            SqlUtils.ExecuteNonQuery(String.Format(
                "UPDATE {0} SET Score = 1.0 WHERE Category='{1}'", 
                TableConstants.ScoredFacts, Category));

            // initiallize the users Belief to 2*|f in u|/|f|-1
            SqlUtils.ExecuteNonQuery(String.Format(
                "UPDATE {0} us SET Belief = "+
                "2*(SELECT COUNT(*) FROM {2} sfu WHERE us.UserId=sfu.UserId AND Category='{1}')/"+
                "(SELECT COUNT(*) FROM {3} sf WHERE sf.Factor>0 AND Category='{1}') - 1",
                TableConstants.UserScores, Category, 
                TableConstants.ScoredFactsUsersView,
                TableConstants.ScoredFacts));

        }

        void CosineMethod(Dictionary<string, object> data)
        {
            using (new PerformanceCounter(RulesLogger))
            using (new PerformanceCounter(CosineStr))
            {


                string usersStmnt = string.Format(
                    "UPDATE {0} us SET us.Belief = (1-{3})*us.Belief + {3}*" +
                    "(2*(SELECT SUM(sfu1.Score) FROM {2} sfu1 WHERE sfu1.UserId = us.UserId AND Category='{4}') - "+
                    "(SELECT SUM(sf2.Score) FROM {1} sf2 WHERE sf2.Factor>0 AND sf2.Category='{4}'))/"+
                    // norm
                    "(SELECT SQRT(COUNT(*)*SUM(sf1.score*sf1.score)) FROM {1} sf1 WHERE sf1.Factor>0 AND Category='{4}')",
                    TableConstants.UserScores, TableConstants.ScoredFacts, 
                    TableConstants.ScoredFactsUsersView, Ni, Category);

                string factsStmnt = string.Format(
                    "UPDATE {0} sf SET sf.Score = 2*"+
                    "IFNULL((SELECT SUM(POW(us1.Belief, 3)) FROM {1} us1, {2} sfu1 "+
                    "WHERE us1.UserId=sfu1.UserId AND sfu1.ItemId=sf.ItemId), 0)/ "+
                    "(SELECT SUM(POW(us2.Belief, 3)) FROM {1} us2) - 1 " +
                    "WHERE Category='{3}'",
                    TableConstants.ScoredFacts, TableConstants.UserScores, 
                    TableConstants.ItemsMentions, Category);

                SqlUtils.ExecuteNonQuery(usersStmnt);
                SqlUtils.ExecuteNonQuery(factsStmnt);

                RepairKeySample.CalculateQuality(SqlUtils);
            }
        }

        public const string CosineStr = "Cosine";
        public const double Ni = 1.0;

    }
}
