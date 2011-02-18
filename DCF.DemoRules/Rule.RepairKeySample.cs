using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using DCF.Lib;
using DCF.DataLayer;
using DCF.DataLayerAwareLib;
using DCF.Common;

namespace DCF.DemoRules
{
    public class RepairKeySample : DcfRule
    {
        public const double InitialFactScoreScale = 0.1;
        public const double InitialScoreBase = 10;
        public const double Alfa = 0.3;
        public const double Epsilon = 0.00001;
        public const double BeliefInitialValue = 0.2;

        public RepairKeySample(MySqlUtils sqlUtils, IRuleSupplier ruleSupplier, string category) :
            base(sqlUtils, ruleSupplier, category)
        {
            m_ruleExecuter += new RuleExecuterDelegate(SampleWithJoin);
            m_ruleInitializer += new RuleExecuterDelegate(internalInit);
            Id = "RepairKeySample";
            InvolvedTables = new List<string>(new string[] { 
                TableConstants.UserCapitals, TableConstants.EncodedUserCapitals });
            AffectedTables = new List<string>();
            PrerequisiteRules = new List<DCF.Lib.Rule>();
        }

        void internalInit(Dictionary<string, object> dataHashTable)
        {
            PrepareDb(SqlUtils, Category);

            //////////////////////////////////////////////
            // create temporary table for repair key operation resluts
            SqlUtils.ExecuteNonQuery(string.Format(
                "CREATE TABLE IF NOT EXISTS {0} ( " +
                "FactId int(11) unsigned NOT NULL) ENGINE = MEMORY",
                TableConstants.RepKeyResults));

        }

        public static void PrepareDb(MySqlUtils sqlUtils, string category)
        {
            ///////////////////////////////////////////////
            // reinitiate the Users Scores table
            // for each user from table of Users create 
            // posititve and negative rows

            sqlUtils.ExecuteNonQuery(string.Format(
                "CREATE TABLE IF NOT EXISTS {0} ( " +
                "UserID int(11) unsigned NOT NULL PRIMARY KEY, " +
                "Belief double NOT NULL, " +
                "Version int(11) NULL, " +
                "NumOfFacts int(11) NOT NULL " +
                // ", FOREIGN KEY usUserID_fkey (UserID) REFERENCES Users (UserID) ON DELETE CASCADE" +
                ") ENGINE = MyISAM",
                TableConstants.UserScores));

            // new users creation in the userscores table
            sqlUtils.ExecuteNonQuery(string.Format(
                "INSERT INTO {0} (UserId, Belief, Version, NumOfFacts) " +
                "(SELECT u.UserID as UserId, {2} as Belief, 1 as Version, " +
                "0 as NumOfFacts FROM {1} u " +
                "WHERE u.UserID NOT IN (SELECT us.UserId FROM {0} us))",
                TableConstants.UserScores, TableConstants.Users, BeliefInitialValue, 
                TableConstants.ItemsMentions));

            // update users number of facts
            sqlUtils.ExecuteNonQuery(string.Format(
                "UPDATE {0} us,  (SELECT im.UserID, COUNT(*) as NumOfItems FROM {1} im GROUP BY im.UserID) s " +
                "SET us.NumOfFacts = s.NumOfItems, Version=1 " +
                "WHERE s.UserID=us.UserID",
                TableConstants.UserScores, TableConstants.ItemsMentions));

            ////////////////////////////////////////////////////
            // creation and initiallization of scored facts table
            sqlUtils.ExecuteNonQuery(string.Format(
                "CREATE TABLE IF NOT EXISTS {0} ( " +
                "ItemID INT(11) unsigned NOT NULL, " +
                "TopicID INT(11) unsigned NOT NULL, " +
                "Factor int(11) NOT NULL, " +
                "Score DOUBLE NOT NULL, " +
                "Category varchar(70) COLLATE utf8_bin NOT NULL, " +
                "Correctness TINYINT(1) NULL, " +
                //"FactName varchar(100) COLLATE utf8_bin NULL, " +
                //"FactValue varchar(500) COLLATE utf8_bin NOT NULL, " +

                "PRIMARY KEY(ItemID), " +
                //"FOREIGN KEY sfItemID_fkey (ItemID) REFERENCES Items (ItemID) ON DELETE CASCADE, " +
                "FOREIGN KEY sfItemID_fkey (ItemID) REFERENCES Items (id) ON DELETE CASCADE, " +
                "FOREIGN KEY sfTopicID_fkey (TopicID) REFERENCES Topics (TopicID) ON DELETE CASCADE " +
                ") ENGINE = MyISAM",
                TableConstants.ScoredFacts));

            // insert into ScoredFacts new facts
            sqlUtils.ExecuteNonQuery(String.Format(
                "INSERT INTO {0} (ItemId, TopicId, Category, Factor, Score) " +
                "SELECT i.ItemId, i.TopicId, '{3}' AS Category, 0 AS Factor, 0 AS Score " +
                "FROM {1} t, {2} i WHERE t.TopicId=i.TopicId AND t.Category = '{3}' " + 
                "AND i.ItemId NOT IN (SELECT ItemID FROM {0})",
                TableConstants.ScoredFacts, TableConstants.Topics, TableConstants.Items, category));

            // update all facts Factor 
            sqlUtils.ExecuteNonQuery(String.Format(
                "UPDATE {0} sf, (SELECT im.ItemId, COUNT(im.ID) as Factor FROM {1} im GROUP BY im.ItemId) s " +
                "SET sf.FActor = s.Factor WHERE sf.ItemId = s.ItemId",
                TableConstants.ScoredFacts, TableConstants.ItemsMentions));
        }

