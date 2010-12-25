using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.DataLayer;

namespace DCF.DemoRules.Test
{
    public enum TopicCategory { SingleAnswer, MultipleAnswers };

    public class TopicTemplate
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public TopicType Type { get; set; }
        public string Category { get; set; }
        public List<string> Values { get; set; }

        public override string ToString()
        {
            return string.Format("Topic {0} type {1} category {2} text \"{3}\" values: {{ {4} }}",
                Name, Type, Category, Text, String.Join( ",", Values.ToArray()));
        }

    }

    public class Topics
    {
        public List<TopicTemplate> TopicList { get; set; }
    }

}
