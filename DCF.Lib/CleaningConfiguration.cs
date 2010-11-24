using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wintellect.PowerCollections;
using DCF.Common;
using System.Configuration;

namespace DCF.Lib
{
    /// <summary>
    /// Keeps the configuraiton of the engine
    /// </summary>
    public static class CleaningConfiguration
    {
        /// <summary>
        /// Only initiate DB on true, otherwise run rules
        /// </summary>
        public static bool DbInitOnly;
        /// <summary>
        /// If to avoid DB initiallization, otherwise initiallize
        /// </summary>
        public static bool NoDbInit;
        /// <summary>
        /// Max number of iterations 
        /// </summary>
        public static int MaxSampleIterations;
        public static int MaxCleaningIterationsPerSample=1;
        public static double ConversionDelta;
        public static int ConversionSamplesCount;
        public static double ConversionAlfa;
        public static double ConversionTolerance;

        public static int ExperimentType;

        public static long NumberOfFacts;
        public static double CorrectFactsRatio;
        public static int NumberOfIncorrectFactsInUse;
        public static int NumberOfCountriesWithRestrictedIncorrectFactsCount;
        public static bool GenerateMayors;

        public static List<Pair<double, double>> UsersProfilesPortionBelief = new List<Pair<double,double>>();

        public static void setUserProfiles(string usersProfilesStr)
        {
            UsersProfilesPortionBelief.Clear();
            char[] separators = { '(', ')' };
            string[] usersStr = usersProfilesStr.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string pair in usersStr)
            {
                string[] profileEntitiesStr = pair.Split(',');
                UsersProfilesPortionBelief.Add(new Pair<double, double>(
                    double.Parse(profileEntitiesStr[0]),
                    double.Parse(profileEntitiesStr[1])));
            }
        }

        public static void TraceCurrentConfiguration()
        {
            Logger.TraceWriteLine(string.Format("DbInitOnly={0}", DbInitOnly));
            Logger.TraceWriteLine(string.Format("NoDbInit={0}", NoDbInit));
            Logger.TraceWriteLine(string.Format("MaxSampleIterations={0}", MaxSampleIterations));
            Logger.TraceWriteLine(string.Format("ConversionDelta={0}", ConversionDelta));
            Logger.TraceWriteLine(string.Format("ConversionSamplesCount={0}", ConversionSamplesCount));
            Logger.TraceWriteLine(string.Format("ConversionAlfa={0}", ConversionAlfa));
            Logger.TraceWriteLine(string.Format("ConversionTolerance={0}", ConversionTolerance));

            Logger.TraceWriteLine(string.Format("ExperimentType={0}", ExperimentType));
            Logger.TraceWriteLine(string.Format("NumberOfFacts={0}", NumberOfFacts));
            Logger.TraceWriteLine(string.Format("CorrectFactsRatio={0}", CorrectFactsRatio));
            Logger.TraceWriteLine(string.Format("NumberOfIncorrectFactsInUse={0}", NumberOfIncorrectFactsInUse));
            Logger.TraceWriteLine(string.Format("NumberOfCountriesWithRestrictedIncorrectFactsCount={0}", 
                NumberOfCountriesWithRestrictedIncorrectFactsCount));
            Logger.TraceWriteLine(string.Format("GenerateMayors={0}", GenerateMayors));

            if (UsersProfilesPortionBelief.Count > 0)
            {
                Logger.TraceWriteLine("Current user profiles:");
                Logger.TraceIndent();
                foreach (var line in UsersProfilesPortionBelief)
                {
                    Logger.TraceWriteLine(string.Format("{0} percent have probability of {1}", line.First, line.Second));
                }
                Logger.TraceUnindent();
            }
        }
        public static void PopulateFromAppConfig()
        {
            // apply settings
            foreach (string sName in SettingNames)
            {
                string setting = ConfigurationSettings.AppSettings[sName];
                if (string.IsNullOrEmpty(setting)) continue;
                switch (sName)
                {
                    case NoInitSettingName:
                        CleaningConfiguration.NoDbInit = bool.Parse(setting);
                        break;
                    case DbInitOnlySettingName:
                        CleaningConfiguration.DbInitOnly = bool.Parse(setting);
                        break;
                    case MaxSampleIterationsName:
                        CleaningConfiguration.MaxSampleIterations = int.Parse(setting);
                        break;
                    case ConversionDeltaName:
                        CleaningConfiguration.ConversionDelta = double.Parse(setting);
                        break;
                    case ConversionAlfaName:
                        CleaningConfiguration.ConversionAlfa = double.Parse(setting);
                        break;
                    case ConversionToleranceName:
                        CleaningConfiguration.ConversionTolerance = double.Parse(setting);
                        break;
                    case ConversionSamplesCountName:
                        CleaningConfiguration.ConversionSamplesCount = int.Parse(setting);
                        break;
                    case ExperimentName:
                        CleaningConfiguration.ExperimentType = int.Parse(setting);
                        break;
                    default:
                        Logger.TraceWriteLine(string.Format("Error: Unknown parameter: {0}", sName));
                        break;
                }
            }
        }

        private const string NoInitSettingName = "NoDbInit";
        private const string DbInitOnlySettingName = "DbInitOnly";
        private const string ConversionSamplesCountName = "ConversionSamplesCount";
        private const string ConversionDeltaName = "ConversionDelta";
        private const string ConversionAlfaName = "ConversionAlfa";
        private const string ConversionToleranceName = "ConversionTolerance";
        private const string MaxSampleIterationsName = "MaxSampleIterations";
        private const string InitSectionName = "InitSection";
        private const string ExperimentName = "Experiment";
        private const string NumberOfIncorrectFactsInUseName = "NumberOfIncorrectFactsInUse";
        private const string NumberOfCountriesWithRestrictedIncorrectFactsCountName =
            "NumberOfCountriesWithRestrictedIncorrectFactsCount";
        private const string GenerateMayorsName = "GenerateMayors";

        private static string[] SettingNames = {
                                                   NoInitSettingName, 
                                                   DbInitOnlySettingName, 
                                                   ConversionSamplesCountName, 
                                                   ConversionDeltaName, 
                                                   ConversionAlfaName,
                                                   ConversionToleranceName,
                                                   MaxSampleIterationsName,
                                                   ExperimentName,
                                                   NumberOfIncorrectFactsInUseName,
                                                   NumberOfCountriesWithRestrictedIncorrectFactsCountName,
                                                   GenerateMayorsName
                                               };

    }
}
