using System.Globalization;
using Microsoft.Xna.Framework;

namespace ChanceOfPrecipitation
{
    /// <summary>A floating point rectangle, that is otherwise equivalent to the integer Rectangle from the XNA framework.</summary>
    public struct RectangleF
    {
        #region Constants

        /// <summary>
        ///     An empty <see cref="RectangleF"/> at the origin.
        /// </summary>
        public static readonly RectangleF Empty = new RectangleF(0, 0, 0, 0);

        #endregion

        #region Properties

        /// <summary>Returns the x-coordinate of the left side of the rectangle.</summary>
        public float Left
        {
            get { return x; }
        }

        /// <summary>Returns the y-coordinate of the top of the rectangle.</summary>
        public float Top
        {
            get { return y; }
        }

        /// <summary>Returns the x-coordinate of the right side of the rectangle.</summary>
        public float Right
        {
            get { return x + width; }
        }

        /// <summary>Returns the y-coordinate of the bottom of the rectangle.</summary>
        public float Bottom
        {
            get { return y + height; }
        }

        /// <summary>Gets the Point that specifies the center of the rectangle.</summary>
        public Vector2 Center
        {
            get {
                Vector2 v;
                v.X = x + width / 2;
                v.Y = y + height / 2;
                return v;
            }
        }

        /// <summary>
        ///     Gets or sets the upper-left value of the <see cref="RectangleF"/>.
        /// </summary>
        public Vector2 Location
        {
            get {
                Vector2 v;
                v.X = x;
                v.Y = y;
                return v;
            }
            set {
                x = value.X;
                y = value.Y;
            }
        }

        /// <summary>
        ///     Gets a value that indicates whether the <see cref="RectangleF"/> is empty.
        /// </summary>
        public bool IsEmpty
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            get { return width == 0 && height == 0 && x == 0 && y == 0; }
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        #endregion

        #region Fields

        /// <summary>Specifies the x-coordinate of the rectangle.</summary>
        public float x;

        /// <summary>Specifies the y-coordinate of the rectangle.</summary>
        public float y;

        /// <summary>Specifies the width of the rectangle.</summary>
        public float width;

        /// <summary>Specifies the height of the rectangle.</summary>
        public float height;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="RectangleF"/> struct.
        /// </summary>
        /// <param name="x">The x-coordinate of the rectangle.</param>
        /// <param name="y">The y-coordinate of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public RectangleF(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        #endregion

        #region Logic

        /// <summary>
        ///     Determines whether this <see cref="RectangleF"/> entirely contains a specified <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="RectangleF"/> to evaluate.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the rectangle contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(RectangleF value)
        {
            return x <= value.x &&
                   value.x + value.width <= x + width &&
                   y <= value.y &&
                   value.y + value.height <= y + height;
        }

        /// <summary>
        ///     Determines whether this <see cref="RectangleF"/> contains a specified Vector2.
        /// </summary>
        /// <param name="value">The Vector2 to evaluate.</param>
        /// <returns>
        ///     <c>true</c> if the rectangle contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Vector2 value)
        {
            return x <= value.X &&
                   value.X < x + width &&
                   y <= value.Y &&
                   value.Y < y + height;
        }

        //  by its x- and y-coordinates.
        /// <summary>
        ///     Determines whether this <see cref="RectangleF"/> contains a specified point represented
        /// </summary>
        /// <param name="x">The x-coordinate of the specified point.</param>
        /// <param name="y">The y-coordinate of the specified point.</param>
        /// <returns>
        ///     <c>true</c> if the rectangle contains the specified point; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(float x, float y)
        {
            return this.x <= x &&
                   x < this.x + width &&
                   this.y <= y &&
                   y < this.y + height;
        }

        /// <summary>
        ///     Determines whether this <see cref="RectangleF"/> entirely contains a specified <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="RectangleF"/> to evaluate.
        /// </param>
        /// <param name="result">
        ///     On exit, is true if this <see cref="RectangleF"/> entirely contains the specified
        ///     <see cref="RectangleF"/>, or false if not.
        /// </param>
        public void Contains(ref RectangleF value, out bool result)
        {
            result = x <= value.x &&
                     value.x + value.width <= x + width &&
                     y <= value.y &&
                     value.y + value.height <= y + height;
        }

        /// <summary>
        ///     Determines whether this <see cref="RectangleF"/> contains a specified Vector2.
        /// </summary>
        /// <param name="value">The Vector2 to evaluate.</param>
        /// <param name="result">
        ///     true if the specified Vector2 is contained within this <see cref="RectangleF"/>; false otherwise.
        /// </param>
        public void Contains(ref Vector2 value, out bool result)
        {
            result = x <= value.X &&
                     value.X < x + width &&
                     y <= value.Y &&
                     value.Y < y + height;
        }

