using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary
{
    public class Region
    {
        public Coordinate TopLeft { get; set; }
        public Coordinate TopRight { get; set; }
        public Coordinate BottomLeft { get; set; }
        public Coordinate BottomRight { get; set; }

        public string Label { get; set; }

   
        public int Height
        {
            get { return Math.Abs(TopLeft.X - BottomLeft.X); }
        }

        public int Width
        {
            get { return Math.Abs(TopRight.Y - TopLeft.Y); }
        }

        public Region(Coordinate topLeft, Coordinate topRight, Coordinate bottomLeft, Coordinate bottomRight, string label)
        {
            this.BottomLeft = bottomLeft;
            this.BottomRight = bottomRight;
            this.TopLeft = topLeft;
            this.TopRight = topRight;
        }

        /// <summary>
        /// Checks if a region is overlapping with another region
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool IsOverlappedRegion(Region region)
        {
            if (this.TopLeft.X > region.BottomRight.X || region.TopLeft.X > this.BottomRight.X)
                return false;

            return this.TopLeft.Y >= region.BottomRight.Y && region.TopLeft.Y >= this.BottomRight.Y;
        }

        /// <summary>
        /// Checks if a point is within the region
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public bool IsWithinRegion(Coordinate coordinate)
        {
            double length = Math.Abs(this.TopLeft.Y - this.BottomLeft.Y);
            double width = Math.Abs(this.TopRight.X - this.TopLeft.X);

            double areaRectangle = length * width;

            // Area of Triangle AQB
            double areaAQB = Math.Abs((this.BottomLeft.X * (this.TopLeft.Y - coordinate.Y) +
                              this.TopLeft.X * (coordinate.Y - this.BottomLeft.Y) +
                              coordinate.X * (this.BottomLeft.Y - this.TopLeft.Y)) / 2.0);

            // Area of Triangle BQC
            double areaBQC = Math.Abs((this.TopRight.X * (this.TopLeft.Y - coordinate.Y) +
                             this.TopLeft.X * (coordinate.Y - this.TopRight.Y) +
                             coordinate.X * (this.TopRight.Y - this.TopLeft.Y)) / 2.0);

            // Area of Triangle CQD
            double areaCQD = Math.Abs((this.TopRight.X * (this.BottomRight.Y - coordinate.Y) +
                             this.BottomRight.X * (coordinate.Y - this.TopRight.Y) +
                             coordinate.X * (this.TopRight.Y - this.BottomRight.Y)) / 2.0);

            // Area of Triangle AQD
            double areaAQD = Math.Abs((this.BottomRight.X * (coordinate.Y - this.BottomLeft.Y) +
                             this.BottomLeft.X * (this.BottomRight.Y - coordinate.Y) +
                             coordinate.X * (this.BottomRight.Y - this.BottomLeft.Y)) / 2.0);

            return areaRectangle == (areaAQB + areaBQC + areaCQD + areaAQD);
        }
    }
}
