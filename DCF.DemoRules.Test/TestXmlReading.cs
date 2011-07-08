using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.DataLayer;
using System.Configuration;
using DCF.Common;
using DCF.XmlRules;
using DCF.Lib;
using DCF.DataLayerAwareLib;
using System.Data;

namespace DCF.DemoRules.Test
{
	class TestXmlReading
	{
		public TestXmlReading(string filename)
		{
			m_filename = filename;
			DBName = ConfigurationManager.AppSettings["DBName"];
			DBUsername = ConfigurationManager.AppSettings["DBUserName"];
			DBPassword = ConfigurationManager.AppSettings["DBPassword"];
			HostName = ConfigurationManager.AppSettings["HostName"];

			foreach (string settingName in CleaningConfiguration.SettingNames)
			{
				string settingValue = ConfigurationManager.AppSettings[settingName];
				if (settingValue != null)
				{
					CleaningConfiguration.Instance[settingName] = settingValue;
				}
			}
		}

		public void init()
		{
			m_sqlUtils = new MySqlNativeClientUtils(DBUsername, DBPassword, DBName, HostName);
			SqlUtils.Connect();
			Logger.TraceWriteLine(string.Format("Connected to {0}/{1}@{2} - {3}", DBUsername, DBPassword, DBName, HostName));
			m_supplier = new XmlRuleSupplier(SqlUtils);

			m_supplier.parseFile(m_filename);
			IList<Lib.Rule> t = m_supplier.GetSampleRules();
			if (t.Count == 1 && t[0].Id == "PageRankRule")
			{
				m_cleansingManager = new CleansingManager(new XmlHack(m_supplier));
			}
			else
			{
				m_cleansingManager = new CleansingManager(m_supplier);
			}

		}

		public void DoTestFlow()
		{
			m_cleansingManager.cleanData(null);
		}

		public string DBUsername { get; private set; }
		public string DBPassword { get; private set; }
		public string DBName { get; private set; }
		public string HostName { get; private set; }
		public MySqlUtils SqlUtils { get { return m_sqlUtils; } }

		private string m_filename;
		private MySqlUtils m_sqlUtils;
		private XmlRuleSupplier m_supplier;
		private CleansingManager m_cleansingManager;
	}

	public class XmlHack : IRuleSupplier
	{
		public XmlHack(XmlRuleSupplier sup)
		{
			m_sup = sup;
		}

		public IList<Lib.Rule> GetCleaningRules()
		{
			return m_sup.GetCleaningRules();
		}

		public IList<Lib.Rule> GetSampleRules()
		{
			IList<Lib.Rule> res = m_sup.GetSampleRules();
			if (res != null)
			{
				res.Add(new ProbOrderConvergence(m_sup.SqlUtils, this));
			}
			return res;
		}
		public XmlRuleSupplier XmlSup { get { return m_sup; } }

		XmlRuleSupplier m_sup;
	}
	public class ProbOrderConvergence : MySqlRule
	{
		private int m_checkStep = 5;
		public ProbOrderConvergence(MySqlUtils sqlUtils, IRuleSupplier r_s)
			: base(sqlUtils, r_s)
		{
			Id = "ProbabilisticConvergence";
			m_ruleInitializer += new RuleExecuterDelegate(ProbConvergence_m_ruleInitializer);
			m_ruleExecuter += new RuleExecuterDelegate(ProbConvergence_m_ruleExecuter);
			object o = ConfigurationManager.AppSettings["ConvergenceCheckStep"];
			if (o != null)
			{
				m_checkStep = Convert.ToInt32(o);
			}
		}

		void ProbConvergence_m_ruleInitializer(Dictionary<string, object> dataHashTable)
		{
			SqlUtils.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS UserScoresHistory LIKE UserScores");
			SqlUtils.ExecuteNonQuery("TRUNCATE TABLE UserScoresHistory");
			SqlUtils.ExecuteNonQuery("INSERT INTO UserScoresHistory SELECT * FROM UserScores");
		}

