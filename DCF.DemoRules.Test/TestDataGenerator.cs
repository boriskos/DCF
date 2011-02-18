using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DCF.DataLayer;
using System.Xml.Serialization;
using System.IO;
using Wintellect.PowerCollections;
using DCF.Common;

namespace DCF.DemoRules.Test
{
    public class TestDataGenerator
    {
        const int LimitOfRows = 100000;

        private class User
        {
            public static Random rnd = new Random();
            static int userNum = 1;
            public User(double confidence, int topicsCount)
            {
                Confidence = confidence;
                Id = userNum++;
                Name = string.Format("User_{0}_{1}", Confidence, Id);
                m_topicsCount = topicsCount;
                m_categoriesList = new int[topicsCount];
                for (int i = 0; i < topicsCount; i++)
                    m_categoriesList[i] = i;
                m_categoriesList = m_categoriesList.OrderBy(i => rnd.NextDouble()).ToArray(); // randomize
            }
            public double Confidence { get; private set; }
            public int Id { get; private set; }
            public string Name { get; private set; }

            public bool GetNextTopic(out int topicIndex) 
            {
                topicIndex = 0;
                if (m_topicsCount > m_curCategory)
                {
                    topicIndex = m_categoriesList[m_curCategory];
                    m_curCategory++;
                    return true;
                }
                return false;
            }

            private int[] m_categoriesList = null;
            private int m_curCategory = 0;
            private int m_topicsCount;
        }

        public TestDataGenerator(InitSection m_initSection)
        {
            this.m_initSection = m_initSection;
        }

        public bool GenerateBasisTables(DataTable dtTopics, DataTable dtItems, DataTable dtCorrectFacts)
        {
            if (m_initSection.GenerateBasisTables)
            {
                string topicsTemplateFile = m_initSection.TopicsDefinitionFile;
                string itemTemplateFile = m_initSection.ItemsDefinitionFile;

                Items items;
                Topics topics;
                // loads data from XML definitions and set the state of the object with the loaded data (topics and items)
                LoadTemplatesFromXML(topicsTemplateFile, itemTemplateFile, out items, out topics);
                // sets the dataset to contain records that were generated according to templates loaded above
                // this operation influences such tables as Topics, Items and CorrectItems
                GenerateBasisRecords(dtTopics, dtItems, dtCorrectFacts, items, topics);
            }
            return m_initSection.GenerateBasisTables;
        }

        /// <summary>
        /// generates ItemsMEntions and Users table
        /// </summary>
        /// <remarks>
        /// The algorithm of User activity emulation is as follows:
        /// 1) create a User with probability p to give correct answer. (p is choosen according to users profiles)
        /// 2) draw a topic at random from topics that where not choosen by this user
        /// 3.0) choose a possible item for this topic with probability p from Correct Facts table and 
        /// with probability of (1-p) from Items fact that is not correct
        /// 3.1) for topic with multiple answers choose randomly n from the number possible correct answers and 
        /// run (3.0) n times for current topic
        /// 4) add the new row to ItemsMentions table
        /// </remarks>
        public void SimulateUsersActivity(
            DataTable dtTopics, DataTable dtItems,
            DataTable dtCorrectFacts, DataTable dtItemsMentions, DataTable dtUsers,
            MySqlUtils sqlUtils)
        {
            Logger.DebugWriteLine("Simulating users activity...");
            Logger.DebugIndent();
            Random userRnd = new Random();
            Random iterationRnd = new Random();
            Random confidenceRnd = new Random();
            List<Pair<double, double>> userProfile = m_initSection.GetUserProfiles();
            long currentFactsNum = 0;
            int topicsCount = (from r in dtItems.AsEnumerable() select r.Field<uint>("TopicId")).Distinct().Count();
            Dictionary<int, int> topicId2CorrectCount = CreateTopicId2CorrectCountMap(dtCorrectFacts);
            int correctFactsCount = dtCorrectFacts.Rows.Count;
            int numberOfUsers = (int)m_initSection.NumberOfFacts / topicsCount + 10;
            List<User> listOfUsers = new List<User>(numberOfUsers);
            int currentUser = 0;
            Dictionary<int, int> restrictedTopics = 
                DrawRestrictedTopics(m_initSection.GetTopicsVariabilityProfiles(), topicsCount);
            // stats
            int[] answerStats = new int[2] { 0, 0 };

            // first populate the users
            for (int i = 0; i < numberOfUsers; i++)
            {
                CreateUser(userRnd, userProfile, listOfUsers, topicsCount);
            }
            PopulateUsersTable(listOfUsers, dtUsers);
            sqlUtils.RePopulateExistingTable(dtUsers); // add users into DB

            // now populate items mentions
            while (currentFactsNum < m_initSection.NumberOfFacts)
            {
                int fixedUser = currentUser + numberOfUsers;
                User user = listOfUsers[currentUser++ % numberOfUsers];

                // get a topic and move it to used
                int topicIndex;
                Logger.Assert(user.GetNextTopic(out topicIndex), "User is exhausted");

                DataRow topicRow = dtTopics.Rows[topicIndex];
                int topicId = (int)topicRow.Field<uint>("TopicId");

                // define iteration for the current topic type
                int iterationCount = 1;
                if (TopicType.MultipleChoiseAnswer == (TopicType)topicRow.Field<UInt16>("TopicType"))
                {
                    iterationCount = iterationRnd.Next(
                        topicId2CorrectCount[topicId]);
                }
                int [] answers = new int[2] {0, 0};
                for (int i = 0; i < iterationCount; i++)
                {
                    double randomNumber = confidenceRnd.NextDouble();
                    if (randomNumber <= user.Confidence)
                    {
                        answers[0]++; // correct
                    }
                    else
                    {
                        answers[1]++; // incorrect
                    }
                }
                if (answers[0] > 0) // correct answers
                    PopulateCorrectAnswers(dtCorrectFacts, dtItemsMentions, user, topicId, answers[0]);
                if (answers[1] > 0) // incorrect answers
                    populateIncorrectAnswers(dtItems, dtItemsMentions, user, topicId, 
                        topicId2CorrectCount[topicId], answers[1], restrictedTopics);
                currentFactsNum += answers[0] + answers[1];
                // stats
                answerStats[0] += answers[0]; 
                answerStats[1] += answers[1];

                if (dtItemsMentions.Rows.Count > LimitOfRows)
                {
                    sqlUtils.PopulateTableToDb(dtItemsMentions);
                    dtItemsMentions.Rows.Clear();
                }
            }
            sqlUtils.PopulateTableToDb(dtItemsMentions); // populate the rest of the items mention
            Logger.TraceWriteLine(string.Format(
                "Database contain {0} facts on {1} topics with {2} correct and {3} incorrect facts - DB correct ratio is {4}",
                currentFactsNum, topicsCount, answerStats[0], answerStats[1], answerStats[0] * 1.0 / currentFactsNum));
            Logger.DebugUnindent();
            Logger.DebugWriteLine("Done!");
        }