        /// <summary>
        ///     Pushes the edges of the <see cref="RectangleF"/> out by the horizontal and vertical values specified.
        /// </summary>
        /// <param name="horizontalAmount">Value to push the sides out by.</param>
        /// <param name="verticalAmount">Value to push the top and bottom out by.</param>
        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            x -= horizontalAmount;
            y -= verticalAmount;
            width += horizontalAmount + horizontalAmount;
            height += verticalAmount + verticalAmount;
        }

        /// <summary>
        ///     Determines whether a specified <see cref="RectangleF"/> intersects with this <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="other">
        ///     The <see cref="RectangleF"/> to evaluate.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the rectangles intersect; otherwise, <c>false</c>.
        /// </returns>
        public bool Intersects(RectangleF other)
        {
            return x < other.x + other.width &&
                   other.x < x + width &&
                   y < other.y + other.height &&
                   other.y < y + height;
        }

        /// <summary>
        ///     Determines whether a specified <see cref="RectangleF"/> intersects with this <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="other">
        ///     The <see cref="RectangleF"/> to evaluate.
        /// </param>
        /// <param name="result">
        ///     true if the specified <see cref="RectangleF"/> intersects with this one; false otherwise.
        /// </param>
        public void Intersects(RectangleF other, out bool result)
        {
            result = x < other.x + other.width &&
                     other.x < x + width &&
                     y < other.y + other.height &&
                     other.y < y + height;
        }

        /// <summary>
        ///     Changes the position of the <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="amount">
        ///     The values to adjust the position of the <see cref="RectangleF"/> by.
        /// </param>
        public void Offset(Vector2 amount)
        {
            x += amount.X;
            y += amount.Y;
        }

        /// <summary>
        ///     Changes the position of the <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="offsetX">Change in the x-position.</param>
        /// <param name="offsetY">Change in the y-position.</param>
        public void Offset(float offsetX, float offsetY)
        {
            x += offsetX;
            y += offsetY;
        }

        /// <summary>
        ///     Creates a <see cref="RectangleF"/> defining the area where one rectangle overlaps with another rectangle.
        /// </summary>
        /// <param name="value1">
        ///     The first <see cref="RectangleF"/> to compare.
        /// </param>
        /// <param name="value2">
        ///     The second <see cref="RectangleF"/> to compare.
        /// </param>
        /// <returns></returns>
        public static RectangleF Intersect(RectangleF value1, RectangleF value2)
        {
            var right1 = value1.x + value1.width;
            var right2 = value2.x + value2.width;
            var bottom1 = value1.y + value1.height;
            var bottom2 = value2.y + value2.height;
            var left = value1.x > value2.x ? value1.x : value2.x;
            var top = value1.y > value2.y ? value1.y : value2.y;
            var right = right1 < right2 ? right1 : right2;
            var bottom = bottom1 < bottom2 ? bottom1 : bottom2;

            RectangleF result;
            if (right > left && bottom > top) {
                result.x = left;
                result.y = top;
                result.width = right - left;
                result.height = bottom - top;
            } else {
                result.x = 0;
                result.y = 0;
                result.width = 0;
                result.height = 0;
            }
            return result;
        }

        /// <summary>
        ///     Creates a <see cref="RectangleF"/> defining the area where one rectangle overlaps with another rectangle.
        /// </summary>
        /// <param name="value1">
        ///     The first <see cref="RectangleF"/> to compare.
        /// </param>
        /// <param name="value2">
        ///     The second <see cref="RectangleF"/> to compare.
        /// </param>
        /// <param name="result">The area where the two first parameters overlap.</param>
        public static void Intersect(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            var right1 = value1.x + value1.width;
            var right2 = value2.x + value2.width;
            var bottom1 = value1.y + value1.height;
            var bottom2 = value2.y + value2.height;
            var left = value1.x > value2.x ? value1.x : value2.x;
            var top = value1.y > value2.y ? value1.y : value2.y;
            var right = right1 < right2 ? right1 : right2;
            var bottom = bottom1 < bottom2 ? bottom1 : bottom2;
            if (right > left && bottom > top) {
                result.x = left;
                result.y = top;
                result.width = right - left;
                result.height = bottom - top;
            } else {
                result.x = 0;
                result.y = 0;
                result.width = 0;
                result.height = 0;
            }
        }

