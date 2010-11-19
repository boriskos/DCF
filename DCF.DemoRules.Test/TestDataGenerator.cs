using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DCF.DataLayer;
using System.Xml.Serialization;
using System.IO;

namespace DCF.DemoRules.Test
{
    public class TestDataGenerator
    {
        public TestDataGenerator(MySqlUtils utils) 
        { 
            SqlUtils = utils;
            SourceDBState = new DataSet();
            TagetDBState = new DataSet();
        }

        internal Items ItemsTemplate { get; private set; }
        internal Topics TopicsTemplate { get; private set; }
        public DataSet SourceDBState { get; private set; }
        public DataSet TagetDBState { get; private set; }
        public MySqlUtils SqlUtils { get; private set; }

        public void RebuildTopicsAndItems(string topicsTemplateFile, string itemTemplateFile)
        {
            // cleans Topics, Items and CorrectFacts Tables (creates last if necessary)
            CleanBasisTables(); 
            // loads data from XML definitions and set the state of the object with the loaded data (topics and items)
            LoadTemplatesFromXML(topicsTemplateFile, itemTemplateFile);
            // sets the dataset to contain records that were generated according to templates loaded above
            // this operation influences such tables as Topics, Items and CorrectItems
            GenerateBasisRecords();
        }

        private void GenerateBasisRecords()
        {
            DataTable dtTopics = new DataTable(TableConstants.Topics);
            DataTable dtItems = new DataTable(TableConstants.Items);
            DataTable dtCorrectFacts = new DataTable(TableConstants.CorrectFacts);

            SqlUtils.ReadTableSchema(TableConstants.Topics, dtTopics);
            SqlUtils.ReadTableSchema(TableConstants.Items, dtItems);
            SqlUtils.ReadTableSchema(TableConstants.CorrectFacts, dtCorrectFacts);

            // filling up topics
            foreach (TopicTemplate topic in TopicsTemplate.TopicList)
            {
                foreach (string tVal in topic.Values)
                {
                    dtTopics.Rows.Add(null, topic.Name, string.Format(topic.Text, tVal), 0,
                        topic.Category, 0.0, tVal);
                }
            }

            // filling up items and correct items
            foreach (ItemTemplate item in ItemsTemplate.ItemList)
            {
                foreach (string iVal in item.CorrectValues)
                {
                    dtItems.Rows.Add(null, iVal,
                        dtTopics.Select(string.Format("TopicName='{0}'", item.TopicName)).First()["TopicId"]);
                }
                dtCorrectFacts = dtItems.Copy();
                foreach (string iVal in item.IncorrectValues)
                {
                    dtItems.Rows.Add(null, iVal,
                        dtTopics.Select(string.Format("TopicName='{0}'", item.TopicName)).First()["TopicId"]);
                }
            }

            SqlUtils.RePopulateExistingTable(dtTopics);
            SqlUtils.RePopulateExistingTable(dtItems);
            SqlUtils.RePopulateExistingTable(dtCorrectFacts);
        }

        /// <summary>
        /// Commits the dataset into database
        /// </summary>
        public void CommitToDatabase()
        {
            SqlUtils.RePopulateExistingTable(TagetDBState);
        }

        /// <summary>
        /// generates ItemsMEntions and Users table
        /// </summary>
        public void SimulateUsersActivity()
        {
            // cleans Users and ItemsMentions
            CleanUserRelatedTables();

            LoadTopicsAndItems();


        }

        private void LoadTopicsAndItems()
        {
            throw new NotImplementedException();
        }

        private void CleanUserRelatedTables()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Truncates tables ItmesMention, Items, CorrectFacts and Topics
        /// </summary>
        private void CleanBasisTables()
        {
            SqlUtils.TruncateTable(TableConstants.ItemsMentions);
            SqlUtils.TruncateTable(TableConstants.Items);
            SqlUtils.TruncateTable(TableConstants.Topics);
            SqlUtils.DropTableIfExists(TableConstants.CorrectFacts);
            SqlUtils.ExecuteNonQuery("CREATE TABLE CorrectFacts AS (SELECT * FROM Items WHERE 1=0)");
        }

