using System.Linq;
using UnityEngine;

namespace Silphid.Extensions
{
    public static class TransformExtensions
    {
        public static void SetX(this Transform transform, float x)
        {
            var pos = transform.position;
            pos.x = x;
            transform.position = pos;
        }

        public static void SetY(this Transform transform, float y)
        {
            var pos = transform.position;
            pos.y = y;
            transform.position = pos;
        }

        public static void SetZ(this Transform transform, float z)
        {
            var pos = transform.position;
            pos.z = z;
            transform.position = pos;
        }

        public static void SetLocalX(this Transform transform, float x)
        {
            var pos = transform.localPosition;
            pos.x = x;
            transform.localPosition = pos;
        }

        public static void SetLocalY(this Transform transform, float y)
        {
            var pos = transform.localPosition;
            pos.y = y;
            transform.localPosition = pos;
        }

        public static void SetLocalZ(this Transform transform, float z)
        {
            var pos = transform.localPosition;
            pos.z = z;
            transform.localPosition = pos;
        }

        public static RectTransform AsRectTransform(this Transform transform)
        {
            return (RectTransform)transform;
        }

        public static Rect GetBounds(this Transform transform)
        {
            var rectTransform = transform.AsRectTransform();
            var rect = rectTransform.rect;
            var pos = rectTransform.position;

            var left = pos.x - rectTransform.pivot.x * rectTransform.rect.width;
            var bottom = pos.y - rectTransform.pivot.y * rectTransform.rect.height;

            var rect2 = new Rect(left, bottom, rect.width, rect.height);
            return rect2;
        }

        public static float GetHeight(this Transform transform)
        {
            return transform.AsRectTransform().rect.height;
        }

        public static float GetWidth(this Transform transform)
        {
            return transform.AsRectTransform().rect.width;
        }

        public static void TranslateAndClamp(this Transform transform, Vector2 vector)
        {
            var parentBound = transform.parent.GetBounds();

            var bound = transform.GetBounds();
            var bound2 = bound.Translate(vector);

            if (bound2.xMin < parentBound.xMin)
                vector.x += (parentBound.xMin - bound2.xMin);
            if (bound2.yMin < parentBound.yMin)
                vector.y += (parentBound.yMin - bound2.yMin);
            if (bound2.xMax > parentBound.xMax)
                vector.x += (parentBound.xMax - bound2.xMax);

            transform.Translate(vector);
        }

        public static GameObject FindInChildrenByName(this Transform transform, string name)
        {
            return (from Transform t in transform where t.name == name select t.gameObject).FirstOrDefault();
        }
    }
}