        /// <summary>
        ///     Creates a new <see cref="RectangleF"/> that exactly contains two other rectangles.
        /// </summary>
        /// <param name="value1">
        ///     The first <see cref="RectangleF"/> to contain.
        /// </param>
        /// <param name="value2">
        ///     The second <see cref="RectangleF"/> to contain.
        /// </param>
        /// <returns>
        ///     The <see cref="RectangleF"/> that must be the union of the first two rectangles.
        /// </returns>
        public static RectangleF Union(RectangleF value1, RectangleF value2)
        {
            var right1 = value1.x + value1.width;
            var right2 = value2.x + value2.width;
            var bottom1 = value1.y + value1.height;
            var bottom2 = value2.y + value2.height;
            var left = value1.x < value2.x ? value1.x : value2.x;
            var top = value1.y < value2.y ? value1.y : value2.y;
            var right = right1 > right2 ? right1 : right2;
            var bottom = bottom1 > bottom2 ? bottom1 : bottom2;

            RectangleF result;
            result.x = left;
            result.y = top;
            result.width = right - left;
            result.height = bottom - top;
            return result;
        }

        /// <summary>
        ///     Creates a new <see cref="RectangleF"/> that exactly contains two other rectangles.
        /// </summary>
        /// <param name="value1">
        ///     The first <see cref="RectangleF"/> to contain.
        /// </param>
        /// <param name="value2">
        ///     The second <see cref="RectangleF"/> to contain.
        /// </param>
        /// <param name="result">
        ///     The <see cref="RectangleF"/> that must be the union of the first two rectangles.
        /// </param>
        public static void Union(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
        {
            var right1 = value1.x + value1.width;
            var right2 = value2.x + value2.width;
            var bottom1 = value1.y + value1.height;
            var bottom2 = value2.y + value2.height;
            var left = value1.x < value2.x ? value1.x : value2.x;
            var top = value1.y < value2.y ? value1.y : value2.y;
            var right = right1 > right2 ? right1 : right2;
            var bottom = bottom1 > bottom2 ? bottom1 : bottom2;

            result.x = left;
            result.y = top;
            result.width = right - left;
            result.height = bottom - top;
        }

        #endregion

        #region Operators

        /// <summary>Compares two rectangles for equality.</summary>
        /// <param name="a">Source rectangle.</param>
        /// <param name="b">Source rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the rectangles are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(RectangleF a, RectangleF b)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return a.x == b.x && a.y == b.y && a.width == b.width && a.height == b.height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>Compares two rectangles for inequality.</summary>
        /// <param name="a">Source rectangle.</param>
        /// <param name="b">Source rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the rectangles are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(RectangleF a, RectangleF b)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return a.x != b.x || a.y != b.y || a.width != b.width || a.height != b.height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public static RectangleF operator +(RectangleF r, Vector2 v)
        {
            RectangleF ans;
            ans.x = r.x + v.X;
            ans.y = r.y + v.Y;
            ans.width = r.width;
            ans.height = r.height;
            return ans;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Microsoft.Xna.Framework.Rectangle"/> to
        ///     <see cref="Engine.Math.RectangleF"/>.
        /// </summary>
        /// <param name="rectangle">The rectangle to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator RectangleF(Rectangle rectangle)
        {
            RectangleF r;
            r.x = rectangle.X;
            r.y = rectangle.Y;
            r.width = rectangle.Width;
            r.height = rectangle.Height;
            return r;
        }

        public static explicit operator Rectangle(RectangleF rectangle)
        {
            return new Rectangle((int)rectangle.x, (int)rectangle.y, (int)rectangle.width, (int)rectangle.height);
        }

        #endregion

        #region Equality Overrides

        /// <summary>Compares two rectangles for equality.</summary>
        /// <param name="other">The other rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the rectangles are equal; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(RectangleF other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return x == other.x &&
                   y == other.y &&
                   width == other.width &&
                   height == other.height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>Compares two rectangles for equality.</summary>
        /// <param name="obj">
        ///     The <see cref="System.Object"/> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified value is a <see cref="RectangleF"/> and the rectangles are equal; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Rectangle && Equals((Rectangle)obj);
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode XNA's Rectangle does the same thing
            return x.GetHashCode() + y.GetHashCode() + width.GetHashCode() + height.GetHashCode();
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }

        #endregion

        #region ToString

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "{{X:{0} Y:{1} Width:{2} Height:{3}}}",
                x.ToString(CultureInfo.CurrentCulture),
                y.ToString(CultureInfo.CurrentCulture),
                width.ToString(CultureInfo.CurrentCulture),
                height.ToString(CultureInfo.CurrentCulture));
        }

        #endregion
    }
}