        /// <summary>
        /// deseriallizes two input files into Topics and Items Templates
        /// </summary>
        /// <param name="topicsTemplateFile">full file name that contains topics data</param>
        /// <param name="itemsTemplateFile">full file name that contains items data</param>
        private void LoadTemplatesFromXML(string topicsTemplateFile, string itemsTemplateFile)
        {
            XmlSerializer xmlSerializationTopics = new XmlSerializer(typeof(Topics));
            using (StreamReader reader = new StreamReader(topicsTemplateFile))
            {
                TopicsTemplate = (Topics)xmlSerializationTopics.Deserialize(reader);
            }
            XmlSerializer xmlSerializationItems = new XmlSerializer(typeof(Items));
            using (StreamReader reader = new StreamReader(itemsTemplateFile))
            {
                ItemsTemplate = (Items)xmlSerializationItems.Deserialize(reader);
            }
        }
        

        #region Previous implementation
        public static readonly string[] Countries = {  
                                "Israel",
                                "Russia",
                                "USA",
                                "England",
                                "France",
                                "Germany",
                                "Mexico",
                                "Costa Rica",
                                "Canada",
                                "Spain",
                                "Portugal",
                                "Italy",
                                "Greece",
                                "Turkey"
                             };
        public static readonly string[] Capitals = {   
                                "Jerusalem",
                                "Moscow",
                                "Washington",
                                "London",
                                "Paris",
                                "Berlin",
                                "Mexico",
                                "San Jose",
                                "Ottawa",
                                "Madrid",
                                "Lisbon",
                                "Rome",
                                "Athens",
                                "Ankara"

                            };
        public static readonly string[] NonCapitals = { "Tel Aviv", "Haifa", "Ber Sheva", "Ariel",
                                  "St. Petersburg", "Vladivastok", "Orel", "Novgorod",
                                  "New York", "Seattle", "San Francisco", "San Diego",
                                  "Liverpool", "Glasgow", "Edinburg", "Oxford",
                                  "Nice", "Cannes", "Lyon", "Saint Tropez",
                                  "Munich", "Frankfurt", "Hamburg", "Dresden",
                                  "Tijuana", "La Paz", "Guadalajara",
                                  "Vancouver", "Montreal", "Victoria", "Toronto",
                                  "Arenal", "La Fortuna",
                                  "Barcelona", "Malaga", "Sevilla", "Valencia",
                                  "Porto", "Sintra",
                                  "Milano", "Venecia", "Neapole", "Pizza",
                                  "Thesaloniki", "Patra",
                                  "Antalya", "Istanbul", "Izmir"
                                                };
        class User
        {
            public static Random rnd = new Random();
            static int userNum = 0;
            public User(double confidence)
            {
                Confidence = confidence;
                m_categoriesList = new int[Countries.Length];
                for (int i = 0; i < Countries.Length; i++)
                    m_categoriesList[i] = i;
                m_categoriesList.OrderBy(i => rnd.NextDouble()); // randomize
            }
            public double Confidence { get; set; }

            public bool GenerateFact(out Pair<string, string> fact)
            {
                string result = null;
                double rndNum = rnd.NextDouble();
                int category = GetCategory();
                int numOfWrongAnswers = NonCapitals.Length;
                if (CleaningConfiguration.NumberOfCountriesWithRestrictedIncorrectFactsCount > 0 &&
                    CleaningConfiguration.NumberOfIncorrectFactsInUse > 0 &&
                    CleaningConfiguration.NumberOfCountriesWithRestrictedIncorrectFactsCount > category)
                {
                    numOfWrongAnswers =
                        Math.Min(CleaningConfiguration.NumberOfIncorrectFactsInUse, numOfWrongAnswers);
                }

                bool correctness = rndNum <= Confidence;
                if (correctness)
                {
                    result = Capitals[category];
                }
                else
                {
                    result = NonCapitals[rnd.Next(numOfWrongAnswers)];
                }
                fact = new Pair<string, string>(Countries[category], result);
                return correctness;
            }