		void ProbConvergence_m_ruleExecuter(Dictionary<string, object> dataHashTable)
		{
			Func<int> curIterationGetter = dataHashTable["CurrentIteration"] as Func<int>;
			int curIteration = curIterationGetter();

			if ((curIteration == 0) || (curIteration % m_checkStep > 0))
			{
				return;
			}
			DataSet ds = new DataSet();
			SqlUtils.ExecuteQuery(
				@"SELECT ush.Belief as PrevScore FROM UserScoresHistory ush, UserScores us 
                    WHERE us.Userid=ush.userid ORDER BY us.Belief DESC, ush.Belief DESC", ds);
			DataRow prev = null;
			int i = 0;
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				i++;
				if (prev == null)
				{
					prev = row;
					continue;
				}
				double prevScore = (double)prev[0];
				double curScore = (double)row[0];
				if (prevScore < curScore)
				{
					Logger.TraceWriteLine(string.Format("{1} - Not in order on {0} row", i, curIteration));
					return;
				}
				prev = row;
			}
			SqlUtils.ExecuteNonQuery(
				"UPDATE UserScoresHistory ush, UserScores us SET ush.Belief=us.Belief WHERE ush.userid=us.userid");
			// all is in order
			if (i > 0)
			{
				this.OnStopCleaningProcess();
			}

		}
	}
	public class ProbAbsConvergence : MySqlRule
	{
		private int m_checkStep = 5;
		private double m_convergencePercent = 0.01;
		public ProbAbsConvergence(MySqlUtils sqlUtils, IRuleSupplier r_s)
			: base(sqlUtils, r_s)
		{
			Id = "ProbabilisticConvergence";
			m_ruleInitializer += new RuleExecuterDelegate(ProbConvergence_m_ruleInitializer);
			m_ruleExecuter += new RuleExecuterDelegate(ProbConvergence_m_ruleExecuter);
			object o = ConfigurationManager.AppSettings["ConvergenceCheckStep"];
			m_convergencePercent = Convert.ToDouble(ConfigurationManager.AppSettings["ConversionDelta"]);

			if (o != null)
			{
				m_checkStep = Convert.ToInt32(o);
			}
		}

		void ProbConvergence_m_ruleInitializer(Dictionary<string, object> dataHashTable)
		{
			SqlUtils.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS UserScoresHistory LIKE UserScores");
			SqlUtils.ExecuteNonQuery("TRUNCATE TABLE UserScoresHistory");
			SqlUtils.ExecuteNonQuery("INSERT INTO UserScoresHistory SELECT * FROM UserScores");
		}

		void ProbConvergence_m_ruleExecuter(Dictionary<string, object> dataHashTable)
		{
			Func<int> curIterationGetter = dataHashTable["CurrentIteration"] as Func<int>;
			int curIteration = curIterationGetter();

			if ((curIteration == 0) || (curIteration % m_checkStep > 0))
			{
				return;
			}
			DataSet ds = new DataSet();
			SqlUtils.ExecuteQuery(string.Format(
@"SELECT ABS(ush.Belief/{0}.0*{1}.0/us.Belief-1) Delta 
FROM UserScoresHistory ush, UserScores us WHERE ush.Userid=us.UserId", curIteration - m_checkStep, curIteration), ds);
			int i = 0;
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				double prevScore = (double)row[0];
				double curScore = (double)row[0];
				double diff = Math.Abs(prevScore - curScore);
				if (diff < curScore)
				{
					Logger.TraceWriteLine(string.Format("{1} - Not in order on {0} row", i, curIteration));
					return;
				}
			}
			SqlUtils.ExecuteNonQuery(
				"UPDATE UserScoresHistory ush, UserScores us SET ush.Belief=us.Belief WHERE ush.userid=us.userid");
			// all is in order
			if (i > 0)
			{
				this.OnStopCleaningProcess();
			}

		}
	}
}