        public InitSection InitSection { get { return m_initSection; } }

        #region Private Helping Methods

        private Dictionary<int, int> DrawRestrictedTopics(List<Pair<int, int>> list, int topicsCount)
        {
            Dictionary<int, int> restrictedTopics = new Dictionary<int, int>();
            Random rndTopics = new Random();
            int j = 0;
            Logger.TraceWriteLine("Restricted Topices are:");
            Logger.TraceIndent();
            foreach (var pair in list)
            {
                Logger.TraceWrite(string.Format(
                    "The following topics have {0} possible incorrect answers: ", pair.Second));

                // for given number of topics choose them and set their restricted values
                for (int i = 0; i < pair.First; i++)
                {
                    Logger.Assert(j++ < topicsCount, "Topic Count is less than number of Topic Restrictions");
                    // get next unrestricted topic
                    int nextTopic;
                    while (restrictedTopics.ContainsKey(nextTopic = rndTopics.Next(topicsCount))) ;
                    restrictedTopics[nextTopic] = pair.Second;
                    Logger.TraceWrite(string.Format("{0}, ", nextTopic));
                }
                Logger.TraceWriteLine(string.Empty);
            }
            Logger.TraceUnindent();
            return restrictedTopics;
        }
        /// <summary>
        /// Converst list of users to Users Table
        /// </summary>
        /// <param name="listOfUsers">users to insert into the database</param>
        /// <param name="dtUsers">table to be populated with users</param>
        private void PopulateUsersTable(List<User> listOfUsers, DataTable dtUsers)
        {
            foreach (User user in listOfUsers)
            {
                dtUsers.Rows.Add( user.Id, string.Format("{0}@tau.ac.il", user.Name), 0, DateTime.Now, 0, 0, user.Name);
            }
        }

        private void CreateUser(Random userRnd, List<Pair<double, double>> userProfile, List<User> listOfUsers, int topicsCount)
        {
            // create user
            User user = new User(DrawConfidence(userRnd.NextDouble(), userProfile), topicsCount);
            listOfUsers.Add(user);
        }

        private static void populateIncorrectAnswers(
            DataTable dtItems, 
            DataTable dtItemsMentions, 
            User user, 
            int topicId, 
            int correcFactsCount, 
            int answersCount,
            Dictionary<int, int> restrictedTopics)
        {
            Random rnd = new Random();
            var allAnswers = from r in dtItems.AsEnumerable()
                             where r.Field<uint>("TopicID") == topicId
                             orderby r.Field<uint>("ItemId") ascending
                             select new { ItemId = r.Field<uint>("ItemId") };
            int lowId = (int)allAnswers.First().ItemId + correcFactsCount;
            int highId;
            if (restrictedTopics.TryGetValue(topicId, out highId))
            {
                Logger.Assert(highId >= answersCount, 
                    string.Format("Restriction is too tight: for user {0} who should choose {1} answers with restriction of {2} possible wrong answers",
                    user, answersCount, highId));
                highId += lowId-1;
            }
            else
            {
                highId = int.MaxValue;
            }
            var incorrectAnswers = from r in allAnswers
                                   where (lowId <= r.ItemId && r.ItemId <= highId)
                                   orderby rnd.NextDouble()
                                   select r;
            foreach (var incorrectItem in incorrectAnswers)
            {
                dtItemsMentions.Rows.Add(m_sMentionId++, user.Id, incorrectItem.ItemId, DateTime.Now);
                if (--answersCount == 0) break; // stop after we add answers[1] of incorrect answers
            }
        }