            public bool GenerateMayorFact(out Pair<string, string> fact, string city)
            {
                string result = null;
                double rndNum = rnd.NextDouble();
                int category = GetCategory();
                int numOfWrongAnswers = NonCapitals.Length;
                if (CleaningConfiguration.NumberOfCountriesWithRestrictedIncorrectFactsCount > 0 &&
                    CleaningConfiguration.NumberOfIncorrectFactsInUse > 0 &&
                    CleaningConfiguration.NumberOfCountriesWithRestrictedIncorrectFactsCount > category)
                {
                    numOfWrongAnswers =
                        Math.Min(CleaningConfiguration.NumberOfIncorrectFactsInUse, numOfWrongAnswers);
                }

                bool correctness = rndNum <= Confidence;
                if (correctness)
                {
                    result = string.Format("M_{0}", city);
                }
                else
                {
                    string wcity;
                    while ((wcity = NonCapitals[rnd.Next(numOfWrongAnswers <= 1 ? 2 : numOfWrongAnswers)]).CompareTo(city) == 0) ;
                    result = string.Format("M_{0}", wcity);
                }
                fact = new Pair<string, string>(city, result);
                return correctness;
            }

            public string GenerateName()
            {
                return string.Format("Conf_{0}_{1}", Confidence, userNum++);
            }

            private int GetCategory()
            {
                int res = m_categoriesList[m_curCategory];
                m_curCategory = (m_curCategory + 1) % Countries.Length;
                return res;
            }

            private int[] m_categoriesList = null;
            private int m_curCategory = 0;
        }

        /// <summary>
        /// Populates the database with users and their facts
        /// </summary>
        /// <param name="numOfFacts"></param>
        /// <param name="noiseRatio"></param>
        /// <param name="sqlUtils"></param>
        public static void PopulateUsersAndTheirFacts(long numOfFacts, double correctRatio, MySqlUtils sqlUtils)
        {
            // initialize statistics
            int statscount = CleaningConfiguration.GenerateMayors ? 2 * Countries.Length + NonCapitals.Length : Countries.Length;
            Dictionary<int, int> userConfidenceMap = new Dictionary<int, int>(Countries.Length + 1);
            for (int i = 0; i <= statscount; i++)
            {
                userConfidenceMap[i] = 0;
            }
            // initialize local variables
            int numOfUsers = 0;
            long numOfPopulatedFacts = 0;
            long numOfCorrectFacts = 0;
            int numOfCountries = Countries.Length;

            DataTable usersTable = new DataTable();
            DataTable userCapitalsTable = new DataTable();
            DataTable userMayorsTable = new DataTable();
            sqlUtils.ReadTableSchema(TableConstants.Users, usersTable);
            sqlUtils.ReadTableSchema(TableConstants.UserCapitals, userCapitalsTable);
            sqlUtils.ReadTableSchema(TableConstants.UserMayors, userMayorsTable);

            //int difFactor = 0;
            User userType = new User(correctRatio);
            while (true)
            {
                numOfUsers++;
                double currentRatio = ((double)numOfCorrectFacts) / numOfFacts;
                User u = userType;
                int newUserId = CreateUser(u, usersTable);
                int curNumOfCorrectFacts =
                    GenerateFacts(newUserId, u, numOfCountries, userCapitalsTable);
                numOfCorrectFacts += curNumOfCorrectFacts;
                numOfPopulatedFacts += numOfCountries;
                // Mayors population
                if (CleaningConfiguration.GenerateMayors)
                {
                    int curNumOfCorrectMayorFacts =
                        GenerateMayorFacts(newUserId, u, NonCapitals.Length + Capitals.Length, userMayorsTable);
                    numOfCorrectFacts += curNumOfCorrectMayorFacts;
                    numOfPopulatedFacts += NonCapitals.Length + Capitals.Length;
                    // for statistics
                    curNumOfCorrectFacts += curNumOfCorrectMayorFacts;
                }
                // count stats
                userConfidenceMap[curNumOfCorrectFacts] = userConfidenceMap[curNumOfCorrectFacts] + 1;

                if (numOfPopulatedFacts >= numOfFacts) break; // exit the loop when the number of facts is sufficient
            }
            Trace.WriteLine(string.Format("Generated {0} facts, {1} of them correct. Correct facts ratio is {2}. " +
                "Total number of users is {3}",
                numOfPopulatedFacts, numOfCorrectFacts, ((double)numOfCorrectFacts) / numOfFacts, numOfUsers));
            Debug.Write("Starting insert to database...");
            using (new PerformanceCounter("Initiallize_insert"))
            {
                generateCorrectFactsTable(sqlUtils);
                sqlUtils.RePopulateExistingTable(usersTable);
                sqlUtils.RePopulateExistingTable(userCapitalsTable);
                if (CleaningConfiguration.GenerateMayors)
                {
                    sqlUtils.RePopulateExistingTable(userMayorsTable);
                }
            }
            Debug.WriteLine("Done!");
            Debug.WriteLine(string.Format("Write operation report: {0}",
                PerformanceCounterStatic.ReportTimer("Initiallize_insert")));
            Trace.WriteLine("User confidence report:");
            foreach (var record in userConfidenceMap)
            {
                Trace.WriteLine(string.Format(
                    "\t{0,2} correct answers gave {1,3} users ({2:P})",
                    record.Key, record.Value, (double)record.Value / numOfUsers));
            }
        }

