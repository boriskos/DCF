using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Globalization;
using Wintellect.PowerCollections;
using DCF.Common;

namespace DCF.DemoRules.Test
{


    public class InitSection: ConfigurationSection
    {
        public InitSection()
        {
        }

        public override bool IsReadOnly()
        {
            return false;
        }
        [ConfigurationProperty("NumberOfFacts",
            DefaultValue=(long)1000,
            IsRequired=true)]
        [LongValidator(MinValue=10, MaxValue=1000000000, ExcludeRange=false)]
        public long NumberOfFacts
        {
            get
            {
                return (long)this["NumberOfFacts"];
            }
            set
            {
                this["NumberOfFacts"] = value;
            }

        }

        [ConfigurationProperty("TopicsVariabilityProfile",
            IsRequired = false)]
        public string TopicsVariabilityProfile
        {
            get
            {
                return (string)this["TopicsVariabilityProfile"];
            }
            set
            {
                this["TopicsVariabilityProfile"] = value;
            }

        }

        [ConfigurationProperty("GenerateBasisTables",
            DefaultValue = false,
            IsRequired = false)]
        public bool GenerateBasisTables
        {
            get
            {
                return (bool)this["GenerateBasisTables"];
            }
            set
            {
                this["GenerateBasisTables"] = value;
            }

        }

        [ConfigurationProperty("UserProfiles")]
        public string UserProfiles
        {
            get
            {
                return (string)this["UserProfiles"];
            }
            set
            {
                this["UserProfiles"] = value;
            }
        }

        [ConfigurationProperty("ItemsDefinitionFile", 
            DefaultValue=null,
            IsRequired=false)]
        public string ItemsDefinitionFile
        {
            get
            {
                return (string)this["ItemsDefinitionFile"];
            }
            set
            {
                this["ItemsDefinitionFile"] = value;
            }
        }

        [ConfigurationProperty("TopicsDefinitionFile",
            DefaultValue = null,
            IsRequired = false)]
        public string TopicsDefinitionFile
        {
            get
            {
                return (string)this["TopicsDefinitionFile"];
            }
            set
            {
                this["TopicsDefinitionFile"] = value;
            }
        }

        /// <summary>
        /// Uses <see cref="Logger"/> class to trace the current state
        /// </summary>
        public void TraceContents()
        {
            Logger.TraceWriteLine(string.Format("NumberOfFacts: {0}", NumberOfFacts));
            Logger.TraceWriteLine(string.Format("GenerateBasisTables: {0}", GenerateBasisTables));
            if (TopicsDefinitionFile != null)
                Logger.TraceWriteLine(string.Format("TopicsDefinitionFile: {0}", TopicsDefinitionFile));
            if (ItemsDefinitionFile != null)
                Logger.TraceWriteLine(string.Format("ItemsDefinitionFile: {0}", ItemsDefinitionFile));
            // report UserProfiles
            List<Pair<double, double>> usersProfilesPortionBelief = GetUserProfiles();
            if (usersProfilesPortionBelief.Count > 0)
            {
                Logger.TraceWriteLine("Current user profiles:");
                Logger.TraceIndent();
                double dbOveralRate = 0.0;
                foreach (var line in usersProfilesPortionBelief)
                {
                    dbOveralRate += line.First * line.Second;
                    Logger.TraceWriteLine(string.Format("{0} percent have probability of {1}", line.First*100, line.Second));
                }
                Logger.TraceWriteLine(string.Format("DB overal correct facts rate is {0}", dbOveralRate));
                Logger.TraceUnindent();
            }
            // report TopicProfiles
            List<Pair<int, int>> topicsProfiles = GetTopicsVariabilityProfiles();
            if (topicsProfiles.Count > 0)
            {
                Logger.TraceWriteLine("Current topic variablility profiles:");
                Logger.TraceIndent();
                foreach (var line in topicsProfiles)
                {
                    Logger.TraceWriteLine(string.Format("{0} topics have variability of {1} possible incorrect answers", line.First, line.Second));
                }
                Logger.TraceUnindent();
            }
        }

        /// <summary>
        /// returns the user profiles as list of pairs : user percent, probability of the user to provde with correct answer
        /// </summary>
        /// <returns></returns>
        public List<Pair<double, double>> GetUserProfiles()
        {
            List<Pair<double, double>> profilesList = new List<Pair<double, double>>();
            string profiles = UserProfiles;
            GetProfiles<double>(profilesList, profiles, a => double.Parse(a));
            return profilesList;
        }
        public List<Pair<int, int>> GetTopicsVariabilityProfiles()
        {
            List<Pair<int, int>> profilesList = new List<Pair<int, int>>();
            string profiles = TopicsVariabilityProfile;
            GetProfiles<int>(profilesList, profiles, a => int.Parse(a));
            return profilesList;
        }

