using System;
using DotNetCqs;

namespace codeRR.Server.Api.Modules.Triggers.Queries
{
    /// <summary>
    ///     Get a configured trigger
    /// </summary>
    [Message]
    public class GetTrigger : Query<GetTriggerDTO>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetTrigger" />.
        /// </summary>
        /// <param name="id">trigger id</param>
        /// <exception cref="ArgumentOutOfRangeException">id</exception>
        public GetTrigger(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException("id");
            Id = id;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetTrigger()
        {
        }

        /// <summary>
        ///     Triggger id
        /// </summary>
        public int Id { get; set; }
    }
}