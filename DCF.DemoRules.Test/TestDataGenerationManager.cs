using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using DCF.Common;
using DCF.DataLayer;
using System.Data;
using System.Collections;
using DCF.Lib;
using Wintellect.PowerCollections;

namespace DCF.DemoRules.Test
{
    public class TestDataGenerationManager: IDisposable
    {
        /// <summary>
        /// Gets the default value of generation parameters,
        /// parses the argument string and overrides default parameters where needed
        /// </summary>
        /// <param name="args"></param>
        public void ParseArgs(string[] args)
        {
            DBName = ConfigurationManager.AppSettings["DBName"];
            DBUsername = ConfigurationManager.AppSettings["DBUserName"];
            DBPassword = ConfigurationManager.AppSettings["DBPassword"];
            HostName = ConfigurationManager.AppSettings["HostName"];

            m_initSection = ConfigurationManager.GetSection("InitSection") as InitSection;
            if (m_initSection == null)
            {
                Logger.TraceWriteLine("InitSection is not found in app config file");
                Environment.Exit(1); // terminate
            }

            // override settings with command line arguments
            try
            {
                foreach (string arg in args)
                {
                    if (arg.StartsWith(ArgName("NumberOfFacts"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.NumberOfFacts = long.Parse(arg.Split('=')[1]);
                    }
                    else if (arg.StartsWith(ArgName("TopicsVariabilityProfile"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.TopicsVariabilityProfile = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("UserProfiles"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.UserProfiles = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("GenerateBasisTables"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.GenerateBasisTables = bool.Parse(arg.Split('=')[1]);
                    }
                    else if (arg.StartsWith(ArgName("ItemsDefinitionFile"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.ItemsDefinitionFile = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("TopicsDefinitionFile"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.TopicsDefinitionFile = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("UsersCount"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.UsersCount = long.Parse(arg.Split('=')[1]);
                    }
                    else if (arg.StartsWith(ArgName("NumOfTopicsToAnswer"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_initSection.NumOfTopicsToAnswer = long.Parse(arg.Split('=')[1]);
                    }

                    // update connection parameters 
                    else if (arg.StartsWith(ArgName("DBName"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        DBName = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("DBPassword"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        DBPassword = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("DBUserName"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        DBUsername = arg.Split('=')[1];
                    }
                    else if (arg.StartsWith(ArgName("HostName"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        HostName = arg.Split('=')[1];
                    }
                    else
                    {
                        Logger.TraceWriteLine(string.Format("Unknown command line parameter: {0}", arg));
                        Environment.Exit(1);
                    }
                }
            }
            catch (Exception)
            {
                Logger.TraceWriteLine("Incorrect argumets");
                Logger.TraceWriteLine("Usage: MySQLTest [parameter=value] ...");
            }
            // display the current configuration
            TraceConfiguration();
        }

        /// <summary>
        /// Initializes the generation process - connection to the database and creation of <see cref="TestDataGenerator"/>
        /// </summary>
        public void InitFlow()
        {
            m_sqlUtils = new MySqlNativeClientUtils(DBUsername, DBPassword, DBName, HostName);
            SqlUtils.Connect();
            Logger.TraceWriteLine(string.Format("Connected to {0}/{1}@{2}", DBUsername, DBPassword, DBName));
            m_testDataGenerator = new TestDataAdvancedGenerator(m_initSection); //TestDataGenerator(m_initSection);
        }

        public void DoTestFlow()
        {
            // regenerate Correct Facts table if necessary
            if (m_initSection.GenerateBasisTables)
            {
                // clean the schema (I will recreate all the tables)
                Logger.DebugWriteLine("Recreating schema tables...");
                Logger.DebugIndent();
                RecreateSchemaTables();
                Logger.DebugUnindent();
                Logger.DebugWriteLine("Done!");
            }

            DataTable dtTopics = new DataTable(TableConstants.Topics);
            DataTable dtItems = new DataTable(TableConstants.Items);
            DataTable dtCorrectFacts = new DataTable(TableConstants.CorrectFacts);
            SqlUtils.ReadTableSchema(TableConstants.Topics, dtTopics);
            SqlUtils.ReadTableSchema(TableConstants.Items, dtItems);
            SqlUtils.ReadTableSchema(TableConstants.CorrectFacts, dtCorrectFacts);

            /////////////////////////////////////////////////////////////////
            // generated the basis tables if necessary and update the database
            if (m_testDataGenerator.GenerateBasisTables(dtTopics, dtItems, dtCorrectFacts))
            {
                Logger.DebugWrite("Replacing Basic Tables in the database...");
                // ItemsMentions has a foreign key to Itmes
                SqlUtils.TruncateTable(TableConstants.ItemsMentions); 
                SqlUtils.TruncateTable(dtItems.TableName);
                SqlUtils.TruncateTable(dtCorrectFacts.TableName);
                SqlUtils.TruncateTable(dtTopics.TableName);
                Logger.DebugWrite("Topics...");
                SqlUtils.RePopulateExistingTable(dtTopics);
                Logger.DebugWrite("Items...");
                SqlUtils.RePopulateExistingTable(dtItems);
                Logger.DebugWrite("CorrectFacts...");
                SqlUtils.RePopulateExistingTable(dtCorrectFacts);
                Logger.DebugWriteLine("Done!");
            }
            else // otherwise, read the database tables
            {
                Logger.DebugWrite("Populating Basic Tables from the database...");
                SqlUtils.PopulateTableFromDB(dtItems);
                SqlUtils.PopulateTableFromDB(dtCorrectFacts);
                SqlUtils.PopulateTableFromDB(dtTopics);
                Logger.DebugWriteLine("Done!");
            }

            /////////////////////////////////////////////////////////////////
            // now simulate the user activities according to the parameters
            DataTable dtItemsMentions = new DataTable(TableConstants.ItemsMentions);
            DataTable dtUsers = new DataTable(TableConstants.Users);
            SqlUtils.ReadTableSchema(TableConstants.ItemsMentions, dtItemsMentions);
            SqlUtils.ReadTableSchema(TableConstants.Users, dtUsers);
            // clean the tables ItemsMentions and Users
            SqlUtils.TruncateTable(dtItemsMentions.TableName);
            SqlUtils.TruncateTable(dtUsers.TableName);

            Logger.DebugWrite("Populating database with users and their items mentions...");
            m_testDataGenerator.SimulateUsersActivity(
                dtTopics, dtItems, dtCorrectFacts, dtItemsMentions, dtUsers, SqlUtils);

            dtUsers = null;
            dtTopics = null;
            dtItems = null;
            dtCorrectFacts = null;
            dtItemsMentions = null; 
          
            GC.Collect();

            DataTable dtUserGraph = new DataTable(TableConstants.UserGraph);
            SqlUtils.ReadTableSchema(TableConstants.UserGraph, dtUserGraph);
            SqlUtils.TruncateTable(TableConstants.UserGraph);
            populateUserGraph(dtUserGraph, dtItems.Rows.Count);

            Logger.DebugWriteLine("Done!");
        }

        private void populateUserGraph(DataTable dtUserGraph, int num_of_items)
        {
            MySpecialBitArray[] bitAr = new MySpecialBitArray[(int)m_initSection.UsersCount];
            uint[] totalUserWeight = new uint[(int)m_initSection.UsersCount];
            for (int i = 0; i < bitAr.Length; i++ )
                bitAr[i] = new MySpecialBitArray(num_of_items);
            DataTable dt = new DataTable();
            SqlUtils.ReadTableSchema(TableConstants.ItemsMentions, dt);
            for (int i = 0; ; i += TestDataGenerator.LimitOfRows)
            {
                SqlUtils.PopulateTableFromDB(dt, string.Format("ID >= {0} AND ID < {1}",
                    i, i + TestDataGenerator.LimitOfRows));
                foreach (DataRow row in dt.Rows)
                {
                    bitAr[(int)row.Field<uint>("UserID")-1].Set(row.Field<uint>("ItemID")-1);
                }
                if (dt.Rows.Count < TestDataGenerator.LimitOfRows)
                    break;
            }
            // count them all
            Logger.TraceWriteLine("Starting UserGraph table population " + DateTime.Now.ToShortTimeString());
            uint curRow = 1;
            for (int i = 0; i < m_initSection.UsersCount; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    uint result = bitAr[i].AndCounting(bitAr[j]);
                    if (result > 0)
                    {
                        totalUserWeight[i]+=result;
                        totalUserWeight[j]+=result;
                        dtUserGraph.Rows.Add(curRow++, i + 1, j + 1, 1.0 * result );
                        dtUserGraph.Rows.Add(curRow++, j + 1, i + 1, 1.0 * result );
                        if (dtUserGraph.Rows.Count > TestDataGenerator.LimitOfRows)
                        {
                            int num = dtUserGraph.Rows.Count;
                            SqlUtils.PopulateTableToDb(dtUserGraph);
                            dtUserGraph.Rows.Clear();
                            GC.Collect();
                            Logger.TraceWriteLine(string.Format("UserGraph table populated {0} rows", num));
                        }
                    }
                }
            }
            Logger.TraceWriteLine("Finished UserGraph table population " + DateTime.Now.ToShortTimeString());
            SqlUtils.ExecuteNonQuery("ALTER TABLE usergraph ADD INDEX sourceuserid_idx (SourceUserID ASC)") ;
            Logger.TraceWriteLine("Applied index on UserGraph table " + DateTime.Now.ToShortTimeString());
            for (int i=0; i<m_initSection.UsersCount; i++)
            {
                SqlUtils.ExecuteNonQuery(
                    string.Format("UPDATE usergraph SET weight = weight/{0} WHERE SourceUserId={1}",
                    totalUserWeight[i], i));
                if (i%1000 == 0) 
                {
                    Logger.TraceWriteLine(
                        string.Format("Udated {0} users {1}", i.ToString(), DateTime.Now.ToShortTimeString()));
                }
                    
            }
            Logger.TraceWriteLine("UserGraph table creation completed " + DateTime.Now.ToShortTimeString());
        }

        private void RecreateSchemaTables()
        {

            SqlUtils.DropTableIfExists(TableConstants.ItemsMentions);
            SqlUtils.DropTableIfExists(TableConstants.Items);
            SqlUtils.DropTableIfExists(TableConstants.Topics);
            SqlUtils.DropTableIfExists(TableConstants.UserFactScores);
            SqlUtils.DropTableIfExists(TableConstants.UserGraph);
            SqlUtils.DropTableIfExists(TableConstants.Users);
            SqlUtils.DropTableIfExists(TableConstants.CorrectFacts);

            SqlUtils.ExecuteNonQuery(@"
CREATE TABLE Items (
  ItemId int(11) unsigned NOT NULL PRIMARY KEY,
  name varchar(500) NOT NULL,
  topicID int(11) unsigned NOT NULL 
) ENGINE = MyISAM");

            SqlUtils.ExecuteNonQuery(@"
CREATE TABLE  Topics  (
   TopicId  int(11) unsigned NOT NULL PRIMARY KEY,
   TopicName  varchar(200) NOT NULL,
   TopicType  smallint(1) unsigned zerofill NOT NULL
) ENGINE=MyISAM");

            SqlUtils.ExecuteNonQuery(@"
CREATE TABLE Users (
  UserID int(11) unsigned NOT NULL PRIMARY KEY ,
  UserName varchar(100) NOT NULL
) ENGINE=MyISAM");

            SqlUtils.ExecuteNonQuery(@"
CREATE TABLE  itemsmentions  (
   ID  int(11) unsigned NOT NULL PRIMARY KEY,
   userID  int(11) unsigned NOT NULL,
   itemID  int(11) unsigned NOT NULL,
   topicID  int(11) unsigned NOT NULL,
   time  timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=MyISAM");

            SqlUtils.ExecuteNonQuery(@"
CREATE TABLE  usergraph  (
   ID  int(11) unsigned NOT NULL PRIMARY KEY ,
   SourceUserID  int(11) unsigned NOT NULL,
   TargetUserID  int(11) unsigned NOT NULL,
   Weight  double NOT NULL DEFAULT '0'
) ENGINE=MyISAM");

            SqlUtils.ExecuteNonQuery("CREATE TABLE CorrectFacts AS (SELECT * FROM Items WHERE 1=0)");
        }

        public void DoTestFlowAdvanced()
        {
            // regenerate Correct Facts table if necessary
            if (m_initSection.GenerateBasisTables)
            {
                Logger.DebugWriteLine(string.Format("Recreating {0} table...", TableConstants.CorrectFacts));
                SqlUtils.DropTableIfExists(TableConstants.CorrectFacts);
                SqlUtils.ExecuteNonQuery("CREATE TABLE CorrectFacts AS (SELECT * FROM Items WHERE 1=0)");
                Logger.DebugWriteLine("Done!");
            }

            DataTable dtTopics = new DataTable(TableConstants.Topics);
            DataTable dtItems = new DataTable(TableConstants.Items);
            DataTable dtCorrectFacts = new DataTable(TableConstants.CorrectFacts);
            SqlUtils.ReadTableSchema(TableConstants.Topics, dtTopics);
            SqlUtils.ReadTableSchema(TableConstants.Items, dtItems);
            SqlUtils.ReadTableSchema(TableConstants.CorrectFacts, dtCorrectFacts);

            /////////////////////////////////////////////////////////////////
            // generated the basis tables if necessary and update the database
            if (m_testDataGenerator.GenerateBasisTables(dtTopics, dtItems, dtCorrectFacts))
            {
                Logger.DebugWrite("Replacing Basic Tables in the database...");
                // ItemsMentions has a foreign key to Itmes
                SqlUtils.TruncateTable(TableConstants.ItemsMentions); 
                SqlUtils.TruncateTable(dtItems.TableName);
                SqlUtils.TruncateTable(dtCorrectFacts.TableName);
                SqlUtils.TruncateTable(dtTopics.TableName);

                SqlUtils.RePopulateExistingTable(dtTopics);
                SqlUtils.RePopulateExistingTable(dtItems);
                SqlUtils.RePopulateExistingTable(dtCorrectFacts);
                Logger.DebugWriteLine("Done!");
            }
            else // otherwise, read the database tables
            {
                Logger.DebugWrite("Populating Basic Tables from the database...");
                SqlUtils.PopulateTableFromDB(dtItems);
                SqlUtils.PopulateTableFromDB(dtCorrectFacts);
                SqlUtils.PopulateTableFromDB(dtTopics);
                Logger.DebugWriteLine("Done!");
            }

            /////////////////////////////////////////////////////////////////
            // now simulate the user activities according to the parameters
            DataTable dtItemsMentions = new DataTable(TableConstants.ItemsMentions);
            DataTable dtUsers = new DataTable(TableConstants.Users);
            SqlUtils.ReadTableSchema(TableConstants.ItemsMentions, dtItemsMentions);
            SqlUtils.ReadTableSchema(TableConstants.Users, dtUsers);
            // clean the tables ItemsMentions and Users
            SqlUtils.TruncateTable(dtItemsMentions.TableName);
            SqlUtils.TruncateTable(dtUsers.TableName);

            m_testDataGenerator.SimulateUsersActivity(
                dtTopics, dtItems, dtCorrectFacts, dtItemsMentions, dtUsers, SqlUtils);
            // commit changes into the database
            Logger.DebugWrite("Populating database with users and their items mentions...");

            // users are also inserted in SqlUtils.RePopulateExistingTable(dtUsers);
            // this table is already been populated SqlUtils.RePopulateExistingTable(dtItemsMentions);
            Logger.DebugWriteLine("Done!");
        }

        public void FinishFlow()
        {
            Dispose();
        }

        #region Disposable Pattern
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_sqlUtils != null)
                {
                    m_sqlUtils.Dispose();
                    m_sqlUtils = null;
                }
            }
        }
        #endregion

        #region Public Properties

        public  string DBUsername { get; private set; }
        public string DBPassword { get; private set; }
        public string DBName { get; private set; }
        public string HostName { get; private set; }

        public MySqlUtils SqlUtils { get { return m_sqlUtils; } }

        #endregion 

        #region Private Helping methods

        private static string ArgName(string name) 
        { 
            return "/" + name.ToLower(); 
        }

        private void TraceConfiguration()
        {
            Logger.TraceWriteLine("Init Section for current run is:");
            Logger.TraceIndent();
            m_initSection.TraceContents();
            Logger.TraceUnindent();
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////
        //"ShowSQL",
        //"ShowTableContents",
        //"DBUserName",
        //"DBPassword",
        //"DBName",
        //"NumberOfFacts",
        //"NumberOfIncorrectFactsInUse",
        //"GenerateBasisTables",
        //"UserProfiles",
        //"ItemsDefinitionFile",
        //"TopicsDefinitionFile"

        private InitSection m_initSection = null;
        private MySqlUtils m_sqlUtils = null;
        private TestDataGenerator m_testDataGenerator = null;

    }
}