        /// <summary>
        /// Applies majority on facts and normalizes their score
        /// </summary>
        public static void CalculateFactScores(MySqlUtils sqlUtils, string category)
        {
            string factScoreUpdate1 = string.Format(
                "UPDATE {0} sf SET sf.Score = IFNULL((SELECT SUM(us.Belief) FROM {1} us, {2} im " +
                "WHERE sf.ItemID=im.ItemID AND im.UserId=us.UserId AND sf.Category='{3}'), 0)",
                TableConstants.ScoredFacts, TableConstants.UserScores,
                TableConstants.ItemsMentions, category);
            string factScoreUpdate2 = string.Format(
                "UPDATE {0} sf, (SELECT SUM(sf1.Score) AS TopicScore, sf1.TopicId " +
                "FROM {0} sf1 WHERE sf1.Category = '{1}' GROUP BY sf1.TopicId) cs " +
                "SET sf.Score = sf.Score / cs.TopicScore " +
                "WHERE sf.TopicId = cs.TopicId AND sf.Category='{1}' AND cs.TopicScore <> 0",
                TableConstants.ScoredFacts, category);
            sqlUtils.ExecuteNonQuery(factScoreUpdate1);
            sqlUtils.ExecuteNonQuery(factScoreUpdate2);
        }


        void SampleWithJoin(Dictionary<string, object> data)
        {
            using (new PerformanceCounter(RulesLogger))
            using (new PerformanceCounter(Id))
            {
                Func<int> curIterationGetter = data["CurrentIteration"] as Func<int>;

                Logger.DebugWriteLine("In "+Id, Logger.RulesStr);
                Logger.DebugIndent();

                // Applies majority on facts and normalizes their score
                using (new PerformanceCounter(Id + "_Update"))
                {
                    CalculateFactScores(SqlUtils, Category);
                }

                DataSet scoredFactsDs = new DataSet();
                using (new PerformanceCounter(Id+"_Fetch"))
                {
                    // join these scores with encoded facts by user
                    SqlUtils.ExecuteQuery(string.Format(
                        "SELECT * FROM {0} WHERE Category='{1}'",
                        TableConstants.ScoredFacts, Category), scoredFactsDs);
                }

                // make sure the countries are distinct
                var repairedFacts = scoredFactsDs.Tables[0].AsEnumerable().RepairKey(
                    row => row.Field<UInt32>("TopicId"),
                    row => row.Field<double>("Score"));
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

                        NewScoreCalculation(SqlUtils);
                        //OriginalScoreCalculation();

                        CalculateQuality(SqlUtils);
                    }
                }

