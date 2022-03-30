using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using HelixToolkit.Wpf;
using CustomExtensions;

namespace TestMatrix3D
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // Euler to Matrix4x4
            Vector3D eulerInput = new Vector3D(Math.PI, 30.515, 151.515);
            Matrix3D matrix4x4 = ConvertEulerXYZtoMatrix3D(eulerInput);

            // Convert matrix to Euler angles
            Vector3D eulerXYZ_1 = new Vector3D();
            Vector3D eulerXYZ_2 = new Vector3D();
            ConvertMatrix3DtoEulerXYZ(matrix4x4, ref eulerXYZ_1, ref eulerXYZ_2);

            // Display results
            Console.WriteLine("\nEuler inputs:");
            Console.WriteLine(eulerInput.ToString("\t"));
            Console.WriteLine("\nEuler XYZ solution 1:");
            Console.WriteLine(eulerXYZ_1.ToString("\t"));
            Console.WriteLine("\nEuler XYZ solution 2:");
            Console.WriteLine(eulerXYZ_2.ToString("\t"));

            Console.WriteLine(ConversionSuccesful(eulerInput));

            Console.ReadLine();
        }

        /// <summary>
        /// Returns 4x4 matrix (row major/post-multiply convention) representing Euler XYZ angles (degree) transformation. 
        /// </summary>
        /// <param name="eulerXYZ">Euler angles in degrees</param>
        /// <returns></returns>
        static Matrix3D ConvertEulerXYZtoMatrix3D(Vector3D eulerXYZ)
        {
            RotateTransform3D Rx = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), eulerXYZ.X)); // Angle in degrees
            RotateTransform3D Ry = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), eulerXYZ.Y));
            RotateTransform3D Rz = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), eulerXYZ.Z));
            return (Rx.Value * Ry.Value * Rz.Value); // Order of transformation is left to right
        }

        /// <summary>
        /// Calculates euler angles in XYZ convention from 4x4 transformation matrix and returns both possible euler solutions. 
        /// For gimbal lock, set Z angle to 0 and determine Returns NaN for solution
        /// </summary>
        /// <param name="matrix4x4">Transformation matrix in Row Major/post-multiply convention</param>
        /// <param name="eulerXYZ_1">Solution 1 euler angles in XYZ convention [radians]</param>
        /// <param name="eulerXYZ_2">Solution 2 euler angles in XYZ convention [radians]</param>
        static void ConvertMatrix3DtoEulerXYZ(Matrix3D matrix4x4, ref Vector3D eulerXYZ_1, ref Vector3D eulerXYZ_2)
        {
            // Note: Matrix3D uses row major/post-multiply convention. i.e. DestinationRowVector = SourceRowVector * Matrix4x4
            // Therefore the resultant 3x3 rotation matrix is the transpose of the standard rotation matrix (column vector convention is the most common standard)

            if (Math.Abs(matrix4x4.M13) != 1)
            {
                // Y angle (theta)
                eulerXYZ_1.Y = -Math.Asin(matrix4x4.M13); // 1st unique value of y angle
                eulerXYZ_2.Y = Math.PI - eulerXYZ_1.Y; // 2nd unique value of y angle

                // X angle (psi)
                double cosY1 = Math.Cos(eulerXYZ_1.Y);
                double cosY2 = Math.Cos(eulerXYZ_2.Y);

                eulerXYZ_1.X = Math.Atan2(matrix4x4.M23 / cosY1, matrix4x4.M33/ cosY1);
                eulerXYZ_2.X = Math.Atan2(matrix4x4.M23 / cosY2, matrix4x4.M33 / cosY2);

                // Z angle (phi)
                eulerXYZ_1.Z = Math.Atan2(matrix4x4.M12 / cosY1, matrix4x4.M11 / cosY1);
                eulerXYZ_2.Z = Math.Atan2(matrix4x4.M12 / cosY2, matrix4x4.M11 / cosY2);
            }

            if (matrix4x4.M13 == -1) // Gimbal lock. Infinite solutions
            {
                // Z angle (phi)
                eulerXYZ_1.Z = 0; // Z angle can be anything. Set to 0.
                eulerXYZ_2.Z = Double.NaN;

                // Y angle (theta)
                eulerXYZ_1.Y = Math.PI / 2;
                eulerXYZ_2.Y = Double.NaN; // Only one solution for given Z angle

                // X angle (psi)
                eulerXYZ_1.X = eulerXYZ_1.Y + Math.Atan2(matrix4x4.M21, matrix4x4.M31);
                eulerXYZ_2.Y = Double.NaN; // Only one solution for given Z angle
            }

            if (matrix4x4.M13 == 1) // Gimbal lock. Infinite solutions
            {
                // Z angle (phi)
                eulerXYZ_1.Z = 0; // Z angle can be anything. Set to 0.
                eulerXYZ_2.Z = Double.NaN;

                // Y angle (theta)
                eulerXYZ_1.Y = -Math.PI / 2;
                eulerXYZ_2.Y = Double.NaN; // Only one solution for given Z angle

                // X angle (psi)
                eulerXYZ_1.X = -eulerXYZ_1.Y + Math.Atan2(-matrix4x4.M21, -matrix4x4.M31);
                eulerXYZ_2.Y = Double.NaN; // Only one solution for given Z angle
            }

            // Convert from radians to degrees
            eulerXYZ_1 = eulerXYZ_1 / Math.PI * 180;
            eulerXYZ_2 = eulerXYZ_2 / Math.PI * 180;

        }
                
        static bool EqualWithinTolerance(double value1, double value2, double tolerance)
        {
            return Math.Abs(value1 - value2) < tolerance;
        }

        static bool EqualWithinTolerance(Vector3D vector1, Vector3D vector2, double tolerance)
        {
            bool result = true;
            result = EqualWithinTolerance(vector1.X, vector2.X, tolerance) && result;
            result = EqualWithinTolerance(vector1.Y, vector2.Y, tolerance) && result;
            result = EqualWithinTolerance(vector1.Z, vector2.Z, tolerance) && result;
            return result;
        }

        static bool ConversionSuccesful(Vector3D eulerInput)
        {
            double tolerance = 0.0001;

            // Define a Rotation from Euler angles
            Matrix3D matrix4x4 = ConvertEulerXYZtoMatrix3D(eulerInput);

            // Convert matrix back to Euler angles
            Vector3D eulerXYZ_1 = new Vector3D();
            Vector3D eulerXYZ_2 = new Vector3D();
            ConvertMatrix3DtoEulerXYZ(matrix4x4, ref eulerXYZ_1, ref eulerXYZ_2);

            if(eulerXYZ_2.X == Double.NaN && eulerXYZ_2.Y == Double.NaN && eulerXYZ_2.Z == Double.NaN) // Check for gimbal lock
            {
                Console.WriteLine("Gimbal lock. eulerInput: " + eulerInput.ToString());
                return true;
            }

            // Test result
            bool compare_1 = EqualWithinTolerance(eulerXYZ_1, eulerInput, tolerance);
            bool compare_2 = EqualWithinTolerance(eulerXYZ_2, eulerInput, tolerance);

            return compare_1 || compare_2;
        }

    }
}


