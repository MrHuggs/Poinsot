//
// Double version of the Unity vector3 class. Uses (mostly) the same conventions.
// 

// Define to  make DQuaternion use doubles:
// define targ_double
// 
// Define to enable conversions in/out of Unity types
// (you don't want this if you are using the class stand-alone)
//#define USE_UNITY

using System;

#if targ_double
using targ_type = System.Double;
#else
using targ_type = System.Single;
#endif

#if USE_UNITY
using UnityEngine;
using UVector3 = UnityEngine.Vector3;
#endif

namespace Assets.DoubleMath
{
	public struct DVector3 : IEquatable<DVector3>
	{
		private static readonly DVector3 zeroVector = new DVector3(0.0f, 0.0f, 0.0f);
		private static readonly DVector3 oneVector = new DVector3(1f, 1f, 1f);
		private static readonly DVector3 upVector = new DVector3(0.0f, 1f, 0.0f);
		private static readonly DVector3 downVector = new DVector3(0.0f, -1f, 0.0f);
		private static readonly DVector3 leftVector = new DVector3(-1f, 0.0f, 0.0f);
		private static readonly DVector3 rightVector = new DVector3(1f, 0.0f, 0.0f);
		private static readonly DVector3 forwardVector = new DVector3(0.0f, 0.0f, 1f);
		private static readonly DVector3 backVector = new DVector3(0.0f, 0.0f, -1f);
		private static readonly DVector3 positiveInfinityVector = new DVector3(targ_type.PositiveInfinity, targ_type.PositiveInfinity, targ_type.PositiveInfinity);
		private static readonly DVector3 negativeInfinityVector = new DVector3(targ_type.NegativeInfinity, targ_type.NegativeInfinity, targ_type.NegativeInfinity);

		public targ_type x;
		public targ_type y;
		public targ_type z;

		public DVector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public DVector3(double x, double y, double z)
		{
			this.x = (targ_type) x;
			this.y = (targ_type) y;
			this.z = (targ_type) z;
		}


		public void Set(targ_type newX, targ_type newY, targ_type newZ)
		{
			this.x = newX;
			this.y = newY;
			this.z = newZ;
		}

		public static DVector3 Scale(DVector3 a, DVector3 b)
		{
			return new DVector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public void Scale(DVector3 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
		}

		public static DVector3 Cross(DVector3 lhs, DVector3 rhs)
		{
			return new DVector3((targ_type)((double)lhs.y * (double)rhs.z - (double)lhs.z * (double)rhs.y), (targ_type)((double)lhs.z * (double)rhs.x - (double)lhs.x * (double)rhs.z), (targ_type)((double)lhs.x * (double)rhs.y - (double)lhs.y * (double)rhs.x));
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
		}

		public override bool Equals(object other)
		{
			if (!(other is DVector3))
				return false;
			return this.Equals((DVector3)other);
		}

		public bool Equals(DVector3 other)
		{
			return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z);
		}

		public static DVector3 Normalize(DVector3 value)
		{
			targ_type num = DVector3.Magnitude(value);
			if ((double)num > 9.99999974737875E-06)
				return value / num;
			return DVector3.zero;
		}

		public void Normalize()
		{
			targ_type num = DVector3.Magnitude(this);
			if ((double)num > 9.99999974737875E-06)
				this = this / num;
			else
				this = DVector3.zero;
		}

		public DVector3 normalized
		{
			get
			{
				return DVector3.Normalize(this);
			}
		}

		public static targ_type Dot(DVector3 lhs, DVector3 rhs)
		{
			return (targ_type)((double)lhs.x * (double)rhs.x + (double)lhs.y * (double)rhs.y + (double)lhs.z * (double)rhs.z);
		}

		public static targ_type Distance(DVector3 a, DVector3 b)
		{
			DVector3 DVector3 = new DVector3(a.x - b.x, a.y - b.y, a.z - b.z);
			return (targ_type) Math.Sqrt((targ_type)((double)DVector3.x * (double)DVector3.x + (double)DVector3.y * (double)DVector3.y + (double)DVector3.z * (double)DVector3.z));
		}

