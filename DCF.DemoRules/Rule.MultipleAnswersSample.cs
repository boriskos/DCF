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
            ///////////////////////////////////////////////
            // reinitiate the Users Scores table
            // for each user from table of Users create 
            // posititve and negative rows

            SqlUtils.ExecuteNonQuery(string.Format(
                "CREATE TABLE IF NOT EXISTS {0} ( " +
                "UserID int(11) unsigned NOT NULL PRIMARY KEY, " +
                "Belief double NOT NULL, " +
                "Version int(11) NULL, " +
                "NumOfFacts int(11) NOT NULL, " +
                "FOREIGN KEY usUserID_fkey (UserID) REFERENCES Users (UserID) ON DELETE CASCADE" +
                ") ENGINE = InnoDB",
                TableConstants.UserScores));

            // new users creation in the userscores table
            SqlUtils.ExecuteNonQuery(string.Format(
                "INSERT INTO {0} (UserId, Belief, Version, NumOfFacts) " +
                "(SELECT u.UserID as UserId, {2} as Belief, 1 as Version, " +
                "0 as NumOfFacts FROM {1} u " +
                "WHERE u.UserID NOT IN (SELECT us.UserId FROM {0} us))",
                TableConstants.UserScores, TableConstants.Users, BeliefInitialValue,
                TableConstants.ItemsMentions));

            // update users number of facts and zero Version
            SqlUtils.ExecuteNonQuery(string.Format(
                "UPDATE {0} us,  (SELECT im.UserID, COUNT(*) as NumOfItems FROM {1} im GROUP BY im.UserID) s " +
                "SET us.NumOfFacts = s.NumOfItems, Version=1 " +
                "WHERE s.UserID=us.UserID",
                TableConstants.UserScores, TableConstants.ItemsMentions));

            ////////////////////////////////////////////////////
            // creation and initiallization of scored facts table
            SqlUtils.ExecuteNonQuery(string.Format(
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
                "FOREIGN KEY sfItemID_fkey (ItemID) REFERENCES Items (ItemID) ON DELETE CASCADE, " +
                "FOREIGN KEY sfTopicID_fkey (TopicID) REFERENCES Topics (TopicID) ON DELETE CASCADE " +
                ") ENGINE = InnoDB",
                TableConstants.ScoredFacts));

            // insert into ScoredFacts new facts
            SqlUtils.ExecuteNonQuery(String.Format(
                "INSERT INTO {0} (ItemId, TopicId, Category, Factor, Score) " +
                "SELECT i.ItemId, i.TopicId, '{3}' AS Category, 0 AS Factor, 0 AS Score " +
                "FROM {1} t, {2} i WHERE t.TopicId=i.TopicId AND t.Category = '{3}' " +
                "AND i.ItemId NOT IN (SELECT ItemID FROM {0})",
                TableConstants.ScoredFacts, TableConstants.Topics, TableConstants.Items, Category));

            // update all facts Factor 
            SqlUtils.ExecuteNonQuery(String.Format(
                "UPDATE {0} sf, (SELECT im.ItemId, COUNT(im.ID) as Factor FROM {1} im GROUP BY im.ItemId) s " +
                "SET sf.FActor = s.Factor WHERE sf.ItemId = s.ItemId",
                TableConstants.ScoredFacts, TableConstants.ItemsMentions));


            ///////////////////////////////////////////////
            // create view that connects user capitals to scored facts
            //SqlUtils.DropTableIfExists(TableConstants.ScoredFactsUsers);
            //SqlUtils.ExecuteNonQuery(string.Format(
            //    "CREATE TABLE IF NOT EXISTS {0} (" +
            //    "FactId INT(11) NOT NULL," +
            //    "UserId INT(11) NOT NULL, " +
            //    "INDEX FactId_idx (FactId ASC), " +
            //    "INDEX UserId_idx (UserId ASC) )" +
            //    "ENGINE=MEMORY",
            //    TableConstants.ScoredFactsUsers));

            //SqlUtils.ExecuteNonQuery(string.Format(
            //    "INSERT INTO {0} (" +
            //    "SELECT sf.ID as FactId, uc.UserID FROM {1} uc, {2} sf " +
            //    "WHERE uc.City = sf.City AND uc.Country=sf.Country )",
            //    TableConstants.ScoredFactsUsers,
            //    TableConstants.UserCapitals,
            //    TableConstants.ScoredFacts));

            //// creates view of capitals to their total score
            //SqlUtils.ExecuteNonQuery(string.Format(
            //    "CREATE OR REPLACE VIEW {0} AS " +
            //    "SELECT uc.Country, SUM(aus.Score) AS Score " +
            //    "FROM {1} uc, {2} aus " +
            //    "WHERE uc.userid=aus.userid " +
            //    "GROUP BY uc.Country",
            //    TableConstants.CapitalsScoresView,
            //    TableConstants.UserCapitals,
            //    TableConstants.AbsoluteUserScoresView));

            //// creation of scored facts view
            //SqlUtils.ExecuteNonQuery(string.Format(
            //    "CREATE OR REPLACE VIEW {0} AS " +
            //    "SELECT sf.ID, sf.Country, sf.City, sum(aus.Score)/cs.Score AS Score " +
            //    "FROM {1} aus, {2} sf, {3} uc, {4} cs " +
            //    "WHERE sf.Country = uc.Country AND sf.City = uc.City AND uc.UserID = aus.UserId " +
            //        "AND cs.Country = sf.Country " +
            //    "GROUP BY sf.Country, sf.City",
            //    TableConstants.ScoredFactsView,
            //    TableConstants.AbsoluteUserScoresView,
            //    TableConstants.ScoredFacts,
            //    TableConstants.UserCapitals,
            //    TableConstants.CapitalsScoresView));
            ///////////////////////////////////////////////////

            //////////////////////////////////////////////
            // create temporary table for repair key operation resluts
            SqlUtils.ExecuteNonQuery(string.Format(
                "CREATE TABLE IF NOT EXISTS {0} ( " +
                "FactId int(11) unsigned NOT NULL) ENGINE = MEMORY",
                TableConstants.RepKeyResults));

        }

        void SampleWithJoin(Dictionary<string, object> data)
        {
            using (new PerformanceCounter(RulesLogger))
            using (new PerformanceCounter(Id))
            {
                Func<int> curIterationGetter = data["CurrentIteration"] as Func<int>;

                Logger.DebugWriteLine("In " + Id, Logger.RulesStr);
                Logger.DebugIndent();

                // compute fact scores and normal;ize them
                using (new PerformanceCounter(Id + "_Update"))
                {
                    string factScoreUpdate1 = string.Format(
                        "UPDATE {0} sf SET sf.Score = IFNULL((SELECT SUM(us.Belief) FROM {1} us, {2} im " +
                        "WHERE sf.ItemID=im.ItemID AND im.UserId=us.UserId AND sf.Category='{3}'), 0)",
                        TableConstants.ScoredFacts, TableConstants.UserScores,
                        TableConstants.ItemsMentions, Category);
                    string factScoreUpdate2 = string.Format(
                        "UPDATE {0} sf, (SELECT SUM(sf1.Score) AS TopicScore, sf1.TopicId " +
                        "FROM {0} sf1 WHERE sf1.Category = '{1}' GROUP BY sf1.TopicId) cs " +
                        "SET sf.Score = sf.Score / cs.TopicScore " +
                        "WHERE sf.TopicId = cs.TopicId AND sf.Category='{1}' AND cs.TopicScore <> 0",
                        TableConstants.ScoredFacts, Category);
                    SqlUtils.ExecuteNonQuery(factScoreUpdate1);
                    SqlUtils.ExecuteNonQuery(factScoreUpdate2);
                }

                DataSet scoredFactsDs = new DataSet();
                using (new PerformanceCounter(Id + "_Fetch"))
                {
                    // join these scores with encoded facts by user
                    SqlUtils.ExecuteQuery(string.Format(
                        "SELECT sf.ItemId, sf.TopicId, sf.Score, sf.Factor, c.Correctness "+
                        "FROM {0} sf, (select 0 as Correctness union select 1 as Correctness) c " +
                        "WHERE sf.Category='{1}'",
                        TableConstants.ScoredFacts, Category), scoredFactsDs);
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
                        SqlUtils.ExecuteNonQuery(string.Format(
                            "UPDATE {0} us " +
                            "SET us.Belief=(us.Belief + " +
                            "IFNULL((SELECT COUNT(*) FROM {1} rk, {2} im WHERE rk.FactId=im.ItemId AND im.UserID=us.UserId), 0)/" +
                            //"us.Version/us.NumOfFacts)/(1+1/us.Version), us.Version=us.Version+1",
                            "us.Version/us.NumOfFacts), us.Version=us.Version+1",
                            TableConstants.UserScores,
                            TableConstants.RepKeyResults,
                            TableConstants.ItemsMentions));
                    }
                }

                Logger.DebugUnindent();
            }
        }

    }
}
