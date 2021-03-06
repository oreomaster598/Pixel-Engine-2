/*
  Box2DNet Copyright (c) 2018 codeyu https://github.com/codeyu/Box2DNet
  Box2D original C++ version Copyright (c) 2006-2007 Erin Catto http://www.gphysics.com
  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:
  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using System; using System.Numerics;
using System.Collections.Generic;
using System.Text;
 

using Random = System.Random;

namespace PE2.Physics.Common
{
	public static class Vector2Extension 
	{	
        public static float Rad2Deg = (float)360.0f / (float)(System.Math.PI * 2);
		public static Vector3 ToVector3(this Vector2 vector) 
		{ 
			return new Vector3(vector.X, vector.Y, 0.0f);
		}
		
		public static bool IsValid(this Vector2 vector)
		{
			return Math.IsValid(vector.X) && Math.IsValid(vector.Y);
		}
		
		public static float Cross(this Vector2 vector, Vector2 other) 
		{ 
			return vector.X * other.Y - vector.Y * other.X;
		}
		
		public static Vector2 CrossScalarPostMultiply(this Vector2 vector, float s) 
		{ 
			return new Vector2(s * vector.Y, -s * vector.X);
		}
		
		public static Vector2 CrossScalarPreMultiply(this Vector2 vector, float s)
		{
			return new Vector2(-s * vector.Y, s * vector.X);
		}
		
		public static Vector2 Abs(this Vector2 vector) { 
			return new Vector2(Math.Abs(vector.X), Math.Abs(vector.Y));
		}
	}
	
	public static class Vector3Extension 
	{ 
		public static Vector2 ToVector2(this Vector3 vector) 
		{
			return new Vector2(vector.X, vector.Y);
		}
	}
	
	public static class QuaternionExtension 
	{
		public static Quaternion FromAngle2D(float radians) 
		{ 
			return Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), radians * ((float)360.0f / (float)(System.Math.PI * 2)));
		}
	}
	
	public class Math
	{
		public static readonly ushort USHRT_MAX = 0xffff;
		public static readonly byte UCHAR_MAX = 0xff;
		public static readonly int RAND_LIMIT = 32767;

		/// <summary>
		/// This function is used to ensure that a floating point number is
		/// not a NaN or infinity.
		/// </summary>
		public static bool IsValid(float x)
		{
			return !(float.IsNaN(x) || float.IsNegativeInfinity(x) || float.IsPositiveInfinity(x));
		}

		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
		public struct Convert
		{
			[System.Runtime.InteropServices.FieldOffset(0)]
			public float x;

			[System.Runtime.InteropServices.FieldOffset(0)]
			public int i;
		}
	

#if USE_MATRIX_FOR_ROTATION
	 	public static Mat22 AngleToRotation(float angle) 
		{
			return new Mat22(angle);
		}
#else
		public static Quaternion AngleToRotation(float angle)
		{
			return QuaternionExtension.FromAngle2D(angle);
		}
#endif 

		/// <summary>
		/// This is a approximate yet fast inverse square-root.
		/// </summary>
		public static float InvSqrt(float x)
		{
			Convert convert = new Convert();
			convert.x = x;
			float xhalf = 0.5f * x;
			convert.i = 0x5f3759df - (convert.i >> 1);
			x = convert.x;
			x = x * (1.5f - xhalf * x * x);
			return x;
		}

		public static float Clamp(float f, float min, float max) {
			if (f < min)
				return min;
			else if (f > max)
				return max;
			else return f;
		}
		public static float Rad2Deg = 57.29578f;
		public static float Epsilon = 1.401298E-45f;
		public static float Sqrt(float x)
		{
			return Math.Sqrt(x);
		}
		public static float Distance(Vector2 v1, Vector2 v2) {
			return (float)System.Math.Sqrt(System.Math.Pow(v2.X - v1.X, 2) + System.Math.Pow(v2.Y - v1.Y, 2));
		}
		
		/// <summary>
		/// Random floating point number in range [lo, hi]
		/// </summary>

		/// <summary>
		/// "Next Largest Power of 2
		/// Given a binary integer value x, the next largest power of 2 can be computed by a SWAR algorithm
		/// that recursively "folds" the upper bits into the lower bits. This process yields a bit vector with
		/// the same most significant 1 as x, but all 1's below it. Adding 1 to that value yields the next
		/// largest power of 2. For a 32-bit value:"
		/// </summary>
		public static uint NextPowerOfTwo(uint x)
		{
			x |= (x >> 1);
			x |= (x >> 2);
			x |= (x >> 4);
			x |= (x >> 8);
			x |= (x >> 16);
			return x + 1;
		}

		public static bool IsPowerOfTwo(uint x)
		{
			bool result = x > 0 && (x & (x - 1)) == 0;
			return result;
		}

		public static float Abs(float a)
		{
			return a > 0.0f ? a : -a;
		}

		public static Vector2 Abs(Vector2 a)
		{
			return new Vector2(Math.Abs(a.X), Math.Abs(a.Y));
		}

		public static Mat22 Abs(Mat22 A)
		{
			Mat22 B = new Mat22();
			B.Set(Math.Abs(A.Col1), Math.Abs(A.Col2));
			return B;
		}

		public static float Min(float a, float b)
		{
			return a < b ? a : b;
		}

		public static int Min(int a, int b)
		{
			return a < b ? a : b;
		}

		public static float Max(float a, float b)
		{
			return a > b ? a : b;
		}

		public static int Max(int a, int b)
		{
			return a > b ? a : b;
		}

		public static Vector2 Clamp(Vector2 a, Vector2 low, Vector2 high)
		{
			return Vector2.Max(low, Vector2.Min(a, high));
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			T tmp = a;
			a = b;
			b = tmp;
		}

		/// <summary>
		/// Multiply a matrix times a vector. If a rotation matrix is provided,
		/// then this Transforms the vector from one frame to another.
		/// </summary>
		public static Vector2 Mul(Mat22 A, Vector2 v)
		{
			return new Vector2(A.Col1.X * v.X + A.Col2.X * v.Y, A.Col1.Y * v.X + A.Col2.Y * v.Y);
		}

		/// <summary>
		/// Multiply a matrix transpose times a vector. If a rotation matrix is provided,
		/// then this Transforms the vector from one frame to another (inverse Transform).
		/// </summary>
		public static Vector2 MulT(Mat22 A, Vector2 v)
		{
			return new Vector2(Vector2.Dot(v, A.Col1), Vector2.Dot(v, A.Col2));
		}

		/// <summary>
		/// A * B
		/// </summary>
		public static Mat22 Mul(Mat22 A, Mat22 B)
		{
			Mat22 C = new Mat22();
			C.Set(Math.Mul(A, B.Col1), Math.Mul(A, B.Col2));
			return C;
		}

		/// <summary>
		/// A^T * B
		/// </summary>
		public static Mat22 MulT(Mat22 A, Mat22 B)
		{
			Vector2 c1 = new Vector2(Vector2.Dot(A.Col1, B.Col1), Vector2.Dot(A.Col2, B.Col1));
			Vector2 c2 = new Vector2(Vector2.Dot(A.Col1, B.Col2), Vector2.Dot(A.Col2, B.Col2));
			return new Mat22(c1, c2);
		}
	
		public static Vector2 Mul(Transform T, Vector2 v)
		{
#if USE_MATRIX_FOR_ROTATION
			return T.position + Math.Mul(T.R, v);
#else
			return T.position + T.TransformDirection(v);
#endif
		}

		public static Vector2 MulT(Transform T, Vector2 v)
		{
#if USE_MATRIX_FOR_ROTATION
			return Math.MulT(T.R, v - T.position);
#else
			return T.InverseTransformDirection(v - T.position);
#endif
		}
	
		/// <summary>
		/// Multiply a matrix times a vector.
		/// </summary>
		public static Vector3 Mul(Mat33 A, Vector3 v)
		{
			return v.X * A.Col1 + v.Y * A.Col2 + v.Z * A.Col3;
		}

		public static float Atan2(float y, float x)
		{
			return (float)System.Math.Atan2(y, x);
		}
	}
}