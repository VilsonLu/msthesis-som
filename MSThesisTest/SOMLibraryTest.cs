using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SOMLibrary;
using SOMLibrary.Interface;
using SOMLibrary.Implementation;
using SOMLibrary.DataModel;

namespace MSThesisTest
{
    [TestClass]
    public class SOMLibraryTest
    {
        [TestMethod]
        public void Node_GetDistance_EuclideanDistance()
        {
            // Arrange
            var weights = new double[] { 0.1, 0.4, 0.5 };
            var inputVectors = new double[] { 1, 0, 0 };

            Node node = new Node(weights, 1, 1);

            // Act 
            double distance = node.GetDistance(inputVectors);

            // Assert
            double expectedResult = 1.1045361;
            Assert.AreEqual(expectedResult, Math.Round(distance, 7));
        }

        [TestMethod]
        public void SOM_MapRadius_CalculateMapRadius()
        {
            // Arrange
            var x = 10;
            var y = 8;

            // Act
            SOM som = new SOM(x, y);
            double radius = som.MapRadius;

            // Assert
            double expected = 5.0;
            Assert.AreEqual(expected, radius);
        }

        [TestMethod]
        public void CSVReader_Read_GetHeaders()
        {
            // Arrange
            string filename = @"Dataset\Iris.csv";
            IReader reader = new CSVReader(filename);

            // Act
            Dataset dataset = reader.Read();

            // Assert
            int expected = 6;
            Assert.AreEqual(expected, dataset.Features.Length);
        }

        [TestMethod]
        public void CSVReader_Read_GetInstances()
        {
            // Arrange
            string filename = @"Dataset\Iris.csv";
            IReader reader = new CSVReader(filename);

            // Act
            Dataset dataset = reader.Read();

            // Assert
            int expected = 150;
            Assert.AreEqual(expected, dataset.Instances.Length);
        }


        [TestMethod]
        public void SOM_IgnoreColumns_GetIsLabelColumn_WhenIsLabelTrue()
        {
            // Arrange
            string filename = @"Dataset\Iris.csv";
            IReader reader = new CSVReader(filename);
            SOM som = new SOM();

            som.GetData(reader);

            // Act
            Dataset dataset = som.Dataset;
            dataset.SetLabel("Species");

            List<int> ignoreColumns = som.IgnoreColumns();

            // Assert
            string expected = "Species";
            string result = dataset.Features[ignoreColumns[0]].FeatureName;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SOM_IgnoreColumns_GetIsKeyColumn_WhenIsLabelTrue()
        {
            // Arrange
            string filename = @"Dataset\Iris.csv";
            IReader reader = new CSVReader(filename);
            SOM som = new SOM();

            som.GetData(reader);

            // Act
            Dataset dataset = som.Dataset;
            dataset.SetKey("Id");

            List<int> ignoreColumns = som.IgnoreColumns();

            // Assert
            string expected = "Id";
            string result = dataset.Features[ignoreColumns[0]].FeatureName;
            Assert.AreEqual(expected, result);
        }
    }

}
