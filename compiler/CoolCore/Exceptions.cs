using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolCore
{
    public class SymbolException : Exception
    {
        public SymbolException(string message) : base(message) { }
    }

    public class VerifierException : Exception
    {
        public VerifierException(string message) : base(message) { }
    }

}