		public static targ_type Magnitude(DVector3 vector)
		{
			return (targ_type) Math.Sqrt((targ_type)((double)vector.x * (double)vector.x + (double)vector.y * (double)vector.y + (double)vector.z * (double)vector.z));
		}

		public targ_type magnitude
		{
			get
			{
				return (targ_type) Math.Sqrt((targ_type)((double)this.x * (double)this.x + (double)this.y * (double)this.y + (double)this.z * (double)this.z));
			}
		}

		public static targ_type SqrMagnitude(DVector3 vector)
		{
			return (targ_type)((double)vector.x * (double)vector.x + (double)vector.y * (double)vector.y + (double)vector.z * (double)vector.z);
		}


		public targ_type sqrMagnitude
		{
			get
			{
				return (targ_type)((double)this.x * (double)this.x + (double)this.y * (double)this.y + (double)this.z * (double)this.z);
			}
		}
		public static DVector3 zero
		{
			get
			{
				return DVector3.zeroVector;
			}
		}

		public static DVector3 one
		{
			get
			{
				return DVector3.oneVector;
			}
		}

		public static DVector3 forward
		{
			get
			{
				return DVector3.forwardVector;
			}
		}

		public static DVector3 back
		{
			get
			{
				return DVector3.backVector;
			}
		}

		public static DVector3 up
		{
			get
			{
				return DVector3.upVector;
			}
		}

		public static DVector3 down
		{
			get
			{
				return DVector3.downVector;
			}
		}

		public static DVector3 left
		{
			get
			{
				return DVector3.leftVector;
			}
		}

		public static DVector3 right
		{
			get
			{
				return DVector3.rightVector;
			}
		}

		public static DVector3 positiveInfinity
		{
			get
			{
				return DVector3.positiveInfinityVector;
			}
		}

		public static DVector3 negativeInfinity
		{
			get
			{
				return DVector3.negativeInfinityVector;
			}
		}

		public static DVector3 operator +(DVector3 a, DVector3 b)
		{
			return new DVector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static DVector3 operator -(DVector3 a, DVector3 b)
		{
			return new DVector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static DVector3 operator -(DVector3 a)
		{
			return new DVector3(-a.x, -a.y, -a.z);
		}

		public static DVector3 operator *(DVector3 a, targ_type d)
		{
			return new DVector3(a.x * d, a.y * d, a.z * d);
		}

		public static DVector3 operator *(targ_type d, DVector3 a)
		{
			return new DVector3(a.x * d, a.y * d, a.z * d);
		}

		public static DVector3 operator /(DVector3 a, targ_type d)
		{
			return new DVector3(a.x / d, a.y / d, a.z / d);
		}

		public static bool operator ==(DVector3 lhs, DVector3 rhs)
		{
			return (double)DVector3.SqrMagnitude(lhs - rhs) < 9.99999943962493E-11;
		}

		public static bool operator !=(DVector3 lhs, DVector3 rhs)
		{
			return !(lhs == rhs);
		}


		public override string ToString()
		{
			return string.Format("({0:F1}, {1:F1}, {2:F1})", (object)this.x, (object)this.y, (object)this.z);
		}


		public string ToString(string format)
		{
			return string.Format("({0}, {1}, {2})", (object)this.x.ToString(format), (object)this.y.ToString(format), (object)this.z.ToString(format));
		}
#if USE_UNITY
		public DVector3(UVector3 v)
		{
			x = v.x;
			y = v.y;
			z = v.z;
		}


		public static DVector3 FromUnity(UVector3 v)
		{
			return new DVector3(v.x, v.y, v.z);
		}

		public static UVector3 ToUnity(DVector3 v)
		{
			return new UVector3((float)v.x, (float)v.y, (float)v.z);
		}
#endif
	}
}
