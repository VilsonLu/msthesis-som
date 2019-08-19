using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SOMLibrary;
using SOMLibrary.Interface;
using SOMLibrary.Implementation;
using SOMLibrary.DataModel;
using FluentAssertions;
using ML.TrajectoryAnalysis.Implementation;
using ML.TrajectoryAnalysis;
using ML.Common.Implementation;
using SOMLibrary.Implementation.LearningRate;
using SOMLibrary.Implementation.DistanceMeasure;
using ML.Common;

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

            IDistanceMeasure distanceMeasure = new EuclideanDistance();
            // Act 
            double distance = distanceMeasure.GetDistance(weights, inputVectors);

            // Assert
            double expectedResult = 1.1045361;
            Assert.AreEqual(expectedResult, Math.Round(distance, 7));
        }

        [TestMethod]
        public void CSVReader_Read_GetHeaders()
        {
            // Arrange
            string filename = @"Dataset\Iris.csv";
            string[] ignoreColumns = new string[0] { };
            string label = "Species";
            IReader reader = new CSVReader(filename, ignoreColumns, label);

            // Act
            Dataset dataset = reader.Read();

            // Assert
            int expected = 5;
            Assert.AreEqual(expected, dataset.Features.Length);
        }

        [TestMethod]
        public void CSVReader_Read_GetInstances()
        {
            // Arrange
            string filename = @"Dataset\Iris.csv";
            string[] ignoreColumns = new string[0] { };
            string label = "Species";
            IReader reader = new CSVReader(filename, ignoreColumns, label);

            // Act
            Dataset dataset = reader.Read();

            // Assert
            int expected = 150;
            Assert.AreEqual(expected, dataset.Instances.Length);
        }

        [TestMethod]
        public void CosineSimilarity_MeasureSimilarity_Positive()
        {
            // Arrange
            double[] a = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            double[] b = new double[] { 3.0, 4.0, 5.0, 6.0, 7.0 };

            IDistanceMeasure distanceMeasure = new CosineSimilarityMeasure();

            // Act
            double actual = distanceMeasure.GetDistance(a, b);

            // Assert
            double expectedValue = 4.465;

            actual.Should().BeApproximately(expectedValue, 0.001);

        }


        [TestMethod]
        public void CosineSimilarity_MeasureSimilarity_Negative()
        {
            // Arrange
            double[] a = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
            double[] b = new double[] { -3.0, -4.0, -5.0, -6.0, -7.0 };

            IDistanceMeasure distanceMeasure = new CosineSimilarityMeasure();

            // Act
            double actual = distanceMeasure.GetDistance(a, b);

            // Assert
            double expectedValue = -4.465;

            actual.Should().BeApproximately(expectedValue, 0.001);

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
            var p2 = new Coordinate(20, 15);
            var p3 = new Coordinate(15, 20);
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
            var p2 = new Coordinate(20, 15);
            var p3 = new Coordinate(15, 20);
            var p4 = new Coordinate(20, 20);
            var region = new Region(p1, p2, p3, p4, "");

            // Act
            int result = region.Width;

            // Assert
            var expectedResult = 5;
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void Instance_ToString_GetTheConcatenatedValue()
        {
            Instance instance = new Instance();
            instance.OrderNo = 1;

            double[] values = { 0.5, 0.7, 0.8, 0.9 };

            instance.Values = values;

            var actualResult = instance.ToString();

            var expectedResult = "0.5 0.7 0.8 0.9";

            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void PowerSeriesLearning_CalculateLearningRate_Initial()
        {
            // Arrange
            ILearningRate learningRate = new PowerSeriesLearningRate(0.75);

            // Act
            var result = learningRate.CalculateLearningRate(0, 10);

            // Assert
            double expected = 0.75;
            result.Should<double>().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void PowerSeriesLearning_CalculateLearningRate_Halfway()
        {
            // Arrange
            ILearningRate learningRate = new PowerSeriesLearningRate(0.75);

            // Act
            var result = learningRate.CalculateLearningRate(5, 10);

            // Assert
            double expected = 0.454;
            result.Should().BeApproximately(expected, 0.001);
        }

        [TestMethod]
        public void PowerSeriesLearning_CalculateLearningRate_End()
        {
            // Arrange
            ILearningRate learningRate = new PowerSeriesLearningRate(0.75);

            // Act
            var result = learningRate.CalculateLearningRate(10, 10);

            // Assert
            double expected = 0.275;
            result.Should().BeApproximately(expected, 0.001);
        }

        [TestMethod]
        public void EuclideanDistanceFast_GetDistance_CalculateEuclideanDistance()
        {
            IDistanceMeasure distance = new EuclideanDistanceFast();

            var weights = new double[] { 0.1, 0.4, 0.5 };
            var inputVectors = new double[] { 1, 0, 0 };

            double actualResult = distance.GetDistance(weights, inputVectors);

            // Assert
            double expectedResult = 1.1045361;
            Assert.AreEqual(expectedResult, Math.Round(actualResult, 7));

        }

        [TestMethod]
        public void EditDistanceMeasure_GetDistance_Substitution_CalculateLevenstheinDistance()
        {
            ISimilarityMeasure similarityMeasure = new EditDistanceMeasure();

            var weights = new double[] { 0.1, 0.4, 0.5 };
            var trajectoryA = new List<Trajectory>()
            {
                new Trajectory()
                {
                    Node = new Node(weights, 1, 1)
                    {
                        ClusterLabel = "AA"
                    }
                }
            };

            var trajectoryB = new List<Trajectory>()
            {
                new Trajectory()
                {
                    Node = new Node(weights, 1, 1)
                    {
                        ClusterLabel = "BB"
                    }
                }
            };

            double actualResult = similarityMeasure.MeasureSimilarity(trajectoryA, trajectoryB);

            // Assert
            double expectedResult = 4.0;
            Assert.AreEqual(expectedResult, Math.Round(actualResult, 7));

        }

        [TestMethod]
        public void EditDistanceMeasure_GetDistance_Addition_CalculateLevenstheinDistance()
        {
            ISimilarityMeasure similarityMeasure = new EditDistanceMeasure();

            var weights = new double[] { 0.1, 0.4, 0.5 };
            var trajectoryA = new List<Trajectory>()
            {
                new Trajectory()
                {
                    Node = new Node(weights, 1, 1)
                    {
                        ClusterLabel = "AAAA"
                    }
                }
            };

            var trajectoryB = new List<Trajectory>()
            {
                new Trajectory()
                {
                    Node = new Node(weights, 1, 1)
                    {
                        ClusterLabel = "AACC"
                    }
                }
            };

            double actualResult = similarityMeasure.MeasureSimilarity(trajectoryA, trajectoryB);

            // Assert
            double expectedResult = 2.0;
            Assert.AreEqual(expectedResult, Math.Round(actualResult, 7));

        }

        [TestMethod]
        public void EditDistanceMeasure_GetDistance_Deletion_CalculateLevenstheinDistance()
        {
            ISimilarityMeasure similarityMeasure = new EditDistanceMeasure();

            var weights = new double[] { 0.1, 0.4, 0.5 };
            var trajectoryA = new List<Trajectory>()
            {
                new Trajectory()
                {
                    Node = new Node(weights, 1, 1)
                    {
                        ClusterLabel = "AACC"
                    }
                }
            };

            var trajectoryB = new List<Trajectory>()
            {
                new Trajectory()
                {
                    Node = new Node(weights, 1, 1)
                    {
                        ClusterLabel = "AA"
                    }
                }
            };

            double actualResult = similarityMeasure.MeasureSimilarity(trajectoryA, trajectoryB);

            // Assert
            double expectedResult = 2.0;
            Assert.AreEqual(expectedResult, Math.Round(actualResult, 7));

        }

        [TestMethod]
        public void EditDistanceMeasure_GetDistance_Mixed_CalculateLevenstheinDistance()
        {
            ISimilarityMeasure similarityMeasure = new EditDistanceMeasure();

            var weights = new double[] { 0.1, 0.4, 0.5 };
            var trajectoryA = new List<Trajectory>()
            {
                new Trajectory()
                {
                    Node = new Node(weights, 1, 1)
                    {
                        ClusterLabel = "AABBCC"
                    }
                }
            };

            var trajectoryB = new List<Trajectory>()
            {
                new Trajectory()
                {
                    Node = new Node(weights, 1, 1)
                    {
                        ClusterLabel = "BBDDEE"
                    }
                }
            };

            double actualResult = similarityMeasure.MeasureSimilarity(trajectoryA, trajectoryB);

            // Assert
            double expectedResult = 8.0;
            Assert.AreEqual(expectedResult, Math.Round(actualResult, 7));

        }

        [TestMethod]
        [TestCategory("String Compression")]
        public void RunLengthCompression_NoCharactersInDifferentPlaces()
        {
            // Arrange
            ICompression compression = new RunLengthCompression();

            // Act
            string result = compression.Compress("ABBBBBBBBCCCCC");

            // Assert
            string expectedResult = "ABBCC";
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        [TestCategory("String Compression")]
        public void RunLengthCompression_Compress_WithMultipleCharactersInDifferentPlaces()
        {
            // Arrange
            ICompression compression = new RunLengthCompression();

            // Act
            string result = compression.Compress("ABBBBBBBBCCCCCAAAA");

            // Assert
            string expectedResult = "ABBCCAA";
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        [TestCategory("String Compression")]
        public void RunLengthCompression_Compress_NoRepeatingCharacters()
        {
            // Arrange
            ICompression compression = new RunLengthCompression();

            // Act
            string result = compression.Compress("ABC");

            // Assert
            string expectedResult = "ABC";
            result.Should().BeEquivalentTo(expectedResult);
        }
    }

}