        private static void GetProfiles<T>(List<Pair<T, T>> profilesList, string profiles, Func<string, T> parser) 
        {
            if (profiles != null)
            {
                char[] separators = { '(', ')' };
                string[] profileStr = profiles.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (string pair in profileStr)
                {
                    string[] profileEntitiesStr = pair.Split(',');
                    profilesList.Add(new Pair<T, T>(
                        parser(profileEntitiesStr[0].Trim()),
                        parser(profileEntitiesStr[1].Trim())));
                }
            }
        }

    }

    public class DoubleValidator : ConfigurationValidatorBase
    {
        // Fields
        private ValidationFlags _flags;
        private double _maxValue;
        private double _minValue;
        private double _resolution;

        // Methods
        public DoubleValidator(double minValue, double maxValue)
            : this(minValue, maxValue, false, double.Epsilon)
        {
        }

        public DoubleValidator(double minValue, double maxValue, bool rangeIsExclusive)
            : this(minValue, maxValue, rangeIsExclusive, double.Epsilon)
        {
        }

        public DoubleValidator(double minValue, double maxValue,
            bool rangeIsExclusive, double resolution)
        {
            this._minValue = double.MinValue;
            this._maxValue = double.MaxValue;
            this._resolution = double.Epsilon;
            if (resolution <= 0)
            {
                throw new ArgumentOutOfRangeException("resolution");
            }
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("minValue", "Validator_min_greater_than_max");
            }
            this._minValue = minValue;
            this._maxValue = maxValue;
            this._resolution = resolution;
            this._flags = rangeIsExclusive ? ValidationFlags.ExclusiveRange : ValidationFlags.None;
        }

        public override bool CanValidate(Type type)
        {
            return (type == typeof(double));
        }

        public override void Validate(object value)
        {
            ValidatorUtils.HelperParamValidation(value, typeof(double));
            ValidatorUtils.ValidateScalar<double>((double)value, this._minValue, this._maxValue, this._resolution, this._flags == ValidationFlags.ExclusiveRange);
        }

        // Nested Types
        private enum ValidationFlags
        {
            None,
            ExclusiveRange
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DoubleValidatorAttribute : ConfigurationValidatorAttribute
    {
        // Fields
        private bool _excludeRange;
        private double _max = 0x7fffffff;
        private double _min = -2147483648;

        // Properties
        public bool ExcludeRange
        {
            get
            {
                return this._excludeRange;
            }
            set
            {
                this._excludeRange = value;
            }
        }

        public double MaxValue
        {
            get
            {
                return this._max;
            }
            set
            {
                if (this._min > value)
                {
                    throw new ArgumentOutOfRangeException("value", "Validator_min_greater_than_max");
                }
                this._max = value;
            }
        }

        public double MinValue
        {
            get
            {
                return this._min;
            }
            set
            {
                if (this._max < value)
                {
                    throw new ArgumentOutOfRangeException("value", "Validator_min_greater_than_max");
                }
                this._min = value;
            }
        }

        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                return new DoubleValidator(this._min, this._max, this._excludeRange);
            }
        }
    }


    internal static class ValidatorUtils
    {
        // Methods
        public static void HelperParamValidation(object value, Type allowedType)
        {
            if ((value != null) && (value.GetType() != allowedType))
            {
                throw new ArgumentException("Validator_value_type_invalid", string.Empty);
            }
        }

        private static void ValidateRangeImpl<T>(T value, T min, T max, bool exclusiveRange) where T : IComparable<T>
        {
            IComparable<T> comparable = value;
            bool flag = false;
            if (comparable.CompareTo(min) >= 0)
            {
                flag = true;
            }
            if (flag && (comparable.CompareTo(max) > 0))
            {
                flag = false;
            }
            if (!(flag ^ exclusiveRange))
            {
                string format = null;
                if (min.Equals(max))
                {
                    if (exclusiveRange)
                    {
                        format = "Validation_scalar_range_violation_not_different";
                    }
                    else
                    {
                        format = "Validation_scalar_range_violation_not_equal";
                    }
                }
                else if (exclusiveRange)
                {
                    format = "Validation_scalar_range_violation_not_outside_range";
                }
                else
                {
                    format = "Validation_scalar_range_violation_not_in_range";
                }
                throw new ArgumentException(string.Format(
                    CultureInfo.InvariantCulture, format, new object[] { min.ToString(), max.ToString() }));
            }
        }

        private static void ValidateResolution(string resolutionAsString, double value, double resolution)
        {
            if ((value % resolution) != 0.0)
            {
                throw new ArgumentException("Validator_scalar_resolution_violation");
            }
        }

        public static void ValidateScalar<T>(T value, T min, T max, T resolution, bool exclusiveRange) where T : IComparable<T>
        {
            ValidateRangeImpl<T>(value, min, max, exclusiveRange);
            ValidateResolution(resolution.ToString(), Convert.ToDouble(value, CultureInfo.InvariantCulture), 
                Convert.ToDouble(resolution, CultureInfo.InvariantCulture));
        }

    }



}
