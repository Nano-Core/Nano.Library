using System;

namespace Nano.Models.Types
{
    /// <summary>
    /// Angle.
    /// </summary>
    public class Angle
    {
        private double radians;
        private double degrees;

        /// <summary>
        /// The <see cref="Angle"/> represented in radians.
        /// </summary>
        public virtual double Radians
        {
            get => radians;
            set
            {
                this.radians = value;
                this.degrees = value * (180.00D / Math.PI);
            }
        }

        /// <summary>
        /// The <see cref="Angle"/> represented in radians, between 0 and 2 Pi. 
        /// </summary>
        public virtual double Radians2Pi
        {
            get
            {
                if (this.radians < 0)
                    return 2 * Math.PI - Math.Abs(this.radians) % (2 * Math.PI);

                if (this.radians > 2 * Math.PI)
                    return this.radians % (2 * Math.PI);

                return this.radians;
            }
        }

        /// <summary>
        /// The <see cref="Angle"/> represented in degrees.
        /// </summary>
        public virtual double Degrees
        {
            get => this.degrees;
            set
            {
                this.degrees = value;
                this.radians = Math.PI / 180.00D * value;
            }
        }

        /// <summary>
        /// The <see cref="Angle"/> represented in degrees, between 0 and 360. 
        /// </summary>
        public virtual double Degrees360
        {
            get
            {
                if (this.degrees < 0)
                    return 360 - Math.Abs(this.degrees) % 360;

                if (this.degrees > 360)
                    return this.degrees % 360;

                return this.degrees;
            }
        }

        /// <summary>
        /// Overridden ToString.
        /// </summary>
        /// <returns>Returns instance formatted as radians.</returns>
        public override string ToString()
        {
            return $"{this.Radians } radians";
        }

        /// <summary>
        /// Addition operator overload.
        /// </summary>
        /// <param name="lhs">Left-hand-side.</param>
        /// <param name="rhs">Right-hand-side</param>
        /// <returns>A new instance of <see cref="Angle"/>.</returns>
        public static Angle operator +(Angle lhs, Angle rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            return rhs == null 
                ? null 
                : new Angle { Radians = lhs.Radians + rhs.Radians };
        }

        /// <summary>
        /// Substraction operator overload.
        /// </summary>
        /// <param name="lhs">Left-hand-side.</param>
        /// <param name="rhs">Right-hand-side</param>
        /// <returns>A new instance of <see cref="Angle"/>.</returns>
        public static Angle operator -(Angle lhs, Angle rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            return rhs == null
                ? null
                : new Angle { Radians = lhs.Radians - rhs.radians };
        }

        /// <summary>
        /// Multiplication operator overload.
        /// </summary>
        /// <param name="lhs">Left-hand-side.</param>
        /// <param name="rhs">Right-hand-side</param>
        /// <returns>A new instance of <see cref="Angle"/>.</returns>
        public static Angle operator *(Angle lhs, Angle rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            return rhs == null
                ? null
                : new Angle { Radians = lhs.Radians * rhs.radians };
        }

        /// <summary>
        /// Multiplication operator overload.
        /// </summary>
        /// <param name="lhs">Left-hand-side.</param>
        /// <param name="rhs">Right-hand-side</param>
        /// <returns>A new instance of <see cref="Angle"/>.</returns>
        public static Angle operator *(Angle lhs, double rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            return new Angle { Radians = lhs.Radians * rhs };
        }

        /// <summary>
        /// Multiplication operator overload.
        /// </summary>
        /// <param name="lhs">Left-hand-side.</param>
        /// <param name="rhs">Right-hand-side</param>
        /// <returns>A new instance of <see cref="Angle"/>.</returns>
        public static Angle operator *(double lhs, Angle rhs)
        {
            return rhs == null
                ? null 
                : new Angle { Radians = lhs * rhs.Radians };
        }

        /// <summary>
        /// Division operator overload.
        /// </summary>
        /// <param name="lhs">Left-hand-side.</param>
        /// <param name="rhs">Right-hand-side</param>
        /// <returns>A new instance of <see cref="Angle"/>.</returns>
        public static Angle operator /(Angle lhs, Angle rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            return rhs == null
                ? null
                : new Angle { Radians = lhs.Radians / rhs.Radians };
        }

        /// <summary>
        /// Division operator overload.
        /// </summary>
        /// <param name="lhs">Left-hand-side.</param>
        /// <param name="rhs">Right-hand-side</param>
        /// <returns>A new instance of <see cref="Angle"/>.</returns>
        public static Angle operator /(Angle lhs, double rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            return new Angle { radians = lhs.Radians / rhs };
        }

        /// <summary>
        /// Division operator overload.
        /// </summary>
        /// <param name="lhs">Left-hand-side.</param>
        /// <param name="rhs">Right-hand-side</param>
        /// <returns>A new instance of <see cref="Angle"/>.</returns>
        public static Angle operator /(double lhs, Angle rhs)
        {
            return rhs == null
                ? null
                : new Angle { radians = lhs / rhs.Radians };
        }

        /// <summary>
        /// Less than operator override.
        /// </summary>
        /// <param name="lhs">Left-hand-side.</param>
        /// <param name="rhs">Right-hand-side</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator <(Angle lhs, Angle rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            return lhs.Radians < rhs?.Radians;
        }

        /// <summary>
        /// Greater than operator override.
        /// </summary>
        /// <param name="lhs">Left-hand-side.</param>
        /// <param name="rhs">Right-hand-side</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator >(Angle lhs, Angle rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            return lhs.Radians > rhs?.Radians;
        }

        /// <summary>
        /// Less than or equal operator override.
        /// </summary>
        /// <param name="lhs">Left hand side.</param>
        /// <param name="rhs">Right hand side.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator <=(Angle lhs, Angle rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            return lhs.Radians <= rhs?.Radians;
        }

        /// <summary>
        /// Greater than or equal operator override.
        /// </summary>
        /// <param name="lhs">Left hand side.</param>
        /// <param name="rhs">Right hand side.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator >=(Angle lhs, Angle rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            return lhs.Radians >= rhs?.Radians;
        }

        /// <summary>
        /// Double operator override.
        /// </summary>
        /// <param name="angle">The <see cref="Angle"/>.</param>
        public static implicit operator double(Angle angle)
        {
            if (angle == null)
                throw new ArgumentNullException(nameof(angle));

            return angle.Radians;
        }

        /// <summary>
        /// String operator override.
        /// </summary>
        /// <param name="angle">The <see cref="Angle"/>.</param>
        public static implicit operator string(Angle angle)
        {
            if (angle == null)
                throw new ArgumentNullException(nameof(angle));

            return angle.ToString();
        }
    }
}