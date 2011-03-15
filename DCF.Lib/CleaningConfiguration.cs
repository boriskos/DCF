using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wintellect.PowerCollections;
using DCF.Common;
using System.Configuration;
using System.Reflection;

namespace DCF.Lib
{
    /// <summary>
    /// Keeps the configuraiton of the engine
    /// </summary>
    public class CleaningConfiguration
    {
        /// <summary>
        /// The values that <see cref="ExperimentType"/> can take
        /// </summary>
        public enum ExperimentTypeEnum
        {
            RepeirPrimaryKey = 0,
            TwoEstimates = 1,
            Cosine = 2,
            Majority = 3
        }
        /// <summary>
        /// Max number of iterations 
        /// </summary>
        public int MaxSampleIterations {get; private set;}
        public int MaxCleaningIterationsPerSample { get; private set; }
        public double ConversionDelta { get; private set; }
        public int ConversionSamplesCount { get; private set; }
        public double ConversionAlfa { get; private set; }
        public double ConversionTolerance { get; private set; }
        public string TopicCategory { get; private set; }

        public int ExperimentType { get; private set; }

        public void TraceCurrentConfiguration()
        {
            foreach (PropertyInfo pi in typeof(CleaningConfiguration).GetProperties())
            {
                if (pi.Name == "Item" || pi.Name == "Instance") continue;
                Logger.TraceWriteLine(string.Format("{0}={1}", pi.Name, pi.GetValue(this, null)));
            }
        }

        public void PopulateFromAppConfig()
        {
            // apply settings
            foreach (string sName in SettingNames)
            {
                string setting = ConfigurationManager.AppSettings[sName];
                this[sName] = setting;
            }
        }

        public object this[string settingName]
        {
            set
            {
                PropertyInfo pi = typeof(CleaningConfiguration).GetProperty(settingName);
                if (pi == null)
                    throw new ArgumentOutOfRangeException(string.Format("Error: Unknown parameter: {0}", settingName));
                if (pi.PropertyType == typeof(string))
                {
                    pi.SetValue(this, value.ToString(), null);
                }
                else
                {
                    pi.SetValue(this, pi.PropertyType.GetMethod("Parse", m_sTypeOfString).Invoke(null, new object[] { value.ToString() }), null);
                }
            }
            get
            {
                PropertyInfo pi = typeof(CleaningConfiguration).GetProperty(settingName);
                if (pi == null)
                    throw new ArgumentOutOfRangeException(string.Format("Error: Unknown parameter: {0}", settingName));
                return pi.GetValue(this, null);
            }
        }

        private const string ConversionSamplesCountName = "ConversionSamplesCount";
        private const string ConversionDeltaName = "ConversionDelta";
        private const string ConversionAlfaName = "ConversionAlfa";
        private const string ConversionToleranceName = "ConversionTolerance";
        private const string MaxSampleIterationsName = "MaxSampleIterations";
        private const string MaxCleaningIterationsPerSampleName = "MaxCleaningIterationsPerSample";
        private const string ExperimentName = "ExperimentType";
        private const string TopicCategoryName = "TopicCategory";

        public static string[] SettingNames = {
                                                   ConversionSamplesCountName, 
                                                   ConversionDeltaName, 
                                                   ConversionAlfaName,
                                                   ConversionToleranceName,
                                                   MaxSampleIterationsName,
                                                   ExperimentName,
                                                   MaxCleaningIterationsPerSampleName,
                                                   TopicCategoryName
                                               };
        private static Type[] m_sTypeOfString = new Type[] { typeof(string) };
        #region Singleton implementation
        /// <summary>
        /// Sataic c-tor ensures that the static variable m_sInstance is initiated before the first use of this class
        /// </summary>
        static CleaningConfiguration()
        {
            m_sInstance = new CleaningConfiguration();
        }

        /// <summary>
        /// Static single instance access property
        /// </summary>
        public static CleaningConfiguration Instance
        {
            get
            {
                return m_sInstance;
            }
        }

        /// <summary>
        /// private constructor
        /// </summary>
        private CleaningConfiguration() 
        {
            MaxCleaningIterationsPerSample = 1;
            ConversionDelta = 0.05;
            ConversionSamplesCount = 10;

        }
        /// <summary>
        /// single instance
        /// </summary>
        private static CleaningConfiguration m_sInstance = null;
        #endregion
    }
}
