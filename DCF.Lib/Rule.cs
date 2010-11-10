using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCF.Lib
{
    public class Rule
    {
        #region Public Types
        public enum RuleTypeEnum { System, Cleaning };
        public enum PresedenceEnum { First, Last, None };
        public delegate void RuleFinishedDelegate(bool success, object data);
        public delegate void RuleExecuterDelegate(Dictionary<string, object> dataHashTable);
        
        
        #endregion

        #region Public Properties
        public RuleTypeEnum RuleType { get; protected set; }
        public bool Persistent { get; protected set; }
        public string Id { get; protected set; }
        public double Probability { get; protected set; }
        public List<string> InvolvedTables { get; protected set; }
        public List<string> AffectedTables { get; protected set; }
        public event EventHandler StopCleaningProcess;
        public event RuleFinishedDelegate DataIsClean;
        public List<Rule> PrerequisiteRules { get; protected set; }
        public PresedenceEnum Precedence { get; protected set; }
        public int PrecedencePriority { get; protected set; }
        public IRuleSupplier RuleSupplier { get; protected set; }

        #endregion
        #region Public Constructors
        public Rule(IRuleSupplier ruleSupplier)
        {
            RuleType = RuleTypeEnum.System;
            Persistent = true;
            Id = Guid.NewGuid().ToString();
            Probability = 1;
            InvolvedTables = null;
            AffectedTables = null;
            PrerequisiteRules = null;
            Precedence = PresedenceEnum.None;
            PrecedencePriority = 0;
            RuleSupplier = ruleSupplier;
        }
        public Rule(RuleExecuterDelegate executer, RuleExecuterDelegate initializer, IRuleSupplier ruleSupplier)
            : this(ruleSupplier)
        {
            if (executer == null) throw new ArgumentNullException("executer");
            m_ruleExecuter += executer;
            if (initializer != null) m_ruleInitializer += initializer;
        }

        #endregion
        
        public virtual void execute(Dictionary<string, object> dataHashTable)
        {
            if (m_ruleExecuter!=null) m_ruleExecuter(dataHashTable);
        }

        public virtual void init(Dictionary<string, object> dataHashTable)
        {
            if (m_ruleInitializer!=null) m_ruleInitializer(dataHashTable);
        }

        protected virtual void OnRuleExecutionFinished(bool success, object data)
        {
            DataIsClean(success, data);
        }
        protected virtual void OnStopCleaningProcess()
        {
            StopCleaningProcess(this, null);
        }
        
        #region Protected members

        protected event RuleExecuterDelegate m_ruleExecuter;
        protected event RuleExecuterDelegate m_ruleInitializer;

	    #endregion    
        public const string RulesLogger = "Rules";

    }

}
