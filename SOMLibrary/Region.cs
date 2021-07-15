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
            get { return Math.Abs(BottomLeft.X - TopLeft.X); }
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
            this.Label = label;
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
            if (coordinate.X >= this.TopLeft.X && coordinate.X <= this.BottomRight.X && coordinate.Y >= this.TopLeft.Y && coordinate.Y <= this.BottomRight.Y)
                return true;

            return false;
        }
    }
}