                Logger.DebugUnindent();
            }
        }

        public static object CalculateQuality(MySqlUtils sqlUtils)
        {
            string innerSelect =
                "select a.* from scoredfacts a, " +
                "(SELECT topicid, max(Score) as score FROM scoredfacts group by topicid) b " +
                "where a.topicid = b.topicid and a.score = b.score group by a.topicid";
            string qualityMeasurement = string.Format(
                "select " +
                "(select count(*) from ({0}) d, correctfacts c where c.itemid=d.itemid) / " +
                "(select count(*) from ({0}) e) as Quality", innerSelect
            );
            object qualityRes = sqlUtils.ExecuteScalar(qualityMeasurement);
            Logger.TraceWriteLine(string.Format("The quality of the run is {0}", qualityRes.ToString()));
            return qualityRes;
        }

        public static void NewScoreCalculation(MySqlUtils sqlUtils)
        {
            // this works but not converges
            sqlUtils.ExecuteNonQuery(string.Format(
                "UPDATE {0} us " +
                "SET us.Belief=((1-{3})*us.Belief + " +
                "{3}*(SELECT COUNT(*) FROM {1} rk, {2} im " +
                "WHERE rk.FactId=im.ItemId AND im.UserID=us.UserId)/" +
                //"us.Version/us.NumOfFacts)/(1+1/us.Version), us.Version=us.Version+1",
                "us.NumOfFacts/us.version), us.Version=us.Version+1",
                TableConstants.UserScores,
                TableConstants.RepKeyResults,
                TableConstants.ItemsMentions,
                0.2));
        }

        private void AddOnlyScoreCalculation()
        {
            // this works but not converges
            SqlUtils.ExecuteNonQuery(string.Format(
                "UPDATE {0} us " +
                "SET us.Belief=(us.Belief + " +
                "(SELECT COUNT(*) FROM {1} rk, {2} im " +
                "WHERE rk.FactId=im.ItemId AND im.UserID=us.UserId)" +
                "), us.Version=us.Version+1",
                TableConstants.UserScores,
                TableConstants.RepKeyResults,
                TableConstants.ItemsMentions,
                0.1));
        }

        private void OriginalScoreCalculation()
        {
            // this is original scores calculation
            SqlUtils.ExecuteNonQuery(string.Format(
                "UPDATE {0} us " +
                "SET us.Belief=(us.Belief + " +
                "(SELECT COUNT(*) FROM {1} rk, {2} im WHERE rk.FactId=im.ItemId AND im.UserID=us.UserId)/" +
                //"us.Version/us.NumOfFacts)/(1+1/us.Version), us.Version=us.Version+1",
                "us.Version/us.NumOfFacts), us.Version=us.Version+1",
                TableConstants.UserScores,
                TableConstants.RepKeyResults,
                TableConstants.ItemsMentions));
        }

        private void ScoreItLikeCosine()
        {
            // behaves like cosine
            string usersStmnt = string.Format(
                "UPDATE {0} us SET us.Belief = (1-{3})*us.Belief + {3}*" +
                "(2*IFNULL((SELECT SUM(sfu1.Score) FROM {2} sfu1, {5} rk " +
                "WHERE rk.FactId=sfu1.ItemID AND sfu1.UserId = us.UserId AND Category='{4}'),0) - " +
                "(SELECT SUM(sf2.Score) FROM {1} sf2 WHERE sf2.Factor>0 AND sf2.Category='{4}'))/" +
                // norm
                "(SELECT SQRT(COUNT(*)*SUM(sf1.score*sf1.score)) FROM {1} sf1 WHERE sf1.Factor>0 AND Category='{4}')",
                TableConstants.UserScores, TableConstants.ScoredFacts,
                TableConstants.ScoredFactsUsersView, 0.2, Category,
                TableConstants.RepKeyResults);

            SqlUtils.ExecuteNonQuery(usersStmnt);
        }
    }
}
