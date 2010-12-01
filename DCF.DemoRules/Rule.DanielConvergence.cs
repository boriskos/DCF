using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using DCF.Lib;
using DCF.DataLayerAwareLib;
using DCF.DataLayer;
using DCF.Common;

namespace DCF.DemoRules
{
    public class DanielConvergence: MySqlRule
    {
        public DanielConvergence(MySqlUtils sqlUtils, IRuleSupplier ruleSupplier)
            : base(sqlUtils, ruleSupplier)
        {
            this.AffectedTables = new List<string>();
            this.Id = "DanielConvergence";
            this.InvolvedTables = new List<string>() { TableConstants.UserScores };
            this.PrerequisiteRules = new List<DCF.Lib.Rule>();
            this.RuleType = RuleTypeEnum.System;

            this.m_ruleExecuter += new RuleExecuterDelegate(CosineConverged_m_ruleExecuter);
            this.m_ruleInitializer += new RuleExecuterDelegate(CosineConverged_m_ruleInitializer);


            // initialize the threshold
            m_ConvergenceThreshold = CleaningConfiguration.Instance.ConversionDelta;
            // initialize convergence count - number of good samples in a row that need to be good in order to decide converence
            m_ConvergenceSamplesCount = CleaningConfiguration.Instance.ConversionSamplesCount;

        }

        void CosineConverged_m_ruleInitializer(Dictionary<string, object> dataHashTable)
        {
            SqlUtils.DropTableIfExists(TableConstants.UserScoresHistory);
            string createUserHistoryTable = string.Format(
                "CREATE TABLE IF NOT EXISTS {0} ( " +
                "Version INT(11) NOT NULL, " +
                "UserId INT(11) NOT NULL, " +
                "UserScore DOUBLE NOT NULL, " +
                "INDEX {0}_userIdx (UserId ASC) " +
                ") ENGINE MEMORY", TableConstants.UserScoresHistory);
            SqlUtils.ExecuteNonQuery(createUserHistoryTable);
        }

        void CosineConverged_m_ruleExecuter(Dictionary<string, object> dataHashTable)
        {
            using (new PerformanceCounter(RulesLogger))
            using (new PerformanceCounter(Id))
            {
                Func<int> curIterationGetter = dataHashTable["CurrentIteration"] as Func<int>;
                int curIteration = curIterationGetter();

                SqlUtils.ExecuteNonQuery(string.Format(
                    "INSERT INTO {0} (Version, UserScore, UserId) " +
                    "(SELECT {2} as Version, Belief as UserScore, UserId FROM {1})",
                    TableConstants.UserScoresHistory, TableConstants.UserScores, curIteration));
                
                DataSet ds = new DataSet();
                SqlUtils.ExecuteQuery(string.Format(
                    "SELECT UserId, MAX(UserScore) as MaxScore, MIN(UserScore) as MinScore " +
                    "FROM {0} WHERE Version>={1} GROUP BY UserId",
                    TableConstants.UserScoresHistory, curIteration - m_ConvergenceSamplesCount), ds);
                // clean unnecesary data
                SqlUtils.ExecuteNonQuery( string.Format(
                    "DELETE FROM {0} WHERE Version < {1}",
                    TableConstants.UserScoresHistory, curIteration - m_ConvergenceSamplesCount));
                bool uncoverged = false;
                int usersCount = ds.Tables[0].Rows.Count;
                double nonConvergedUsersCount = 0.0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    double minUserScore = row.Field<double>("MinScore");
                    double maxUserScore = row.Field<double>("MaxScore");
                    double userDif = (maxUserScore - minUserScore) / maxUserScore;
                    if (userDif > m_ConvergenceThreshold)
                    {
                        Logger.DebugWriteLine(string.Format(
                            "Iteration {0} Score (max, min) ({3}, {4}) PreviousIteration {1} "+
                            "difference {2}",
                            curIteration, curIteration - m_ConvergenceSamplesCount, userDif, maxUserScore, minUserScore));

                        nonConvergedUsersCount += 1.0;
                        if (nonConvergedUsersCount / usersCount > CleaningConfiguration.Instance.ConversionTolerance)
                        {
                            uncoverged = true;
                            break;
                        }
                    }
                }
                if (!uncoverged && curIteration - m_ConvergenceSamplesCount > 0)
                {
                    OnStopCleaningProcess();
                }
             }
        }


        private double m_ConvergenceThreshold;
        private int m_ConvergenceSamplesCount;
    }
}
