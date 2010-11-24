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
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Item Template {");
            sb.AppendFormat(" Topic Name: {{ {0} }}", TopicName);
            sb.Append(" Correct Values: {");
            foreach (string val in CorrectValues)
            {
                sb.AppendFormat(" \"{0}\"", val);
            }
            sb.Append(" }");
            sb.Append(" Incorrect Values: {");
            foreach (string val in IncorrectValues)
            {
                sb.AppendFormat(" \"{0}\"", val);
            }
            sb.Append(" } } ");
            return sb.ToString();
        }
    }
    
    public class Items
    {
        public List<ItemTemplate> ItemList { get; set; }
    }
}
