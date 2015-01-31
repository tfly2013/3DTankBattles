// Authors: Fei Tang
// Last Modified: 20/10/2014
// HeightMap file used to generate Landscape
//Use SharpDX
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project2
{
    /// <summary>
    /// The height map which is used for generating landscape
    /// </summary>
    public class HeightMap
    {
        public float[,] heights;
        public int density;
        public float range;
        public float scale;
        public float unitLength;

        public float maxHeight;
        public float minHeight;
        public float diff;

        /// <summary>
        /// Generate a height map for landscape using Diamond Square Algorithm.
        /// </summary>
        /// <param name="density">The density of height map.</param>
        /// <param name="range">The range of height.(from -range to range)</param>
        /// <param name="scale">The length and width of landscape.</param>
        public HeightMap(int density, float range, float scale)
        {
            this.density = density;
            this.range = range;
            this.scale = scale;
            unitLength = scale / density;
            heights = diamondSquare(density, range);

            // Calculate the maximum and mininum height on the height map
            minHeight = maxHeight = heights[0, 0];
            for (int i = 0; i < density; i++)
                for (int j = 0; j < density; j++)
                {
                    minHeight = Math.Min(minHeight, heights[i, j]);
                    maxHeight = Math.Max(maxHeight, heights[i, j]);
                }
            diff = maxHeight - minHeight;
        }

        /// <summary>
        /// Indexer of Height Map.
        /// </summary>
        public float this[int i, int j]
        {
            get
            {
                return heights[i, j];
            }
        }

        public float GetHeight(float x, float z)
        {
            if (x != float.NaN && z != float.NaN)
            {
                int i = Convert.ToInt32((x + scale / 2) / unitLength);
                int j = Convert.ToInt32((z + scale / 2) / unitLength);
                if (i >= 0 && i < density && j >= 0 && j < density)
                {
                    return heights[i, j];
                }
            }
            return float.NaN;
        }

        /// <summary>
        /// Color the landscape base on the distribution of height
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public float GetTexture(float height)
        {
            if (height - minHeight > diff * 0.8)
                return 0;
            else if (height - minHeight > diff * 0.6)
                return 0.25f;
            else if (height - minHeight > diff * 0.3)
                return 0.5f;
            else
                return 0.75f;
        }

        /// <summary>
        /// Diamond Square algorithm. 
        /// </summary>
        /// <param name="density">The density of height map.</param>
        /// <param name="range">The range of height.(from -range to range)</param>
        /// <returns>The 2D array that contains heights.</returns>
        public float[,] diamondSquare(int density, float range)
        {
            float[,] data = new float[density, density];
            // set the initial value of corners to 0
            data[0, 0] =
            data[0, density - 1] =
            data[density - 1, 0] =
            data[density - 1, density - 1] = 0;

            // Initialize random
            Random r = new Random();

            // the length of side of square or diamond
            int side = density - 1;
            // keep dividing side until reach 1
            while (side > 1)
            {
                // Square
                for (int x = 0; x < density - 1; x += side)
                {
                    for (int y = 0; y < density - 1; y += side)
                    {
                        // calculate average of corners
                        float average = (data[x, y] + data[x + side, y] +
                            data[x, y + side] + data[x + side, y + side]) / 4;

                        // centre of square is the average plus a random value within +-range
                        data[x + side / 2, y + side / 2] = r.NextFloat(0, 1) * range * 2 - range + average;
                    }
                }

                // Diamond
                for (int x = 0; x < density - 1; x += side / 2)
                {
                    for (int y = (x + side / 2) % side; y < density - 1; y += side)
                    {
                        // x, y is centre of diamond
                        // calculate average of the midpoint of each side
                        float average = (data[(x - side / 2 + density - 1) % (density - 1), y] +
                                data[(x + side / 2) % (density - 1), y] +
                                data[x, (y + side / 2) % (density - 1)] +
                                data[x, (y - side / 2 + density - 1) % (density - 1)]) / 4;

                        // centre of diamond is the average plus a random value within +-range
                        average = r.NextFloat(0, 1) * range * 2 - range + average;
                        data[x, y] = average;

                        // handle values on the edges
                        if (x == 0) data[density - 1, y] = average;
                        if (y == 0) data[x, density - 1] = average;
                    }
                }
                // each iteration half the side of square and diamonds and the range
                side /= 2;
                range /= 2.0f;
            }

            for (int i = 0; i < density; i++)
            {
                for (int j = 0; j < density; j++)
                {
                    data[i, j] -= data[side / 2, side / 2];
                }
            }
            return data;
        }
    }
}
