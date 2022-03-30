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
}
