using System;

namespace Nano.Models.Types
{
    /// <summary>
    /// Percentage.
    /// </summary>
    public class Percentage
    {
        /// <summary>
        /// Gets the percentage value as an <see cref="System.Int32"/>.
        /// </summary>
        public int AsInteger => (int)(this.AsDecimal * 100.00M);

        /// <summary>
        /// Gets the percentage value as an <see cref="decimal"/>.
        /// </summary>
        public decimal AsDecimal { get; }

        /// <summary>
        /// Constructor that accepts an <see cref="Int32"/>.
        /// </summary>
        /// <param name="int"></param>
        public Percentage(int  @int)
            : this((decimal)@int / 100)
        {

        }

        /// <summary>
        /// Constructor that accepts an <see cref="Double"/>.
        /// </summary>
        /// <param name="double"></param>
        public Percentage(double @double)
            : this((decimal)@double)
        {

        }

        /// <summary>
        /// Constructor that accepts an <see cref="Decimal"/>.
        /// </summary>
        /// <param name="decimal"></param>
        public Percentage(decimal @decimal)
        {
            this.AsDecimal = @decimal;
        }

        /// <summary>
        /// Compare the instance for equality with the passed <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The <see cref="Percentage"/> to compare for equality with the instance.</param>
        /// <returns>A <see cref="Boolean"/> indicating whether the instance was equal to <paramref name="other"/>.</returns>
        public bool Equals(Percentage other)
        {
            return this.AsDecimal == other.AsDecimal;
        }

        // Overridden overloads.
        /// <summary>
        /// Override of <see cref="System.Object.Equals(object)"/> for use with operator overlods.
        /// </summary>
        /// <param name="object">The <see cref="System.Object"/> to compare for equality with the instance.</param>
        /// <returns>A <see cref="Boolean"/> indicating whether the instance was equal to <paramref name="object"/>.</returns>
        public override bool Equals(object @object)
        {
            if (@object == null)
                return false;

            return @object is Percentage && this.Equals(@object);
        }

        /// <summary>
        /// Override of <see cref="Object.GetHashCode()"/> to compare the <see cref="Percentage.AsDecimal"/> and not the instance itself.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return AsDecimal.GetHashCode();
        }

        /// <summary>
        /// Gets the <see cref="Percentage"/> value as a <see cref="string"/>.
        /// The <see cref="string"/> is formatted like: $"{<see cref="Percentage.AsInteger"/>}%"   
        /// </summary>
        /// <returns>A <see cref="string"/> formatted as a percentage value, where '%' is appended.</returns>
        public override string ToString()
        {
            return $"{this.AsInteger}%";
        }

        // Operator overloads.
        /// <summary>
        /// Adds the <paramref name="rhs"/> to the <paramref name="lhs"/>.
        /// </summary>
        /// <param name="lhs">Instance to add to.</param>
        /// <param name="rhs">Value to add to the instance.</param>
        /// <returns>A new <see cref="Percentage"/> value of the result.</returns>
        public static Percentage operator +(Percentage lhs, Percentage rhs)
        {
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return new Percentage(lhs.AsDecimal + rhs.AsDecimal);
        }

        /// <summary>
        /// Subtracts the <paramref name="rhs"/> from the <paramref name="lhs"/>.
        /// </summary>
        /// <param name="lhs">Instance to subtract from.</param>
        /// <param name="rhs">Value to subtract from the instance.</param>
        /// <returns>A new <see cref="Percentage"/> value of the result.</returns>
        public static Percentage operator -(Percentage lhs, Percentage rhs)
        {
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return new Percentage(lhs.AsDecimal - rhs.AsDecimal);
        }

        /// <summary>
        /// Multiplies the <paramref name="rhs"/> with the <paramref name="lhs"/>.
        /// </summary>
        /// <param name="lhs">Instance to multiply with.</param>
        /// <param name="rhs">Value to multiply with the instance.</param>
        /// <returns>A new <see cref="Percentage"/> value of the result.</returns>
        public static Percentage operator *(Percentage lhs, Percentage rhs)
        {
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return new Percentage(lhs.AsDecimal * rhs.AsDecimal);
        }