        /// <summary>
        /// Creates and populates the table CorrectFacts with correct facts
        /// </summary>
        private static void generateCorrectFactsTable(MySqlUtils sqlUtils)
        {
            // correct facts
            sqlUtils.ExecuteNonQuery(string.Format(
                "CREATE TABLE IF NOT EXISTS {0} ( " +
                "Country varchar(50) NOT NULL, " +
                "City varchar(100) NOT NULL) ",
                TableConstants.CorrectFacts));
            DataTable tbl = new DataTable();
            sqlUtils.ReadTableSchema(TableConstants.CorrectFacts, tbl);
            for (int i = 0; i < Capitals.Length; i++)
            {
                tbl.Rows.Add(Countries[i], Capitals[i]);
            }
            sqlUtils.RePopulateExistingTable(tbl);
            if (CleaningConfiguration.GenerateMayors)
            {
                // capital Correct facts
                sqlUtils.DropTableIfExists("Capital_" + TableConstants.CorrectFacts);
                string capitalStmnt = string.Format(
                    "CREATE TABLE IF NOT EXISTS {0}{1} AS " +
                    "(SELECT * FROM {1})",
                    "Capital_", TableConstants.CorrectFacts);
                sqlUtils.ExecuteNonQuery(capitalStmnt);

                // mayor correct facts
                sqlUtils.DropTableIfExists("Mayor_" + TableConstants.CorrectFacts);
                string mayorStmnt = string.Format(
                    "CREATE TABLE IF NOT EXISTS {0} AS " +
                    "(SELECT City, CONCAT('M_', City) AS Mayor FROM {1})",
                    "Mayor_" + TableConstants.CorrectFacts, TableConstants.UserMayors);
                sqlUtils.ExecuteNonQuery(mayorStmnt);
            }
        }

        /// <summary>
        /// builds portfolio for the user and generates facts according to it
        /// </summary>
        /// <param name="numOfUsers"></param>
        /// <param name="maxNumOfFactsEach"></param>
        /// <param name="sqlUtils"></param>
        public static void PopulateUsersAndTheirFacts(int numOfUsers, int maxNumOfFactsEach, MySqlUtils sqlUtils)
        {
            int[] userNumPerType = { (int)(numOfUsers * 0.05), 
                                       (int)(numOfUsers * 0.25), 
                                       (int)(numOfUsers * 0.65), 
                                       (int)(numOfUsers * 0.05)
                                   };
            User[] userTypes = { new User(1.0), new User(.6), new User(.3), new User(0.0) };

            // read users and userscores table schema
            DataTable usersTable = new DataTable();
            DataTable userCapitalsTable = new DataTable();
            sqlUtils.ReadTableSchema(TableConstants.Users, usersTable);
            sqlUtils.ReadTableSchema(TableConstants.UserCapitals, userCapitalsTable);

            for (int i = 0; i < userNumPerType.Length; i++)
            {
                for (int j = 0; j < userNumPerType[i]; j++)
                {
                    int newUserId = CreateUser(userTypes[i], usersTable);
                    GenerateFacts(newUserId, userTypes[i], maxNumOfFactsEach, userCapitalsTable);
                }
            }

            sqlUtils.RePopulateExistingTable(usersTable);
            sqlUtils.RePopulateExistingTable(userCapitalsTable);
        }

