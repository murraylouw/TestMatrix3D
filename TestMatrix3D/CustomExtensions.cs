using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CustomExtensions
{
    public static class Vector3DExtension
    {
        public static string ToString(this Vector3D vec, string delimiter)
        {
            return vec.X.ToString() + delimiter + vec.Y.ToString() + delimiter + vec.Z.ToString() ;
        }
    }

    public static class RandomExtension
    {
        public static double NextDoubleInRange(this Random random, double lowerBound, double upperBound)
        {
            double randomDouble = random.NextDouble();
            double randomDoubleInRange = randomDouble * (upperBound - lowerBound) + lowerBound;
            return randomDoubleInRange;
        }

        public static Vector3D NextEulerXYZ(this Random random)
        {
            Vector3D eulerXYZ = new Vector3D(random.NextDoubleInRange(-180, 180), random.NextDoubleInRange(-180, 180), random.NextDoubleInRange(-180, 180));
            return eulerXYZ;
        }
    }
}