        private static void PopulateCorrectAnswers(
            DataTable dtCorrectFacts, 
            DataTable dtItemsMentions, 
            User user, 
            int topicId, 
            int answersCount)
        {
            Random rnd = new Random();
            var correctAnswers = from r in dtCorrectFacts.AsEnumerable()
                                 where r.Field<uint>("TopicID") == topicId
                                 orderby rnd.NextDouble()
                                 select new { ItemId = r.Field<uint>("ItemId") };
            foreach (var correctItem in correctAnswers)
            {
                dtItemsMentions.Rows.Add(m_sMentionId++, user.Id, correctItem.ItemId, DateTime.Now);
                if (--answersCount == 0) break; // stop after we add answersCount of correct answers
            }
        }

        /// <summary>
        /// Creates map of topic id to number of correct answers
        /// </summary>
        /// <param name="dtCorrectFacts"></param>
        /// <returns></returns>
        private Dictionary<int, int> CreateTopicId2CorrectCountMap(DataTable dtCorrectFacts)
        {
            Dictionary<int, int> res = new Dictionary<int,int>();
            var answer = from r in dtCorrectFacts.AsEnumerable()
                         group r by r.Field<uint>("TopicID") into g
                         select new
                         {
                             TopicId = g.Key,
                             AnswerCount = g.Count()
                         };
            foreach (var line in answer)
                res[(int)line.TopicId] = line.AnswerCount;
            return res;
        }

        /// <summary>
        /// Given a random number and user profiles chooses a profile form the list and 
        /// returns user's confidence from the profile
        /// </summary>
        private double DrawConfidence(double p, List<Pair<double, double>> userProfile)
        {
            foreach (var profile in userProfile)
            {
                if (profile.First >= p) return profile.Second;
                p -= profile.First;
            }
            throw new System.Configuration.ConfigurationErrorsException("User Profiles are not valid"); 
        }

        /// <summary>
        /// deseriallizes two input files into Topics and Items Templates
        /// </summary>
        /// <param name="topicsTemplateFile">full file name that contains topics data</param>
        /// <param name="itemsTemplateFile">full file name that contains items data</param>
        private void LoadTemplatesFromXML(string topicsTemplateFile, string itemsTemplateFile,
            out Items items, out Topics topics)
        {
            items = null;
            topics = null;
            // TODO: XmlSerializers module is not found
            XmlSerializer xmlSerializationTopics = new XmlSerializer(typeof(Topics));
            using (StreamReader reader = new StreamReader(topicsTemplateFile))
            {
                topics = (Topics)xmlSerializationTopics.Deserialize(reader);
            }
            Logger.DebugWriteLine(string.Format("{1} Topics are sucessfully loaded from {0}", topicsTemplateFile, topics.TopicList.Count));
            XmlSerializer xmlSerializationItems = new XmlSerializer(typeof(Items));
            using (StreamReader reader = new StreamReader(itemsTemplateFile))
            {
                items = (Items)xmlSerializationItems.Deserialize(reader);
            }
            Logger.DebugWriteLine(string.Format("{1} Items are sucessfully loaded from {0}", itemsTemplateFile, items.ItemList.Count));
        }

        protected void GenerateBasisRecords(
            DataTable dtTopics, DataTable dtItems, DataTable dtCorrectFacts,
            Items items, Topics topics)
        {
            Logger.DebugWriteLine("Generating Basic Records...");
            // filling up topics
            int topicId = 1;
            foreach (TopicTemplate topic in topics.TopicList)
            {
                if (topic.Values == null || topic.Values.Count == 0)
                {
                    dtTopics.Rows.Add(topicId++, topic.Name, 
                        topic.Text, 0, topic.Category, 0.0, String.Empty);
                }
                else
                {
                    foreach (string tVal in topic.Values)
                    {
                        dtTopics.Rows.Add(topicId++, string.Format(topic.Name, tVal), 
                            string.Format(topic.Text, tVal), 0, topic.Category, 0.0, tVal);
                    }
                }
            }

            // filling up items and correct items
            int itemId = 1;
            foreach (ItemTemplate item in items.ItemList)
            {
                foreach (string iVal in item.CorrectValues)
                {
                    // popultate items and correct items at once
                    dtCorrectFacts.Rows.Add(
                        dtItems.Rows.Add(itemId++, iVal,
                            dtTopics.Select(string.Format("TopicName='{0}'", item.TopicName)).First()["TopicId"]).ItemArray
                            );
                }
                foreach (string iVal in item.IncorrectValues)
                {
                    dtItems.Rows.Add(itemId++, iVal,
                        dtTopics.Select(string.Format("TopicName='{0}'", item.TopicName)).First()["TopicId"]);
                }
            }
            Logger.DebugWriteLine("Done!");
        }

        #endregion
        
        private InitSection m_initSection;
        private static uint m_sMentionId = 1;
    }
}
