using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wintellect.PowerCollections;
using DCF.Common;

namespace DCF.Lib
{
    /// <summary>
    /// Probabilities extension to the <see cref="IEnumerable<T>"/> class
    /// </summary>
    public static class ProbabilityExtensions
    {
        /// <summary>
        /// Performs repair key on the <see cref="IEnumerable<T>"/> given in <paramref name="source"/> parameter
        /// </summary>
        /// <typeparam name="TSource">Type of the <see cref="IEnumerable"/> list</typeparam>
        /// <typeparam name="TKey">Type of key of the list</typeparam>
        /// <param name="source">list on which to perform the repair key operation</param>
        /// <param name="keySelector">Functor that returns the key, by which to perform repair-key operation</param>
        /// <param name="probSelector">Functor that returns the normalized probability for each item in the repair-key operation</param>
        /// <returns><see cref="IEnumerable<T>"/> of <typeparamref name="TSource"/> that have only one row per distinct key</returns>
        public static IEnumerable<TSource> RepairKey<TSource, TKey>
            (this IEnumerable<TSource> source,
             Func<TSource, TKey> keySelector,
             Func<TSource, double> probSelector
            )
        {
            return source.RepairKey(keySelector, probSelector,
                                     EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// Performs repair key on the <see cref="IEnumerable<T>"/> given in <paramref name="source"/> parameter
        /// </summary>
        /// <typeparam name="TSource">Type of the <see cref="IEnumerable"/> list</typeparam>
        /// <typeparam name="TKey">Type of key of the list</typeparam>
        /// <param name="source">list on which to perform the repair key operation</param>
        /// <param name="keySelector">Functor that returns the key, by which to perform repair-key operation</param>
        /// <param name="probSelector">Functor that returns the normalized probability for each item in the repair-key operation</param>
        /// <param name="comparer">Key value comparer</param>
        /// <returns><see cref="IEnumerable<T>"/> of <typeparamref name="TSource"/> that have only one row per distinct key</returns>
        public static IEnumerable<TSource> RepairKey<TSource, TKey>
            (this IEnumerable<TSource> source,
             Func<TSource, TKey> keySelector,
             Func<TSource, double> probSelector,
             IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            if (probSelector == null)
            {
                throw new ArgumentNullException("probSelector");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            return RepairKeyImpl(source, keySelector, probSelector, comparer);
        }

        public const double ProbabilityEpsilon = 0.00001;
        public static bool ProbabilityEquals(this double src, double target)
        {
            double t = Math.Abs(src - target);
            return t < ProbabilityEpsilon;
        }

        #region Backup

        ///// <summary>
        ///// Repair-key implementation
        ///// </summary>
        //private static IEnumerable<TSource> RepairKeyImpl<TSource, TKey>
        //    (IEnumerable<TSource> source,
        //     Func<TSource, TKey> keySelector,
        //     Func<TSource, double> probSelector,
        //     IEqualityComparer<TKey> comparer)
        //{
        //    using (new PerformanceCounter("RepairKey"))
        //    {
        //        Dictionary<TKey, double> selectedProbabilities = new Dictionary<TKey, double>(comparer);
        //        Dictionary<TKey, TSource> targetSet = new Dictionary<TKey, TSource>(comparer);
        //        Random rnd = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
        //        foreach (TSource element in source)
        //        {
        //            double probability;
        //            TKey curKey = keySelector(element);
        //            double curProbability = probSelector(element);
        //            if (selectedProbabilities.TryGetValue(curKey, out probability))
        //            {
        //                double rndNum = rnd.NextDouble();
        //                if ((rndNum * (probability + curProbability) > probability) ||
        //                    // in case of 0 probability
        //                    (Math.Abs(curProbability) < Double.Epsilon &&
        //                     Math.Abs(probability) < Double.Epsilon && rndNum > 0.5))
        //                {
        //                    targetSet[curKey] = element;
        //                    selectedProbabilities[curKey] = curProbability;
        //                }
        //            }
        //            else
        //            {
        //                targetSet.Add(curKey, element);
        //                selectedProbabilities.Add(curKey, curProbability);
        //            }
        //        }
        //        return targetSet.Values.AsEnumerable();
        //    }
        //}

        #endregion

        /// <summary>
        /// Repair-key implementation
        /// </summary>
        private static IEnumerable<TSource> RepairKeyImpl<TSource, TKey>
            (IEnumerable<TSource> source,
             Func<TSource, TKey> keySelector,
             Func<TSource, double> probSelector,
             IEqualityComparer<TKey> comparer)
        {
            using (new PerformanceCounter("RepairKey"))
            {

                // this buckets are the map of keys to ProbabilityKeyInfo class: 
                Dictionary<TKey, ProbabilityKeyInfo<TSource>> buckets =
                    new Dictionary<TKey, ProbabilityKeyInfo<TSource>>(comparer);

                // At first we classify the data
                foreach (TSource element in source)
                {
                    ProbabilityKeyInfo<TSource> keyInfo;
                    TKey curKey = keySelector(element);
                    double curProbability = probSelector(element);
                    System.Diagnostics.Debug.Assert(curProbability >= 0.0 && curProbability <= 1.0,
                        string.Format("RepairKey: Probability {0} is out of range", curProbability));
                    if (buckets.TryGetValue(curKey, out keyInfo))
                    {
                        keyInfo.AddElement(element, curProbability);
                    }
                    else
                    {
                        keyInfo = new ProbabilityKeyInfo<TSource>(element, curProbability);
                        buckets.Add(curKey, keyInfo);
                    }
                }
                // the resulting set of the method is one entry per key
                List<TSource> targetSet = new List<TSource>( buckets.Keys.Count );

                // At second we iterate through classified data and apply random method 
                //Random rnd = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
                foreach (var dictEntry in buckets)
                {
                    ProbabilityKeyInfo<TSource> keyInfo = dictEntry.Value;
                    // there is only one item - no probabilities
                    if (keyInfo.Items.Count == 1)
                    {
                        targetSet.Add(keyInfo.Items[0].First);
                    }
                    else
                    {
                        double rndNum = m_sRandGen.NextDouble();
                        targetSet.Add(keyInfo.RetrieveElementByProb(rndNum));
                    }

                }
                return targetSet;
            }
        }

        private static Random m_sRandGen = new Random();

        /// <summary>
        /// Ref object that keeps stats for probability calcuation. 
        /// </summary>
        /// <remarks>
        /// It is located in the dictionary that is why not struct.
        /// </remarks>
        private class ProbabilityKeyInfo<TElement>
        {
            #region public Properties
            
            public double MaxProb { get; private set; }
            public double MinProb { get; private set; }
            public List<Pair<TElement, double>> Items { get; private set; }
            public double SumProb { get; private set; }

            #endregion public Properties

            public ProbabilityKeyInfo(TElement item, double prob)
            {
                SumProb = MaxProb = MinProb = prob;
                Items = new List<Pair<TElement, double>>();
                Items.Add(new Pair<TElement, double>(item, prob));
            }
            /// <summary>
            /// Introduces a new element to the container by updating statistics: 
            ///     Max Probability, 
            ///     Min Probability,
            ///     Sum of all Probabilities.
            /// </summary>
            /// <remarks>
            /// This method adds zero fix by adding <see cref="ProbabilityDelta"/> to all probabilities.
            /// </remarks>
            /// <param name="item">The item to be stored.</param>
            /// <param name="prob">The probability score of the <paramref name="item"/>.</param>
            public void AddElement(TElement item, double prob) 
            {
                if (MaxProb < prob)
                {
                    MaxProb = prob;
                }
                if (MinProb > prob)
                {
                    MinProb = prob;
                }
                SumProb += prob;
                Items.Add(new Pair<TElement, double>(item, prob));
            }

            /// <summary>
            /// Returns the element of <see cref="Items"/> according to <paramref name="rndNum"/>
            /// </summary>
            /// <remarks>
            /// This method uses <paramref name="rndNum"/> that is in the range [0,1] as factor to <see cref="SumProb"/>. 
            /// Resulting number identifies the item from the beginning of <see cref="Items"/> list accoring to its weight (probability score)
            /// that is choosen 
            /// </remarks>
            /// <param name="rndNum">Number in range of [0,1]</param>
            /// <returns>
            /// The first element from <see cref="Items"/> which weight together with weights of all previous elements 
            /// exceeds <paramref name="rndNum"/>*<see cref="SumProb"/>.
            /// </returns>
            public TElement RetrieveElementByProb(double rndNum)
            {
                if (rndNum>1 || rndNum<0) 
                    throw new ArgumentException("The argument must be in range [0,1]");
                System.Diagnostics.Debug.Assert( SumProb.ProbabilityEquals(1.0),
                    string.Format("The SumProb={0} is not normalized", SumProb));
                double rndRel = SumProb * rndNum; // since SumProb should be equal to 1 this line is for future growth
                foreach (var item in Items)
                {
                    rndRel -= item.Second;
                    if (rndRel <= double.Epsilon)
                    {
                        return item.First;
                    }
                }
                throw new InvalidProgramException("RetrieveElementByProb: the loop ended without return!");
            }

            /// <summary>
            /// Used in order to cancel zero probability problem
            /// </summary>
            //private const double ProbabilityDelta = 0.01;

            //private double m_curSumProb = 0.0;
        }
    }
}
