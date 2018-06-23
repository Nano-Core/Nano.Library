using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Nano.Models.Types
{
    /// <summary>
    /// Email Address.
    /// </summary>
    public class EmailAddress
    {
        private const string VALIDATE_PATTERN = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

        /// <summary>
        /// Email.
        /// </summary>
        [EmailAddress]
        [MaxLength(256)]
        public virtual string Email { get; set; }

        /// <summary>
        /// Is Valid.
        /// </summary>
        public virtual bool IsValid
        {
            get => this.Email != null && Regex.IsMatch(this.Email, EmailAddress.VALIDATE_PATTERN);
            protected set { }
        }
    }
}