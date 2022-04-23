﻿using Nano.Security.Models;

namespace Nano.Web.Api.Requests.Auth
{
    /// <inheritdoc />
    public class LogInRefreshRequest : BaseRequestPost
    {
        /// <summary>
        /// Login.
        /// </summary>
        public virtual LogInRefresh LogInRefresh { get; set; } = new();

        /// <inheritdoc />
        public LogInRefreshRequest()
        {
            this.Action = "login/refresh";
            this.Controller = "auth";
        }

        /// <inheritdoc />
        public override object GetBody()
        {
            return this.LogInRefresh;
        }
    }
}