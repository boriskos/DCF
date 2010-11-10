using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.Lib;
using DCF.DataLayer;


namespace DCF.DataLayerAwareLib
{
    public class MySqlRule : Rule
    {
        protected MySqlRule(MySqlUtils sqlUtils, IRuleSupplier ruleSupplier)
            : base(ruleSupplier)
        {
            SqlUtils = sqlUtils;
        }

        protected MySqlUtils SqlUtils { get; set; }
    }
}
