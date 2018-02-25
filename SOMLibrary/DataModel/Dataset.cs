using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOMLibrary.Extensions;

namespace SOMLibrary.DataModel
{
    public class Dataset
    {
        public Feature[] Features { get; set; }

        public Instance[] Instances { get; set; }

        /// <summary>
        /// Set the feature to a label
        /// </summary>
        /// <param name="feature"></param>
        public void SetLabel(string feature)
        {
            var selectedFeature = Features.First(x => x.FeatureName == feature);

            if (selectedFeature == null)
            {
                throw new Exception("Feature does not exists");
            }

            selectedFeature.IsLabel = true;
        }

        /// <summary>
        /// Set the feature to an ID
        /// </summary>
        /// <param name="feature"></param>
        public void SetKey(string feature)
        {
            var selectedFeature = Features.First(x => x.FeatureName == feature);

            if (selectedFeature == null)
            {
                throw new Exception("Feature does not exists");
            }

            selectedFeature.IsKey = true;
        }

        /// <summary>
        /// Get the specific instance in the dataset while ignoring the labels and keys
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="i">index</param>
        /// <returns></returns>
        public T[] GetInstance<T>(int i)
        {
            int instanceCounts = Instances.Length;

            if (i > instanceCounts)
            {
                throw new Exception("Selected instance does not exists");
            }

            var ignoredColumn = GetIgnoreColumns();
            List<T> values = new List<T>();
            var instance = Instances[i].Values;

            for (int j = 0; j < instance.Length; j++)
            {
                if (ignoredColumn.Any(x => x == j))
                {
                    continue;
                }

                values.Add(instance[j].ConvertType<T>());
            }

            return values.ToArray();
        }

        /// <summary>
        /// Get the label of an instance
        /// </summary>
        /// <param name="i">index</param>
        /// <param name="feature">feature name to get the label</param>
        /// <returns></returns>
        public string GetInstanceLabel(int i, string feature)
        {
            // TODO: validate the feature first

            int index = Features.Where(x => x.FeatureName == feature && x.IsLabel).Select(x => x.OrderNo).FirstOrDefault();

            return Instances[i].Values[index].ToString();
        }

        /// <summary>
        /// Get list of columns not to be used for training
        /// Example: Id, Labels
        /// </summary>
        /// <returns></returns>
        public List<int> GetIgnoreColumns()
        {
            var ignoreColumns = Features.Where(x => x.IsKey || x.IsLabel).Select(x => x.OrderNo);
            return ignoreColumns.ToList();
        }

        /// <summary>
        /// Returns the number of instances
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get
            {
                return Instances.Length;
            } 
        }
    }
}
