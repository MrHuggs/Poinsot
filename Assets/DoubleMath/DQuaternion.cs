//
// Double version of the Unity quaternion class. Uses (mostly) the same conventions.
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
using Vector3 = UnityEngine.Vector3;
using UVector3 = UnityEngine.Vector3;
using UQuaternion = UnityEngine.Quaternion;
#endif

namespace Assets.DoubleMath
{
	public struct DQuaternion : IEquatable<DQuaternion>
	{
		private static readonly DQuaternion identityQuaternion = new DQuaternion(0.0, 0.0, 0.0, 1);
		public targ_type x;
		public targ_type y;
		public targ_type z;
		public targ_type w;
		public const targ_type kEpsilon = (targ_type) 1E-06f;

		public DQuaternion(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public DQuaternion(double x, double y, double z, double w)
		{
			this.x = (targ_type) x;
			this.y = (targ_type)y;
			this.z = (targ_type)z;
			this.w = (targ_type)w;
		}

		public static DQuaternion Inverse(DQuaternion q)
		{
			// Note that Unity's inverse method assumes a normalized quaterion, which makes
			// the inverse much faster.
			targ_type l2 = Dot(q, q);
			if ((double) l2 < (double)Mathf.Epsilon)
				return identityQuaternion;
			DQuaternion cong = DQuaternion.Conjugate(q);
			cong.Scale(1 / l2);
			return cong;
		}
		public static DQuaternion AngleAxis(double angle, DVector3 axis)
		{
			var naxis = axis.normalized;

			// Unity expects to pass in an angle in degrees. We want the half angle in radians:
			double a_2 = angle * (.5 * (Math.PI * 2) / 360);
			double s_2 = Math.Sin(a_2);
			double c_2 = Math.Cos(a_2);

			return new DQuaternion(naxis.x * s_2, naxis.y * s_2, naxis.z * s_2, c_2);
		}

		public void Set(float newX, float newY, float newZ, float newW)
		{
			x = newX;
			y = newY;
			z = newZ;
			w = newW;
		}
		public void Set(double newX, double newY, double newZ, double newW)
		{
			x = (targ_type)newX;
			y = (targ_type)newY;
			z = (targ_type)newZ;
			w = (targ_type)newW;
		}

		public static DQuaternion identity
		{
			get
			{
				return DQuaternion.identityQuaternion;
			}
		}

		public static DQuaternion Normalize(DQuaternion q)
		{
			targ_type num = (targ_type) Math.Sqrt(DQuaternion.Dot(q, q));
			if ((double)num < (double)Mathf.Epsilon)
				return DQuaternion.identity;
			return new DQuaternion(q.x / num, q.y / num, q.z / num, q.w / num);
		}

		public void Normalize()
		{
			targ_type num = (targ_type)Math.Sqrt(DQuaternion.Dot(this, this));
			if ((double)num < (double)Mathf.Epsilon)
				Set(0, 0, 0, 1);
			else
				Set(x / num, y / num, z / num, w / num);
		}

		public DQuaternion normalized
		{
			get
			{
				return DQuaternion.Normalize(this);
			}
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
		}

		public override bool Equals(object other)
		{
			if (!(other is DQuaternion))
				return false;
			return this.Equals((DQuaternion)other);
		}

		public bool Equals(DQuaternion other)
		{
			return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z) && this.w.Equals(other.w);
		}

		public override string ToString()
		{
			return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", (object)this.x, (object)this.y, (object)this.z, (object)this.w);
		}

		public string ToString(string format)
		{
			return string.Format("({0}, {1}, {2}, {3})", (object)this.x.ToString(format), (object)this.y.ToString(format), (object)this.z.ToString(format), (object)this.w.ToString(format));
		}


		public static DQuaternion operator *(DQuaternion lhs, DQuaternion rhs)
		{
			return new DQuaternion((targ_type)((double)lhs.w * (double)rhs.x + (double)lhs.x * (double)rhs.w + (double)lhs.y * (double)rhs.z - (double)lhs.z * (double)rhs.y), 
				(targ_type)((double)lhs.w * (double)rhs.y + (double)lhs.y * (double)rhs.w + (double)lhs.z * (double)rhs.x - (double)lhs.x * (double)rhs.z), 
				(targ_type)((double)lhs.w * (double)rhs.z + (double)lhs.z * (double)rhs.w + (double)lhs.x * (double)rhs.y - (double)lhs.y * (double)rhs.x), 
				(targ_type)((double)lhs.w * (double)rhs.w - (double)lhs.x * (double)rhs.x - (double)lhs.y * (double)rhs.y - (double)lhs.z * (double)rhs.z));
		}

