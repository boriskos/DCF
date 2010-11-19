using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace DCF.DemoRules.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlSerializer serializerTopics = new XmlSerializer(typeof(Topics));
            TextWriter textWriterTopics = new StreamWriter(@"C:\temp\Topics.xml");
            Topics topics = new Topics
            {
                TopicList = new List<TopicTemplate> { 
                    new TopicTemplate { Name = "CountryOf{0}", Category="CountryOf", Text = "What is a country of {0}?", 
                        Values = new List<string> {"Moscow", "Tel Aviv", "Jerusalem", "Tokyo", "New York", "London"}},
                    new TopicTemplate { Name = "CapitalOf{0}", Category="CapitalOf", Text = "What is the capital of {0}?", 
                        Values = new List<string> {"Russia", "Israel", "USA", "Japan", "France", "England"}}
                }
            };
            serializerTopics.Serialize(textWriterTopics, topics);
            textWriterTopics.Close();

            XmlSerializer serializerItems = new XmlSerializer(typeof(Items));
            TextWriter textWriterItems = new StreamWriter(@"C:\temp\Items.xml");
            Items items = new Items
            {
                ItemList = new List<ItemTemplate> 
                {
                    new ItemTemplate { TopicName="CountryOfMoscow", CorrectValues = new List<string> {"Russia", "USA", "France", "Tokyo"}},
                    new ItemTemplate { TopicName="CountryOfTel Aviv", CorrectValues = new List<string> {"Israel", "USA", "Jordan"}},
                    new ItemTemplate { TopicName="CountryOfJerusalem", CorrectValues = new List<string> {"Russia", "USA", "Israel"}},
                    new ItemTemplate { TopicName="CountryOfTokyo", CorrectValues = new List<string> {"Japan", "China", "South Korea", "Philipines"}},
                    new ItemTemplate { TopicName="CountryOfNew York", CorrectValues = new List<string> {"USA", "England"}},
                    new ItemTemplate { TopicName="CountryOfLondon", CorrectValues = new List<string> {"USA", "England", "Germany"}}
                }
            };
            serializerItems.Serialize(textWriterItems, items);
            textWriterItems.Close();
        }
    }
}
