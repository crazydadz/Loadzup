using UnityEngine;
using JetBrains.Annotations;

namespace Silphid.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Filter(this Vector2 This, Vector2 inputValue, float smoothness)
        {
            return This * smoothness + inputValue * (1 - smoothness);
        }

        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

		public static Vector2 Multiply(this Vector2 This, Vector2 other)
		{
			return new Vector2(This.x * other.x, This.y * other.y);
		}

        public static Vector2 ClampToBounds(this Vector2 vector, Rect rect)
        {
            vector.x = Mathf.Clamp(vector.x, rect.xMin, rect.xMax);
            vector.y = Mathf.Clamp(vector.y, rect.yMin, rect.yMax);

            return vector;
        }

		/// <summary>
		/// Returns value clamped to the [min, max] interval
		/// </summary>
		[Pure]
		public static Vector2 Clamp(this Vector2 This, Vector2 min, Vector2 max)
		{
			return new Vector2(This.x.Clamp(min.x, max.x), This.y.Clamp(min.y, max.y));
		}

        public static Vector3 ToVector3(this Vector2 vector2, float z = 0)
        {
            return new Vector3(vector2.x, vector2.y, z);
        }

		public static Vector2 SetX(this Vector2 This, float x)
        {
			return new Vector2(x, This.y);
        }

		public static Vector2 SetY(this Vector2 This, float y)
        {
			return new Vector2(This.x, y);
        }

        public static float Length(this Vector2 vect)
        {
            return Mathf.Sqrt(vect.x * vect.x + vect.y * vect.y);
        }

		/// <summary>
		/// Returns interpolated value at given ratio, between This and target values.
		/// </summary>
		[Pure]
		public static Vector2 InterpolateTo(this Vector2 This, Vector2 target, float ratio)
		{
			return This + ((target - This) * ratio);
		}

		/// <summary>
		/// Returns quadratically interpolated value at given ratio, between This and target values, using one control point.
		/// </summary>
		[Pure]
		public static Vector2 QuadraticInterpolateTo(this Vector2 This, Vector2 target, Vector2 controlPoint, float ratio)
		{
			// First level
			var a = This.InterpolateTo(controlPoint, ratio);
			var b = controlPoint.InterpolateTo(target, ratio);

			// Second level
			return a.InterpolateTo(b, ratio);
		}

		/// <summary>
		/// Returns cubically interpolated value at given ratio, between This and target values, using two control points.
		/// </summary>
		[Pure]
		public static Vector2 CubicInterpolateTo(this Vector2 This, Vector2 target, Vector2 controlPoint1, Vector2 controlPoint2, float ratio)
		{
			// First level
			var a = This.InterpolateTo(controlPoint1, ratio);
			var b = controlPoint1.InterpolateTo(controlPoint2, ratio);
			var c = controlPoint2.InterpolateTo(target, ratio);

			// Second level
			var ab = a.InterpolateTo(b, ratio);
			var bc = b.InterpolateTo(c, ratio);

			// Third level
			return ab.InterpolateTo(bc, ratio);
		}
    }
}