        public static void PopulateUsersAndTheirFacts(
            long numOfFacts,
            List<Pair<double, double>> userProfiles,
            MySqlUtils sqlUtils)
        {
            // initialize statistics
            int statscount = CleaningConfiguration.GenerateMayors ? 2 * Countries.Length + NonCapitals.Length : Countries.Length;
            Dictionary<int, int> userConfidenceMap = new Dictionary<int, int>(statscount);
            for (int i = 0; i <= statscount; i++)
            {
                userConfidenceMap[i] = 0;
            }

            double numOfUsersTotal = 1.0 * numOfFacts / Countries.Length;
            // if we need to generate mayors table number of users total 
            // will contain mayor facts too
            if (CleaningConfiguration.GenerateMayors)
            {
                numOfUsersTotal = 1.0 * numOfFacts / (2 * Countries.Length + NonCapitals.Length);
            }
            int numOfPopulatedFacts = 0;
            int numOfCorrectCapitalFacts = 0;
            int numOfCorrectMayorFacts = 0;

            // read users and userscores table schema
            DataTable usersTable = new DataTable();
            DataTable userCapitalsTable = new DataTable();
            DataTable userMayorsTable = new DataTable();
            sqlUtils.ReadTableSchema(TableConstants.Users, usersTable);
            sqlUtils.ReadTableSchema(TableConstants.UserCapitals, userCapitalsTable);
            sqlUtils.ReadTableSchema(TableConstants.UserMayors, userMayorsTable);

            // for each user
            foreach (var profile in userProfiles)
            {
                User user = new User(profile.Second);
                // this is the number of users in the group
                int numOfFactsPerUser = (int)Math.Ceiling(profile.First * numOfUsersTotal);
                for (int i = 0; i < numOfFactsPerUser; i++)
                {
                    int newUserId = CreateUser(user, usersTable);
                    int curNumOfCorrectCapFacts =
                        GenerateFacts(newUserId, user, Countries.Length, userCapitalsTable);
                    numOfCorrectCapitalFacts += curNumOfCorrectCapFacts;
                    numOfPopulatedFacts += Countries.Length;

                    int curNumOfCorrectMayorFacts = 0;
                    if (CleaningConfiguration.GenerateMayors)
                    {
                        curNumOfCorrectMayorFacts =
                            GenerateMayorFacts(newUserId, user, NonCapitals.Length + Capitals.Length, userMayorsTable);
                        numOfCorrectMayorFacts += curNumOfCorrectMayorFacts;
                        numOfPopulatedFacts += NonCapitals.Length + Capitals.Length;
                    }

                    // count stats
                    userConfidenceMap[curNumOfCorrectMayorFacts + curNumOfCorrectCapFacts] =
                        userConfidenceMap[curNumOfCorrectMayorFacts + curNumOfCorrectCapFacts] + 1;

                }
            }

            Trace.WriteLine(string.Format(
                "Generated {0} facts, {1} of them correct ({2} in Capitals, {3} in Mayors). Correct facts ratio is {2}. " +
                "Total number of users is {3}",
                numOfPopulatedFacts, numOfCorrectCapitalFacts + numOfCorrectMayorFacts,
                ((double)numOfCorrectCapitalFacts + numOfCorrectMayorFacts) / numOfPopulatedFacts, numOfUsersTotal));
            InitSection initSection = ConfigurationManager.GetSection("InitSection") as InitSection;
            Trace.WriteLineIf(CleaningConfiguration.UsersProfilesPortionBelief != null && CleaningConfiguration.UsersProfilesPortionBelief.Count > 0,
                string.Format("User Profiles as follows: {0}", initSection.UserProfiles));
            Debug.Write("Starting insert to database...");
            using (new PerformanceCounter("Initiallize_insert"))
            {
                sqlUtils.RePopulateExistingTable(usersTable);
                sqlUtils.RePopulateExistingTable(userCapitalsTable);
                if (CleaningConfiguration.GenerateMayors)
                {
                    sqlUtils.RePopulateExistingTable(userMayorsTable);
                }
                generateCorrectFactsTable(sqlUtils);
            }
            Debug.WriteLine("Done!");
            Debug.WriteLine(string.Format("Write operation report: {0}",
                PerformanceCounterStatic.ReportTimer("Initiallize_insert")));
            Trace.WriteLine("User confidence report:");
            foreach (var record in userConfidenceMap)
            {
                Trace.WriteLine(string.Format(
                    "\t{0,2} correct answers gave {1,3} users ({2:P})",
                    record.Key, record.Value, (double)record.Value / numOfUsersTotal));
            }
        }

