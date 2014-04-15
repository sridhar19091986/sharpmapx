using System;

namespace Portable.IO
{
    public class ApplicationException: Exception
    {
        public ApplicationException(string message)
            : base(message)
        {
            
        }

        public ApplicationException()
            :base()
        {
            
        }

        public ApplicationException(string message, Exception innerException)
            :base(message, innerException)
        {
            
        }
    }
}
