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
    }

    public class Topics
    {
        public List<TopicTemplate> TopicList { get; set; }
    }

}
