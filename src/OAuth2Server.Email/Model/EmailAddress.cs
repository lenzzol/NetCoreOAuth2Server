namespace OAuth2Server.Email.Model
{
    /// ===============================================================================================================
    /// <struct>
    /// 	 	 	<see cref="EmailAddress"/>
    /// </struct>
    /// <summary>
    ///             Holds information about the person sending or receiving an email.
    /// </summary>
    /// ===============================================================================================================
    public struct EmailAddress
    {
        /// -------------------------------------
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// -------------------------------------
        public string Name { get; set; }

        /// -------------------------------------
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// -------------------------------------
        public string Email { get; set; }
    }
}