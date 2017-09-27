﻿using System;
using DotNetCqs;

namespace codeRR.Api.Core.ApiKeys.Commands
{
    /// <summary>
    ///     Create a new api key
    /// </summary>
    [AuthorizeRoles("SysAdmin")]
    public class EditApiKey : Command
    {
        /// <summary>
        ///     Creates a new instance of <see cref="EditApiKey" />.
        /// </summary>
        public EditApiKey(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            Id = id;
        }


        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected EditApiKey()
        {
        }


        /// <summary>
        ///     applications that this key may modify. Empty = allow for all applications.
        /// </summary>
        public int[] ApplicationIds { get; set; }

        /// <summary>
        ///     Application that uses this api key
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        ///     Key id
        /// </summary>
        public int Id { get; }
    }
}