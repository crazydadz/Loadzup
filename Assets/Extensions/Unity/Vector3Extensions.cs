using UnityEngine;
using JetBrains.Annotations;

namespace Silphid.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 Subtract(this Vector3 vector, Vector3 other)
        {
            return new Vector3(vector.x - other.x, vector.y - other.y, vector.z - other.z);
        }

        public static Vector3 Multiply(this Vector3 vector, Vector3 other)
        {
            return new Vector3(vector.x * other.x, vector.y * other.y, vector.z * other.z);
        }

        public static Vector3 WithX(this Vector3 This, float x)
        {
            return new Vector3(x, This.y, This.z);
        }

        public static Vector3 WithY(this Vector3 This, float y)
        {
            return new Vector3(This.x, y, This.z);
        }

        public static Vector3 WithZ(this Vector3 This, float z)
        {
            return new Vector3(This.x, This.y, z);
        }

        public static float Distance(this Vector3 This, Vector3 other) =>
            Vector3.Distance(This, other);

        /// <summary>
		/// Returns interpolated value at given ratio, between This and target values.
		/// </summary>
		[Pure]
		public static Vector3 InterpolateTo(this Vector3 This, Vector3 target, float ratio)
		{
			return This + ((target - This) * ratio);
		}
    }
}