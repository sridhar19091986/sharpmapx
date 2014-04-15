using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public sealed class DBNull
    {
        public static readonly DBNull Value = new DBNull();
        private DBNull() { }
    }
}