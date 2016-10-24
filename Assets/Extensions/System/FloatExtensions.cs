using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Silphid.Extensions
{
    public static class FloatExtensions
    {
        private const float SmallValue = 0.0001f;

        #region Float

        [Pure]
        public static float Filter(this float This, float inputValue, float smoothness)
        {
            return This * smoothness + inputValue * (1 - smoothness);
        }

        [Pure]
        public static string ToInvariantString(this float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        [Pure]
        public static int Sign(this float value)
        {
            return Math.Sign(value);
        }

        [Pure]
        public static float Abs(this float value)
        {
            return Math.Abs(value);
        }

        [Pure]
        public static float Floor(this float value)
        {
            return (float)Math.Floor(value);
        }

        [Pure]
        public static float Ceiling(this float value)
        {
            return (float)Math.Ceiling(value);
        }

        /// <summary>
        /// Returns absolute delta between to values.
        /// </summary>
        [Pure]
        public static float Distance(this float from, float to)
        {
            return Math.Abs(from - to);
        }

        /// <summary>
        /// Returns value clamped to the [min, max] interval
        /// </summary>
        [Pure]
        public static float Clamp(this float value, float min, float max)
        {
            return (min < max) ? value.Minimum(min).Maximum(max) : value.Minimum(max).Maximum(min);
        }

        /// <summary>
        /// Returns value clamped to the [0, 1] interval
        /// </summary>
        [Pure]
        public static float ClampUnit(this float value)
        {
            return value.Clamp(0, 1);
        }

        /// <summary>
        /// Returns value clipped to the [min, +INF] interval
        /// </summary>
        [Pure]
        public static float Minimum(this float value, float min)
        {
            return Math.Max(value, min);
        }

        /// <summary>
        /// Returns value clipped to the [-INF, max] interval
        /// </summary>
        [Pure]
        public static float Maximum(this float value, float max)
        {
            return Math.Min(value, max);
        }

        /// <summary>
        /// Returns [0, 1] ratio of given value within the [min, max] interval
        /// </summary>
        [Pure]
        public static float Ratio(this float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        /// <summary>
        /// Returns [0, 1] ratio of given value within the [min, max] interval,
        /// clamped within that interval.
        /// </summary>
        [Pure]
        public static float RatioClamp(this float value, float min, float max)
        {
            return value.Clamp(min, max).Ratio(min, max);
        }

		/// <summary>
		/// Returns interpolated value at given ratio, between This and target values.
		/// </summary>
		[Pure]
		public static float InterpolateTo(this float This, float target, float ratio)
		{
			return This + ((target - This) * ratio);
		}

		/// <summary>
		/// Returns quadratically interpolated value at given ratio, between This and target values, using one control point.
		/// </summary>
		[Pure]
		public static float QuadraticInterpolateTo(this float This, float target, float controlPoint, float ratio)
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
		public static float CubicInterpolateTo(this float This, float target, float controlPoint1, float controlPoint2, float ratio)
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

        /// <summary>
        /// Returns [minTo, maxTo] interpolation of given value within the [minFrom, maxFrom] interval
        /// </summary>
        [Pure]
        public static float Transpose(this float value, float minFrom, float maxFrom, float minTo, float maxTo)
        {
			return minTo.InterpolateTo(maxTo, value.Ratio(minFrom, maxFrom));
        }

        /// <summary>
        /// Returns [minTo, maxTo] interpolation of given value within the [minFrom, maxFrom] interval,
        /// clipped to the [minTo, maxTo] interval
        /// </summary>
        [Pure]
        public static float TransposeClamp(this float value, float minFrom, float maxFrom, float minTo, float maxTo)
        {
            return value.Transpose(minFrom, maxFrom, minTo, maxTo).Clamp(minTo, maxTo);
        }

        /// <summary>
        /// Returns fractional part of given value.
        /// </summary>
        [Pure]
        public static float Fraction(this float value)
        {
            return value - value.Floor();
        }

        /// <summary>
        /// Returns wrapped value in order to fit within [from, to] range.
        /// </summary>
        [Pure]
        public static float Wrap(this float value, float from, float to)
        {
            float ratio = Ratio(value, from, to);
            ratio -= (float)Math.Floor(ratio);
			return from.InterpolateTo(to, ratio);
        }

        /// <summary>
        /// Applies Hermite interpolation to smooth given ratio (ease-in and ease-out)
        /// </summary>
        [Pure]
        public static float SmoothRatio(this float ratio)
        {
            return (ratio * ratio) * (3 - 2 * ratio);
        }

        [Pure]
        public static bool IsAlmostZero(this float value)
        {
            return value.IsAlmostEqualTo(0f);
        }

        [Pure]
        public static bool IsAlmostEqualTo(this float value, float otherValue)
        {
            return IsAlmostEqualTo(value, otherValue, SmallValue);
        }

        [Pure]
        public static bool IsAlmostEqualTo(this float value, float otherValue, float epsilon)
        {
            return Math.Abs(value - otherValue) <= epsilon;
        }

        [Pure]
        public static float Round(this float value)
        {
            return (float)Math.Round(value);
        }

        [Pure]
        public static float Snap(this float value, float increment)
        {
            return (float) Math.Round(value / increment) * increment;
        }

        [Pure]
        public static int RoundToInt(this float value)
        {
            return (int)Math.Round(value);
        }

        [Pure]
        public static bool IsWithin(this float value, float min, float max)
        {
            return (min < max) ? (min < value && value < max) : (max < value && value < min);
        }

        #endregion
    }
}