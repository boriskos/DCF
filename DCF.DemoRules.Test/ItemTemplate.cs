using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCF.DemoRules.Test
{
    public class ItemTemplate
    {
        public string TopicName { get; set; }
        public List<string> CorrectValues { get; set; }
        public List<string> IncorrectValues { get; set; }
    }
    
    public class Items
    {
        public List<ItemTemplate> ItemList { get; set; }
    }
}
