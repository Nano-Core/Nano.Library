namespace Nano.Web.Hosting.Enums
{
    /// <summary>
    /// X-Xss Protection Policy BlockMode.
    /// </summary>
    public enum XXssProtectionPolicyBlockMode
    {
        /// <summary>
        /// Disabled.
        /// Specifies that the X-Xss-Protection header should not be set in the HTTP response.
        /// </summary>
        Disabled,

        /// <summary>
        /// Filter Disabled.
        /// Specifies that the X-Xss-Protection header should be set in the HTTP response, explicitly disabling the IE XSS filter.
        /// </summary>
        FilterDisabled,

        /// <summary>
        /// Filter Enabled.
        /// Specifies that the X-Xss-Protection header should be set in the HTTP response, explicitly enabling the IE XSS filter.
        /// </summary>
        FilterEnabled,

        /// <summary>
        /// Filter Enabled Block Mode.
        /// Specifies that the X-Xss-Protection header should be set in the HTTP response, explicitly enabling the IE XSS filter.
        /// BlockMode is set to true.
        /// </summary>
        FilterEnabledBlockMode
    }
}