        /// <summary>
        /// Divides the <paramref name="rhs"/> from the <paramref name="lhs"/>.
        /// </summary>
        /// <param name="lhs">Instance to devide from.</param>
        /// <param name="rhs">Value to divide from the instance.</param>
        /// <returns>A new <see cref="Percentage"/> value of the result.</returns>
        public static Percentage operator /(Percentage lhs, Percentage rhs)
        {
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return new Percentage(lhs.AsDecimal / rhs.AsDecimal);
        }

        /// <summary>
        /// Checking if the <paramref name="lhs"/> is greater than <paramref name="lhs"/>.
        /// </summary>
        /// <param name="lhs">Instance to devide from.</param>
        /// <param name="rhs">Value to divide from the instance.</param>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="Percentage.AsDecimal"/> of the <paramref name="lhs"/> is greater than <paramref name="lhs"/>.</returns>
        public static bool operator >(Percentage lhs, Percentage rhs)
        {
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return lhs.AsDecimal > rhs.AsDecimal;
        }

        /// <summary>
        /// Checking if the <paramref name="lhs"/> is greater or equal than <paramref name="lhs"/>.
        /// </summary>
        /// <param name="lhs">Instance to devide from.</param>
        /// <param name="rhs">Value to divide from the instance.</param>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="Percentage.AsDecimal"/> of the <paramref name="lhs"/> is greater or equal than <paramref name="lhs"/>.</returns>
        public static bool operator >=(Percentage lhs, Percentage rhs)
        {
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return lhs.AsDecimal >= rhs.AsDecimal;
        }

        /// <summary>
        /// Checking if the <paramref name="lhs"/> is less than <paramref name="lhs"/>.
        /// </summary>
        /// <param name="lhs">Instance to devide from.</param>
        /// <param name="rhs">Value to divide from the instance.</param>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="Percentage.AsDecimal"/> of the <paramref name="lhs"/> is less than <paramref name="lhs"/>.</returns>
        public static bool operator <(Percentage lhs, Percentage rhs)
        {
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return lhs.AsDecimal < rhs.AsDecimal;
        }

        /// <summary>
        /// Checking if the <paramref name="lhs"/> is less or equal than <paramref name="lhs"/>.
        /// </summary>
        /// <param name="lhs">Instance to devide from.</param>
        /// <param name="rhs">Value to divide from the instance.</param>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="Percentage.AsDecimal"/> of the <paramref name="lhs"/> is  less or equal than <paramref name="lhs"/>.</returns>
        public static bool operator <=(Percentage lhs, Percentage rhs)
        {
            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return lhs.AsDecimal <= rhs.AsDecimal;
        }

        /// <summary>
        /// Checking if the <paramref name="lhs"/> is not equal to the <paramref name="lhs"/>.
        /// </summary>
        /// <param name="lhs">Instance to devide from.</param>
        /// <param name="rhs">Value to divide from the instance.</param>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="Percentage.AsDecimal"/> of the <paramref name="lhs"/> is not equal to the <paramref name="lhs"/>.</returns>
        public static bool operator !=(Percentage lhs, Percentage rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return lhs.AsDecimal != rhs.AsDecimal;
        }

        /// <summary>
        /// Checking if the <paramref name="lhs"/> is equal to the <paramref name="lhs"/>.
        /// </summary>
        /// <param name="lhs">Instance to devide from.</param>
        /// <param name="rhs">Value to divide from the instance.</param>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="Percentage.AsDecimal"/> of the <paramref name="lhs"/> is equal to the <paramref name="lhs"/>.</returns>
        public static bool operator ==(Percentage lhs, Percentage rhs)
        {
            if (lhs == null)
                throw new ArgumentNullException(nameof(lhs));

            if (rhs == null)
                throw new ArgumentNullException(nameof(rhs));

            return lhs.AsDecimal == rhs.AsDecimal;
        }
    }
}