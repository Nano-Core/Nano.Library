﻿using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogInExternalGoogleRequest : BaseLogInExternalRequest<LoginExternalGoogle>
    {
        /// <inheritdoc />
        public LogInExternalGoogleRequest()
        {
            this.Action = "login/external/google";
            this.Controller = "auth";
        }
    }

}