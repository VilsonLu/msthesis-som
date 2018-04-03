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
        public void Region_IsOverlappingRegion_OverlappingRectangle_ReturnTrue()
        {
            // Arrange
            var p1 = new Coordinate(1, 5);
            var p2 = new Coordinate(4, 5);
            var p3 = new Coordinate(1, 2);
            var p4 = new Coordinate(4, 2);
            var region1 = new Region(p1, p2, p3, p4, "");

            var r1 = new Coordinate(3, 7);
            var r2 = new Coordinate(6, 7);
            var r3 = new Coordinate(3, 4);
            var r4 = new Coordinate(6, 4);
            var region2 = new Region(r1, r2, r3, r4, "");

            // Act
            bool result = region1.IsOverlappedRegion(region2);

            // Assert
            bool expectedResult = true;
            Assert.AreEqual(expectedResult, result);

        }

        [TestMethod]
        public void Region_IsOverlappedRegion_NotOverlappingRectangle_ReturnFalse()
        {
            // Arrange
            var p1 = new Coordinate(1, 5);
            var p2 = new Coordinate(4, 5);
            var p3 = new Coordinate(1, 2);
            var p4 = new Coordinate(4, 2);
            var region1 = new Region(p1, p2, p3, p4, "");

            var r1 = new Coordinate(7, 3);
            var r2 = new Coordinate(9, 3);
            var r3 = new Coordinate(7, 1);
            var r4 = new Coordinate(9, 4);
            var region2 = new Region(r1, r2, r3, r4, "");

            // Act
            bool result = region1.IsOverlappedRegion(region2);

            // Assert
            bool expectedResult = false;
            Assert.AreEqual(expectedResult, result);

        }

        [TestMethod]
        public void Region_IsWithinRegion_PointIsInRectangle_ReturnTrue()
        {
            // Arrange
            var p1 = new Coordinate(1, 5);
            var p2 = new Coordinate(4, 5);
            var p3 = new Coordinate(1, 2);
            var p4 = new Coordinate(4, 2);
            var region = new Region(p1, p2, p3, p4, "");

            var point = new Coordinate(2, 3);

            // Act
            bool result = region.IsWithinRegion(point);

            // Assert
            bool expectedResult = true;
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void Region_IsWithinRegion_PointIsInBoundary_ReturnTrue()
        {
            // Arrange
            var p1 = new Coordinate(1, 5);
            var p2 = new Coordinate(4, 5);
            var p3 = new Coordinate(1, 2);
            var p4 = new Coordinate(4, 2);
            var region = new Region(p1, p2, p3, p4, "");

            var point = new Coordinate(1, 4);

            // Act
            bool result = region.IsWithinRegion(point);

            // Assert
            bool expectedResult = true;
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void Region_IsWithinRegion_PointIsOutsideRegion_ReturnFalse()
        {
            // Arrange
            var p1 = new Coordinate(1, 5);
            var p2 = new Coordinate(4, 5);
            var p3 = new Coordinate(1, 2);
            var p4 = new Coordinate(4, 2);
            var region = new Region(p1, p2, p3, p4, "");

            var point = new Coordinate(4, 1);

            // Act
            bool result = region.IsWithinRegion(point);

            // Assert
            bool expectedResult = false;
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void Region_Height_CalculateHeight()
        {
            // Arrange
            var p1 = new Coordinate(15, 15);
            var p2 = new Coordinate(15, 20);
            var p3 = new Coordinate(20, 15);
            var p4 = new Coordinate(20, 20);
            var region = new Region(p1, p2, p3, p4, "");


            // Act
            int result = region.Height;

            // Assert
            var expectedResult = 5;
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void Region_Width_CalculateWidth()
        {
            // Arrange
            var p1 = new Coordinate(15, 15);
            var p2 = new Coordinate(15, 20);
            var p3 = new Coordinate(20, 15);
            var p4 = new Coordinate(20, 20);
            var region = new Region(p1, p2, p3, p4, "");

            // Act
            int result = region.Width;

            // Assert
            var expectedResult = 5;
            Assert.AreEqual(expectedResult, result);
        }



        public void Test_Method()
        {
            // Arrange
            string filename = @"C:\Users\Vilson\Documents\Visual Studio 2017\Projects\MSThesis\SOMClient\Dataset\Iris.csv";
            IReader reader = new CSVReader(filename);
            SOM som = new SOM(10, 10, 0.5);

            som.GetData(reader);
            som.FeatureLabel = "Species";

            Dataset dataset = som.Dataset;
            dataset.SetKey("Id");
            dataset.SetLabel("Species");

            // Act
            som.Train();
            som.LabelNodes();
            bool test = true;
        }
    }

}