        private static int GenerateMayorFacts(int newUserId, User user, int numberOfFactsToGenerate, DataTable userMayorsTable)
        {
            int numOfCorrectFacts = 0;
            for (int i = 0; i < numberOfFactsToGenerate; i++)
            {
                Pair<string, string> fact;
                if (i < Capitals.Length)
                {
                    numOfCorrectFacts += user.GenerateMayorFact(out fact, Capitals[i]) ? 1 : 0;
                }
                else
                {
                    numOfCorrectFacts += user.GenerateMayorFact(out fact, NonCapitals[i - Capitals.Length]) ? 1 : 0;
                }
                userMayorsTable.Rows.Add(userMayorsTable.Rows.Count, newUserId, fact.First, fact.Second);
            }
            return numOfCorrectFacts;
        }


        private static int GenerateFacts(int newUserId, User user, int numberOfFactsToGenerate, DataTable userCapitalsTable)
        {
            int numOfCorrectFacts = 0;
            for (int i = 0; i < numberOfFactsToGenerate; i++)
            {
                Pair<string, string> fact;
                numOfCorrectFacts += user.GenerateFact(out fact) ? 1 : 0;
                userCapitalsTable.Rows.Add(userCapitalsTable.Rows.Count, newUserId, fact.First, fact.Second);
            }
            return numOfCorrectFacts;
        }

        private static int CreateUser(User user, DataTable usersTable)
        {
            string userName = user.GenerateName();
            int userId = usersTable.Rows.Count + 1;
            string userCountry = Countries[User.rnd.Next(Countries.Length - 1)];
            // users table
            usersTable.Rows.Add(userId, userName, userCountry);

            return userId;
        }

        private const double DefaultUserBelief = 0.2;


        internal static void PopulateParlamentsTable(double parlamentsRatio, MySqlUtils sqlUtils)
        {
            // create the table if not exists
            string creationStmnt = string.Format("CREATE TABLE IF NOT EXISTS {0}  (" +
                "name VARCHAR(150) NOT NULL ," +
                "city VARCHAR(100) NOT NULL ," +
                "PRIMARY KEY (name) ," +
                "UNIQUE INDEX city_UNIQUE (city ASC) )", TableConstants.Parlaments);
            sqlUtils.ExecuteNonQuery(creationStmnt);
            // now populate it with capitals
            DataTable tbl = new DataTable(TableConstants.Parlaments);
            sqlUtils.ReadTableSchema(TableConstants.Parlaments, tbl);
            Random rnd = new Random();
            for (int i = 0; i < Countries.Length; i++)
            {
                if (rnd.NextDouble() <= parlamentsRatio)
                {
                    tbl.Rows.Add(Countries[i] + " Parlament", Capitals[i]);
                }
            }
            sqlUtils.RePopulateExistingTable(tbl);
        }
        #endregion 
    }
}
