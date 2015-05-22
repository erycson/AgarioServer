using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgarioServer.Game.Exceptions
{
    public class RealmFullException : Exception
    {
        internal RealmFullException()
            : base("The realm reached its maximum capacity")
        {

        }
    }
}