		public static DVector3 operator *(DQuaternion rotation, DVector3 point)
		{
			targ_type num1 = rotation.x * 2;
			targ_type num2 = rotation.y * 2;
			targ_type num3 = rotation.z * 2;
			targ_type num4 = rotation.x * num1;
			targ_type num5 = rotation.y * num2;
			targ_type num6 = rotation.z * num3;
			targ_type num7 = rotation.x * num2;
			targ_type num8 = rotation.x * num3;
			targ_type num9 = rotation.y * num3;
			targ_type num10 = rotation.w * num1;
			targ_type num11 = rotation.w * num2;
			targ_type num12 = rotation.w * num3;
			DVector3 vector3;
			vector3.x = (targ_type)((1.0 - ((double)num5 + (double)num6)) * (double)point.x + ((double)num7 - (double)num12) * (double)point.y + ((double)num8 + (double)num11) * (double)point.z);
			vector3.y = (targ_type)(((double)num7 + (double)num12) * (double)point.x + (1.0 - ((double)num4 + (double)num6)) * (double)point.y + ((double)num9 - (double)num10) * (double)point.z);
			vector3.z = (targ_type)(((double)num8 - (double)num11) * (double)point.x + ((double)num9 + (double)num10) * (double)point.y + (1.0 - ((double)num4 + (double)num5)) * (double)point.z);
			return vector3;
		}

		private static bool IsEqualUsingDot(targ_type dot)
		{
			return (double)dot > 0.999998986721039;
		}


		public static bool operator ==(DQuaternion lhs, DQuaternion rhs)
		{
			return DQuaternion.IsEqualUsingDot(DQuaternion.Dot(lhs, rhs));
		}

		public static bool operator !=(DQuaternion lhs, DQuaternion rhs)
		{
			return !(lhs == rhs);
		}

		public static targ_type Dot(DQuaternion a, DQuaternion b)
		{
			return (targ_type)((double)a.x * (double)b.x + (double)a.y * (double)b.y + (double)a.z * (double)b.z + (double)a.w * (double)b.w);
		}

		// New method:
		public static DQuaternion Conjugate(DQuaternion q)
		{
			return new DQuaternion(-q.x, -q.y, -q.z, q.w);
		}

		public void Scale(targ_type s)
		{
			x *= s;
			y *= s;
			z *= s;
			w *= s;
		}

#if USE_UNITY
		public DQuaternion(UQuaternion uq)
		{
			x = uq.x;
			y = uq.y;
			z = uq.z;
			w = uq.w;
		}


		public static DQuaternion FromUnity(UQuaternion uq)
		{
			return new DQuaternion(uq.x, uq.y, uq.z, uq.w);
		}

		public static UQuaternion ToUnity(DQuaternion uq)
		{
			return new UQuaternion((float) uq.x, (float) uq.y, (float) uq.z, (float) uq.w);
		}

		static public void Test()
		{
			UQuaternion q1 = new UQuaternion(.1f, .2f, .3f, -4);
			UQuaternion q2 = new UQuaternion(10, 9, -8, 7);

			Debug.Assert(q1.Equals(ToUnity(FromUnity(q1))));
			Debug.Assert(q2.Equals(ToUnity(FromUnity(q2))));

			Debug.Assert(q1.normalized.EqTest(ToUnity(FromUnity(q1).normalized)));
			Debug.Assert(q2.normalized.EqTest(ToUnity(FromUnity(q2).normalized)));

			q1.Normalize();		// Unity's inverse method only works for normalized
			q2.Normalize();		// Quaternions.
		
			Debug.Assert(UQuaternion.Inverse(q1).Equals(ToUnity(Inverse(FromUnity(q1)))));
			Debug.Assert(UQuaternion.Inverse(q2).EqTest(ToUnity(Inverse(FromUnity(q2)))));


			Debug.Assert((q1 * q2).Equals(ToUnity(FromUnity(q1 * q2))));
			Debug.Assert((q2 * q1).Equals(ToUnity(FromUnity(q2 * q1))));

			Vector3 axis;
			DVector3 daxis;
			float angle;

			axis = new Vector3(1, 2, 3);
			daxis = DVector3.FromUnity(axis);
			angle = 27;

			Debug.Assert(UQuaternion.AngleAxis(angle, axis).EqTest(ToUnity(DQuaternion.AngleAxis(angle, daxis))));

			axis = new Vector3(-1, 2, -.1f);
			daxis = DVector3.FromUnity(axis);
			angle = -500;
			Debug.Assert(UQuaternion.AngleAxis(angle, axis).EqTest(ToUnity(DQuaternion.AngleAxis(angle, daxis))));


		}


#endif
	};


#if USE_UNITY
	static class DQTest
	{
		const double Epsilon = .0001;
		// Equality check for Unity Quaternions using an epsilon.
		public static bool EqTest(this UnityEngine.Quaternion ob, UnityEngine.Quaternion other)
		{
			double d = Math.Abs(ob.x - other.x) + Math.Abs(ob.y - other.y) + Math.Abs(ob.z - other.z) + Math.Abs(ob.w - other.w);

			if (d > Epsilon)
			{
				Debug.Log("Quat compare failed:");
				Debug.Log(string.Format("{0}, {1}, {2}, {3}", ob.x, ob.y, ob.z, ob.w));
				Debug.Log(string.Format("{0}, {1}, {2}, {3}", other.x, other.y, other.z, other.w));
				return false;
			}
			return true;
		}
	};
#endif
}
