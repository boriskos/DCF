using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DCF.Common;
using Wintellect.PowerCollections;

namespace DCF.DemoRules.Test
{
    public class TestDataAdvancedGenerator : TestDataGenerator
    {
        public TestDataAdvancedGenerator(InitSection initSection) :
            base(initSection)
        {
        }

        public new bool GenerateBasisTables(DataTable dtTopics, DataTable dtItems, DataTable dtCorrectFacts)
        {
            if (InitSection.GenerateBasisTables)
            {
                Items items;
                Topics topics;
                // loads data from XML definitions and set the state of the object with the loaded data (topics and items)
                GenerateTopicsAndItems(out items, out topics);
                // sets the dataset to contain records that were generated according to templates loaded above
                // this operation influences such tables as Topics, Items and CorrectItems
                GenerateBasisRecords(dtTopics, dtItems, dtCorrectFacts, items, topics);
            }
            return InitSection.GenerateBasisTables;
        }

        //public void SimulateUsersActivity(
        //    DataTable dtTopics, DataTable dtItems,
        //    DataTable dtCorrectFacts, DataTable dtItemsMentions, DataTable dtUsers,
        //    MySqlUtils sqlUtils)
        //{
        //    Logger.DebugWriteLine("Simulating users activity...");
        //    Logger.DebugIndent();
        //    Random userRnd = new Random();
        //    Random iterationRnd = new Random();
        //    Random confidenceRnd = new Random();
        //    List<Pair<double, double>> userProfile = InitSection.GetUserProfiles();
        //    long currentFactsNum = 0;
        //    int topicsCount = (from r in dtItems.AsEnumerable() select r.Field<uint>("TopicId")).Distinct().Count();
        //    Dictionary<int, int> topicId2CorrectCount = CreateTopicId2CorrectCountMap(dtCorrectFacts);
        //    int correctFactsCount = dtCorrectFacts.Rows.Count;
        //    int numberOfUsers = (int)m_initSection.NumberOfFacts / topicsCount + 10;
        //    List<User> listOfUsers = new List<User>(numberOfUsers);
        //    int currentUser = 0;
        //    Dictionary<int, int> restrictedTopics =
        //        DrawRestrictedTopics(m_initSection.GetTopicsVariabilityProfiles(), topicsCount);
        //    // stats
        //    int[] answerStats = new int[2] { 0, 0 };

        //    // first populate the users
        //    for (int i = 0; i < numberOfUsers; i++)
        //    {
        //        CreateUser(userRnd, userProfile, listOfUsers, topicsCount);
        //    }
        //    PopulateUsersTable(listOfUsers, dtUsers);
        //    sqlUtils.RePopulateExistingTable(dtUsers); // add users into DB

        //    // now populate items mentions
        //    while (currentFactsNum < m_initSection.NumberOfFacts)
        //    {
        //        int fixedUser = currentUser + numberOfUsers;
        //        User user = listOfUsers[currentUser++ % numberOfUsers];

        //        // get a topic and move it to used
        //        int topicIndex;
        //        Logger.Assert(user.GetNextTopic(out topicIndex), "User is exhausted");

        //        DataRow topicRow = dtTopics.Rows[topicIndex];
        //        int topicId = (int)topicRow.Field<uint>("TopicId");

        //        // define iteration for the current topic type
        //        int iterationCount = 1;
        //        if (TopicType.MultipleChoiseAnswer == (TopicType)topicRow.Field<UInt16>("TopicType"))
        //        {
        //            iterationCount = iterationRnd.Next(
        //                topicId2CorrectCount[topicId]);
        //        }
        //        int[] answers = new int[2] { 0, 0 };
        //        for (int i = 0; i < iterationCount; i++)
        //        {
        //            double randomNumber = confidenceRnd.NextDouble();
        //            if (randomNumber <= user.Confidence)
        //            {
        //                answers[0]++; // correct
        //            }
        //            else
        //            {
        //                answers[1]++; // incorrect
        //            }
        //        }
        //        if (answers[0] > 0) // correct answers
        //            PopulateCorrectAnswers(dtCorrectFacts, dtItemsMentions, user, topicId, answers[0]);
        //        if (answers[1] > 0) // incorrect answers
        //            populateIncorrectAnswers(dtItems, dtItemsMentions, user, topicId,
        //                topicId2CorrectCount[topicId], answers[1], restrictedTopics);
        //        currentFactsNum += answers[0] + answers[1];
        //        // stats
        //        answerStats[0] += answers[0];
        //        answerStats[1] += answers[1];

        //        if (dtItemsMentions.Rows.Count > LimitOfRows)
        //        {
        //            sqlUtils.PopulateTableToDb(dtItemsMentions);
        //            dtItemsMentions.Rows.Clear();
        //        }
        //    }
        //    sqlUtils.PopulateTableToDb(dtItemsMentions); // populate the rest of the items mention
        //    Logger.TraceWriteLine(string.Format(
        //        "Database contain {0} facts on {1} topics with {2} correct and {3} incorrect facts - DB correct ratio is {4}",
        //        currentFactsNum, topicsCount, answerStats[0], answerStats[1], answerStats[0] * 1.0 / currentFactsNum));
        //    Logger.DebugUnindent();
        //    Logger.DebugWriteLine("Done!");
        //}

        #region Private Helping methods
        private void GenerateTopicsAndItems(out Items items, out Topics topics)
        {
            topics = new Topics();
            items = new Items();
            Random rnd = new Random();
            var profiles = InitSection.GetTopicsSpecialVariabilityProfiles();
            foreach(var profile in profiles)
            {
                // first is number of topics
                for(int i=0; i<profile.First; i++)
                {
                    TopicTemplate topic = new TopicTemplate();
                    topic.Category = i.ToString();
                    topic.Name = "Topic "+topic.Category;
                    topic.Text = "Generated " + topic.Name;
                    topic.Type = DCF.DataLayer.TopicType.SingleAnswer;
                    topics.TopicList.Add(topic);

                    // now create the set of items for the topic
                    int itemsnum = rnd.Next(profile.Second, profile.Third);
                    ItemTemplate item = new ItemTemplate();
                    item.CorrectValues.Add("t_" + i.ToString());
                    item.TopicName = topic.Name;
                    for (int j = 0; j < itemsnum; j++)
                    {
                        item.IncorrectValues.Add(string.Format("f{0}_{1}", i.ToString(), j.ToString()));
                    }
                    items.ItemList.Add(item);
                }
            }
        }

        #endregion
    }
}
