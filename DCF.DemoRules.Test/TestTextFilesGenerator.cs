using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using DCF.Common;
using Wintellect.PowerCollections;
using System.Diagnostics;
using System.IO;

namespace DCF.DemoRules.Test
{
    public class TestTextFilesGenerator
    {
        public void ParseArgs(string[] args)
        {
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
                    else if (arg.StartsWith(ArgName("WorkingDirectory"), StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_workingDirectory = arg.Split('=')[1];
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

            m_bitAr = new MySpecialBitArray[(int)m_initSection.UsersCount];
            m_totalUserWeight = new int[(int)m_initSection.UsersCount];

            // display the current configuration
            TraceConfiguration();
        }

        public void GenerateState()
        {
            // here we simulate user activity
            List<Triple<int, int, int>> list = m_initSection.GetTopicsSpecialVariabilityProfiles();
            Trace.Assert(list.Count==1);
            Trace.Assert(list[0].Second==list[0].Third);
            m_items_num_per_topic = list[0].Second;
            m_topics_num = list[0].First;
            List<Pair<double, double>> userProfiles = m_initSection.GetUserProfiles();
            Trace.Assert(userProfiles.Count==1);
            double userConfidence = userProfiles[0].Second;
            uint correctAnswerCount = generateUsers(0, (int)m_initSection.UsersCount, userConfidence);
            Logger.TraceWriteLine(string.Format("Generated {0} facts with {1} correct - ratio of DB {2}",
                m_initSection.NumOfTopicsToAnswer * m_initSection.UsersCount, correctAnswerCount,
                ((double)correctAnswerCount) / (m_initSection.NumOfTopicsToAnswer * m_initSection.UsersCount)));
        }

        private uint generateUsers(int firstUser, int usersCount, double userConfidence)
        {
            Random rnd = new Random();
            Random rnd1 = new Random();
            int lenBitArray = m_items_num_per_topic * m_topics_num;
            uint correctAnswerCount = 0;
            for (int i = firstUser; i < Math.Min(firstUser + usersCount, m_bitAr.Length); i++)
            {
                m_bitAr[i] = new MySpecialBitArray(lenBitArray);
                TestDataGenerator.User u = new TestDataGenerator.User(
                    userConfidence, m_topics_num, (int)m_initSection.NumOfTopicsToAnswer);
                int topicIndex=0;
                while (u.GetNextTopic(out topicIndex))
                {
                    int curBitIndex = topicIndex * m_items_num_per_topic; //index of first item of the topic - correct answer
                    if (rnd.NextDouble() > u.Confidence) // if decided to answer incorrect
                    {
                        curBitIndex += rnd1.Next(1, m_items_num_per_topic - 1);
                    }
                    else
                    {
                        correctAnswerCount++;
                    }
                    m_bitAr[i].Set((uint)curBitIndex);
                }
            }
            return correctAnswerCount;
        }

        public void CreateBasisTablesFiles()
        {
            UsersFileCreation();
            TopicsFileCreation();
            ItemsFileCreation();
            CorrectFactsFileCreation();
            ItemsMentionsCreation();
            UserGraphFileCreation();
            Logger.TraceWriteLine("creation of all files finished at " + DateTime.Now.ToLongTimeString());

        }

        private void UserGraphFileCreation()
        {
            Logger.TraceWriteLine("creation of usergraph " + DateTime.Now.ToLongTimeString());
			string filename = Path.Combine(m_workingDirectory, "UserGraph.dat");
			using (StreamWriter fs_usergraph = new StreamWriter(filename, false, Encoding.ASCII))
            {
                int cur_id = 1;
                DateTime time = DateTime.Now;
                for (int i = m_bitAr.Length - 1; i >= 0; i--)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        int count = (int)m_bitAr[i].AndCounting(m_bitAr[j]);
                        if (count > 0)
                        {
                            m_totalUserWeight[i] += count;
                            m_totalUserWeight[j] += count;
                        }
                    }
                }
                DCF.Common.PerformanceCounterStatic.ReportTimer("Total User Weight Calc");
                Logger.TraceWriteLine(string.Format(
                    "creation of usergraph finished calculation {0} - {1}", DateTime.Now.ToLongTimeString(),
                    DCF.Common.PerformanceCounterStatic.ReportTimer("Total User Weight Calc")));
                for (int i = m_bitAr.Length - 1; i >= 0; i--)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        int count = (int)m_bitAr[i].AndCounting(m_bitAr[j]);
                        if (count > 0)
                        {
                            writeLineUserGraph(fs_usergraph, cur_id++, i, j, count);
                            writeLineUserGraph(fs_usergraph, cur_id++, j, i, count);
                        }
                    }
                    if (i % 1000 == 0)
                    {
                        Logger.TraceWriteLine(string.Format("Cretion of 1000 users finished after {0} seconds (current user {1})",
                            (DateTime.Now - time).TotalSeconds, i));
                        time = DateTime.Now;
                    }
                } // for i
            } // using
        }

        private void writeLineUserGraph(StreamWriter fs_usergraph, int cur_id, int i, int j, int count)
        {
            fs_usergraph.Write(cur_id); // mention id
            fs_usergraph.Write(',');
            fs_usergraph.Write(i + 1); // source user id
            fs_usergraph.Write(',');
            fs_usergraph.Write(j + 1); // target user id
            fs_usergraph.Write(',');
            fs_usergraph.WriteLine(((double)count) / m_totalUserWeight[i]); // notmalized weight
        }


        private void ItemsMentionsCreation()
        {
            Logger.TraceWrite("creation of mentions " + DateTime.Now.ToLongTimeString());
			string filename = Path.Combine(m_workingDirectory, "ItemsMentions.dat");
			using (StreamWriter fs_correctfacts = new StreamWriter(filename, false, Encoding.ASCII))
            {
                int cur_id = 1;
                for (int i = 0; i < m_bitAr.Length; i++)
                {
                    for (uint itemid = m_bitAr[i].NextSetIndex(); itemid < m_bitAr[i].Size;
                        itemid = m_bitAr[i].NextSetIndex(itemid))
                    {
                        fs_correctfacts.Write(cur_id++); // mention id
                        fs_correctfacts.Write(',');
                        fs_correctfacts.Write(i + 1); // user id
                        fs_correctfacts.Write(',');
                        fs_correctfacts.Write(itemid + 1); // itemid
                        fs_correctfacts.Write(',');
                        fs_correctfacts.Write(itemid / m_items_num_per_topic + 1); // topic id
                        fs_correctfacts.WriteLine(",2011-06-10 16:08:32 "); // time
                    }
                }
                Logger.TraceWriteLine(string.Format("...{0} rows has been created", cur_id - 1));
            }
        }

        private void CorrectFactsFileCreation()
        {
            Logger.TraceWriteLine("creation of correct " + DateTime.Now.ToLongTimeString());
			string filename = Path.Combine(m_workingDirectory, "CorrectFacts.dat");
            using (StreamWriter fs_correctfacts = new StreamWriter(filename, false, Encoding.ASCII))
            {

                for (int i = 0; i < m_topics_num; i++)
                {
                    fs_correctfacts.Write(i * m_items_num_per_topic + 1); // correct item id
                    fs_correctfacts.Write(',');
                    fs_correctfacts.Write('t');
                    fs_correctfacts.Write(i + 1);
                    fs_correctfacts.Write("i0,");
                    fs_correctfacts.WriteLine(i + 1); // topicid
                }
            }
        }

        private void ItemsFileCreation()
        {
            Logger.TraceWriteLine("creation of items " + DateTime.Now.ToLongTimeString());
			string filename = Path.Combine(m_workingDirectory, "Items.dat");
			using (StreamWriter fs_items = new StreamWriter(filename, false, Encoding.ASCII))
            {

                for (int i = 0; i < m_topics_num * m_items_num_per_topic; i++)
                {
                    fs_items.Write(i + 1); // item id
                    fs_items.Write(',');
                    fs_items.Write('t');
                    fs_items.Write(i / m_items_num_per_topic + 1);
                    fs_items.Write('i');
                    fs_items.Write((i % m_items_num_per_topic) + 1); // name
                    fs_items.Write(',');
                    fs_items.WriteLine(i / m_items_num_per_topic + 1); // topicid
                }
            }
        }

        private void TopicsFileCreation()
        {
            Logger.TraceWriteLine("creation of topics " + DateTime.Now.ToLongTimeString());
			string filename = Path.Combine(m_workingDirectory, "Topics.dat");

            using (StreamWriter fs_topics = new StreamWriter(filename, false, Encoding.ASCII))
            {

                for (int i = 0; i < m_topics_num; i++)
                {
                    fs_topics.Write(i + 1); // topic id
                    fs_topics.Write(',');
                    fs_topics.Write('t');
                    fs_topics.Write(i + 1);  // name
                    fs_topics.WriteLine(",0"); // type
                }
            }
        }

        private void UsersFileCreation()
        {
            Logger.TraceWriteLine("creation of users " + DateTime.Now.ToLongTimeString());
            // users
			string filename = Path.Combine(m_workingDirectory, "Users.dat");
			using (StreamWriter fs_users = new StreamWriter(filename, false, Encoding.ASCII))
            {

                for (int i = 0; i < m_initSection.UsersCount; i++)
                {
                    fs_users.Write(i + 1); // user id
                    fs_users.Write(',');
                    fs_users.Write("user_");
                    fs_users.WriteLine(i + 1); // user name
                }
            }
        }


        delegate uint GenerateUsers(int firstUser, int usersCount, double userConfidence);
        delegate void FileCreation();
        public void GenerateUsersConcurrently()
        {
            System.Threading.ThreadPool.SetMaxThreads(10, 1000);
            List<Triple<int, int, int>> list = m_initSection.GetTopicsSpecialVariabilityProfiles();
            Trace.Assert(list.Count == 1);
            Trace.Assert(list[0].Second == list[0].Third);
            m_items_num_per_topic = list[0].Second;
            m_topics_num = list[0].First;
            List<Pair<double, double>> userProfiles = m_initSection.GetUserProfiles();
            Trace.Assert(userProfiles.Count == 1);
            double userConfidence = userProfiles[0].Second;
            const int UserChunk = 1000;
            m_numOfThreadsToFinish = (int)m_initSection.UsersCount/UserChunk + 
                ((m_initSection.UsersCount % UserChunk == 0) ? 0 : 1);
            Logger.TraceWriteLine(string.Format("Starting {0} request for users simulation", m_numOfThreadsToFinish));
            for (int i = 0; i < m_initSection.UsersCount; i += UserChunk)
            {
                GenerateUsers gu = new GenerateUsers(generateUsers);
                gu.BeginInvoke(i, UserChunk, userConfidence, FinishUserGeneration, gu);
            }
            FileCreation ufc = new FileCreation(UsersFileCreation);
            FileCreation tfc = new FileCreation(TopicsFileCreation);
            FileCreation ifc = new FileCreation(ItemsFileCreation);
            FileCreation cffc = new FileCreation(CorrectFactsFileCreation);

            Logger.TraceWriteLine(string.Format("Starting dump of {0}", "correct facts"));
            IAsyncResult cffcar = cffc.BeginInvoke(null, null);
            Logger.TraceWriteLine(string.Format("Starting dump of {0}", "users"));
            IAsyncResult ufcar = ufc.BeginInvoke(null, null);
            Logger.TraceWriteLine(string.Format("Starting dump of {0}", "topics"));
            IAsyncResult tfcar = tfc.BeginInvoke(null, null);
            Logger.TraceWriteLine(string.Format("Starting dump of {0}", "items"));
            IAsyncResult ifcar = ifc.BeginInvoke(null, null);

            Logger.TraceWrite("Wainting for threads to finish ");
            while (m_numOfThreadsToFinish > 0)
            {
                Logger.TraceWrite(".");
                System.Threading.Thread.Sleep(100);
            }
            Logger.TraceWriteLine(" Done!");
            Logger.TraceWriteLine(string.Format("Generated {0} facts with {1} correct - ratio of DB {2}",
                m_initSection.NumOfTopicsToAnswer * m_initSection.UsersCount, m_totalCorrectAnswers,
                ((double)m_totalCorrectAnswers) / (m_initSection.NumOfTopicsToAnswer * m_initSection.UsersCount)));

            FileCreation imfc = new FileCreation(ItemsMentionsCreation);
            FileCreation ugfc = new FileCreation(UserGraphFileCreation);

            IAsyncResult imfcar = imfc.BeginInvoke(null,null);
            IAsyncResult ugfcar = ugfc.BeginInvoke(null,null);

            Logger.TraceWriteLine("Waiting all threads to finish");

            ufc.EndInvoke(ufcar);
            Logger.TraceWriteLine("Users creation finished");
            tfc.EndInvoke(tfcar);
            Logger.TraceWriteLine("Topics creation finished");
            ifc.EndInvoke(ifcar);
            Logger.TraceWriteLine("Items creation finished");
            cffc.EndInvoke(cffcar);
            Logger.TraceWriteLine("Correct Facts creation finished");

            imfc.EndInvoke(imfcar);
            Logger.TraceWriteLine("ItemsMentions creation finished");
            ugfc.EndInvoke(ugfcar);
            Logger.TraceWriteLine("UserGraph creation finished");

        }

        void FinishUserGeneration(IAsyncResult ar)
        {
            GenerateUsers gu = (GenerateUsers) ar.AsyncState;
            uint correct = gu.EndInvoke(ar);
            System.Threading.Interlocked.Add(ref m_totalCorrectAnswers, (int)correct);
            System.Threading.Interlocked.Add(ref m_numOfThreadsToFinish, -1);
        }

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
        private InitSection m_initSection = null;
        MySpecialBitArray[] m_bitAr = null; //new MySpecialBitArray[(int)m_initSection.UsersCount];
        int[] m_totalUserWeight = null; //new uint[(int)m_initSection.UsersCount];
        int m_topics_num;
        int m_items_num_per_topic;
        int m_totalCorrectAnswers = 0;
        int m_numOfThreadsToFinish = 0;
		string m_workingDirectory = string.Empty;
        void FinishClusteredUserCreation(IAsyncResult ar)
        {
            CreateClusteredUserActivityDelegate gu = (CreateClusteredUserActivityDelegate)ar.AsyncState;
            uint correct = gu.EndInvoke(ar);
            System.Threading.Interlocked.Add(ref m_totalCorrectAnswers, (int)correct);
            System.Threading.Interlocked.Add(ref m_numOfThreadsToFinish, -1);
        }
        internal void GenerateClusteredUsersConcurrently()
        {
            System.Threading.ThreadPool.SetMaxThreads(10, 1000);
            List<Triple<int, int, int>> list = m_initSection.GetTopicsSpecialVariabilityProfiles();
            Trace.Assert(list.Count == 1);
            Trace.Assert(list[0].Second == list[0].Third);
            m_items_num_per_topic = list[0].Second;
            m_topics_num = list[0].First;
            List<Pair<double, double>> userProfiles = m_initSection.GetUserProfiles();
            Trace.Assert(userProfiles.Count == 1);
            double userConfidence = userProfiles[0].Second;
            const uint UserChunk = 1000;

            uint usersCount = (uint)m_initSection.UsersCount;
            uint numTopicsToAnswer = (uint)m_initSection.NumOfTopicsToAnswer;
            uint clusterCount = usersCount / UserChunk ;

            // check if number of topics is sufficient
            if (m_topics_num / clusterCount < 10)
            {
                Logger.TraceWriteLine(string.Format("Number of topics ({0}) is not sufficient to devide into {1} clusters",
                    m_topics_num, clusterCount));
                Environment.Exit(1);
            }
            ///////////////////////////////////////////////////////////////////////////////////
            // create users
            for (uint curCluster = 0; curCluster < clusterCount; curCluster++)
            {
                CreateClusteredUserActivityDelegate curDel = new CreateClusteredUserActivityDelegate(CreateClusteredUserActivity);
                curDel.BeginInvoke(curCluster, UserChunk, clusterCount, usersCount, (uint)m_topics_num, numTopicsToAnswer, 
                    (uint)m_items_num_per_topic, userConfidence, m_bitAr, 
                    FinishClusteredUserCreation, curDel);
                System.Threading.Interlocked.Add(ref m_numOfThreadsToFinish, 1);
            }

            FileCreation ufc = new FileCreation(UsersFileCreation);
            FileCreation tfc = new FileCreation(TopicsFileCreation);
            FileCreation ifc = new FileCreation(ItemsFileCreation);
            FileCreation cffc = new FileCreation(CorrectFactsFileCreation);

            Logger.TraceWriteLine(string.Format("Starting dump of {0}", "correct facts"));
            IAsyncResult cffcar = cffc.BeginInvoke(null, null);
            Logger.TraceWriteLine(string.Format("Starting dump of {0}", "users"));
            IAsyncResult ufcar = ufc.BeginInvoke(null, null);
            Logger.TraceWriteLine(string.Format("Starting dump of {0}", "topics"));
            IAsyncResult tfcar = tfc.BeginInvoke(null, null);
            Logger.TraceWriteLine(string.Format("Starting dump of {0}", "items"));
            IAsyncResult ifcar = ifc.BeginInvoke(null, null);

            Logger.TraceWrite("Wainting for threads to finish ");
            while (m_numOfThreadsToFinish > 0)
            {
                Logger.TraceWrite(".");
                System.Threading.Thread.Sleep(100);
            }
            Logger.TraceWriteLine(" Done!");

            Logger.TraceWriteLine(string.Format("Generated {0} facts with {1} correct - ratio of DB {2}",
                m_initSection.NumOfTopicsToAnswer * m_initSection.UsersCount, m_totalCorrectAnswers,
                ((double)m_totalCorrectAnswers) / (m_initSection.NumOfTopicsToAnswer * m_initSection.UsersCount)));

            FileCreation imfc = new FileCreation(ItemsMentionsCreation);
            FileCreation ugfc = new FileCreation(UserGraphFileCreation);

            IAsyncResult imfcar = imfc.BeginInvoke(null, null);
            IAsyncResult ugfcar = ugfc.BeginInvoke(null, null);

            Logger.TraceWriteLine("Waiting all threads to finish");

            ufc.EndInvoke(ufcar);
            Logger.TraceWriteLine("Users creation finished");
            tfc.EndInvoke(tfcar);
            Logger.TraceWriteLine("Topics creation finished");
            ifc.EndInvoke(ifcar);
            Logger.TraceWriteLine("Items creation finished");
            cffc.EndInvoke(cffcar);
            Logger.TraceWriteLine("Correct Facts creation finished");

            imfc.EndInvoke(imfcar);
            Logger.TraceWriteLine("ItemsMentions creation finished");
            ugfc.EndInvoke(ugfcar);
            Logger.TraceWriteLine("UserGraph creation finished");

        }

        private delegate uint CreateClusteredUserActivityDelegate(uint curCluster, uint UserChunk, uint clusterCount, uint usersCount,
            uint topics_num, uint numTopicsToAnswer, uint items_num_per_topic, double userConfidence,
            MySpecialBitArray[] userSimulation);

        private class RandomComparer : IComparer<uint>
        {
            Random rnd = new Random();
            #region IComparer<uint> Members

            public int Compare(uint x, uint y)
            {
                if (rnd.NextDouble() <= 0.5)
                {
                    return 1;
                }
                return -1;
            }

            #endregion
        }

        private uint CreateClusteredUserActivity(uint curCluster, uint UserChunk, uint clusterCount, uint usersCount, 
            uint topics_num, uint numTopicsToAnswer, uint items_num_per_topic, double userConfidence, 
            MySpecialBitArray[] userSimulation)
        {
            uint correctAns = 0;
            uint minTopic = curCluster * topics_num / clusterCount;
            uint maxTopic = (curCluster + 1) * topics_num / clusterCount;

            uint minUser = curCluster * usersCount / UserChunk;
            uint maxUser = minUser + UserChunk;
            Random rnd = new Random();
            RandomComparer rndCmp = new RandomComparer();
            Random anotherRnd = new Random();
            for (uint userInd = minUser; userInd < maxUser; userInd++)
            {
                userSimulation[userInd] = new MySpecialBitArray(topics_num * items_num_per_topic);
                uint[] topicsToAnswer = new uint[maxTopic - minTopic];
                for (uint i = 0; i < maxTopic - minTopic; i++)
                {
                    topicsToAnswer[i] = i + minTopic;
                }
                Array.Sort<uint>(topicsToAnswer);
                for (uint i = 0; i < numTopicsToAnswer; i++)
                {
                    if (rnd.NextDouble() <= userConfidence)
                    {
                        userSimulation[userInd].Set(topicsToAnswer[i] * items_num_per_topic);
                        correctAns++;
                    }
                    else
                    {
                        uint num = (uint)anotherRnd.Next(1, (int)items_num_per_topic);
                        userSimulation[userInd].Set(topicsToAnswer[i] * items_num_per_topic + num);
                    }
                }
            }
            return correctAns;
        }
    }
}
