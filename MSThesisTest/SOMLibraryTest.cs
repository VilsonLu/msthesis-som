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

            List<int> ignoreColumns = dataset.GetIgnoreColumns();

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

            List<int> ignoreColumns = dataset.GetIgnoreColumns();

            // Assert
            string expected = "Id";
            string result = dataset.Features[ignoreColumns[0]].FeatureName;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Dataset_GetInstance_WithLabelsAndKeys()
        {
            // Arrange
            string filename = @"Dataset\Iris.csv";
            IReader reader = new CSVReader(filename);
            SOM som = new SOM();

            som.GetData(reader);

            Dataset dataset = som.Dataset;
            dataset.SetKey("Id");
            dataset.SetLabel("Species");

            // Act
            double[] result = dataset.GetInstance<double>(0);


            // Assert
            int expectedCount = 4;
            double x1 = 5.1;
            double x2 = 3.5;
            double x3 = 1.4;
            double x4 = 0.2;
            Assert.AreEqual(expectedCount, result.Length);
            Assert.AreEqual(x1, result[0]);
            Assert.AreEqual(x2, result[1]);
            Assert.AreEqual(x3, result[2]);
            Assert.AreEqual(x4, result[3]);
        }

        [TestMethod]
        public void Test_Method()
        {
            // Arrange
            string filename = @"Dataset\Iris.csv";
            IReader reader = new CSVReader(filename);
            SOM som = new SOM(10,10, 0.5);

            som.GetData(reader);

            Dataset dataset = som.Dataset;
            dataset.SetKey("Id");
            dataset.SetLabel("Species");

            // Act
            som.Train();
            bool test = true;
        }
    }

}
