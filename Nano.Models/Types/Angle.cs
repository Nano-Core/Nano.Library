using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Types
{
    /// <summary>
    /// Angle.
    /// </summary>
    public class Angle
    {
        private decimal radians;
        private decimal degrees;

        /// <summary>
        /// The <see cref="Angle"/> represented in radians.
        /// </summary>
        [Required]
        [DefaultValue(0.00)]
        public virtual decimal Radians
        {
            get => radians;
            set
            {
                this.radians = value;
                this.degrees = value * (180.00M / (decimal)Math.PI);
            }
        }

        /// <summary>
        /// The <see cref="Angle"/> represented in radians, between 0 and 2 Pi. 
        /// </summary>
        public virtual decimal Radians2Pi
        {
            get
            {
                const decimal PI = (decimal)Math.PI;

                if (this.radians < 0)
                    return 2 * PI - Math.Abs(this.radians) % (2 * PI);

                if (this.radians > 2 * PI)
                    return this.radians % (2 * PI);

                return this.radians;
            }
        }

        /// <summary>
        /// The <see cref="Angle"/> represented in degrees.
        /// </summary>
        [Required]
        [DefaultValue(0.00)]
        public virtual decimal Degrees
        {
            get => this.degrees;
            set
            {
                this.degrees = value;
                this.radians = (decimal)Math.PI / 180.00M * value;
            }
        }

        /// <summary>
        /// The <see cref="Angle"/> represented in degrees, between 0 and 360. 
        /// </summary>
        public virtual decimal Degrees360
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
        /// Compare the instance for equality with the passed <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The <see cref="Angle"/> to compare for equality with the instance.</param>
        /// <returns>A <see cref="Boolean"/> indicating whether the instance was equal to <paramref name="other"/>.</returns>
        public virtual bool Equals(Angle other)
        {
            return radians == other.radians && degrees == other.degrees;
        }

        /// <summary>
        /// Override of <see cref="System.Object.Equals(object)"/> for use with operator overlods.
        /// </summary>
        /// <param name="object">The <see cref="System.Object"/> to compare for equality with the instance.</param>
        /// <returns>A <see cref="Boolean"/> indicating whether the instance was equal to <paramref name="object"/>.</returns>
        public override bool Equals(object @object)
        {
            return @object is Angle && this.Equals((Angle)@object);
        }

        /// <summary>
        /// Override of <see cref="Object.GetHashCode()"/> to compare the <see cref="Angle.Radians"/> and not the instance itself.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return this.radians.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
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
        public static Angle operator *(Angle lhs, decimal rhs)
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
        public static Angle operator *(decimal lhs, Angle rhs)
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
        public static Angle operator /(Angle lhs, decimal rhs)
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
        public static Angle operator /(decimal lhs, Angle rhs)
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
        /// Checking if the <paramref name="lhs"/> is not equal to the <paramref name="lhs"/>.
        /// </summary>
        /// <param name="lhs">Instance to devide from.</param>
        /// <param name="rhs">Value to divide from the instance.</param>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="Angle.Radians"/> of the <paramref name="lhs"/> is not equal to the <paramref name="lhs"/>.</returns>
        public static bool operator !=(Angle lhs, Angle rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return lhs.radians != rhs.radians;
        }

        /// <summary>
        /// Checking if the <paramref name="lhs"/> is equal to the <paramref name="lhs"/>.
        /// </summary>
        /// <param name="lhs">Instance to devide from.</param>
        /// <param name="rhs">Value to divide from the instance.</param>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="Angle.Radians"/> of the <paramref name="lhs"/> is equal to the <paramref name="lhs"/>.</returns>
        public static bool operator ==(Angle lhs, Angle rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return lhs.radians == rhs.radians;
        }

        /// <summary>
        /// Double operator override.
        /// </summary>
        /// <param name="angle">The <see cref="Angle"/>.</param>
        public static implicit operator double(Angle angle)
        {
            if (angle == null)
                throw new ArgumentNullException(nameof(angle));

            return (double)angle.Radians;
        }

        /// <summary>
        /// Double operator override.
        /// </summary>
        /// <param name="angle">The <see cref="Angle"/>.</param>
        public static implicit operator decimal(Angle angle)
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