using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.App.Core.Environments
{
    /// <summary>
    /// Keeps track of all named environments in a system.
    /// </summary>
    public class Environment
    {
        public Environment(string name)
        {
            Name = name;
        }

        protected Environment()
        {

        }

        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name as used when reporting errors.
        /// </summary>
        public string Name { get; private set; }
    }
}
