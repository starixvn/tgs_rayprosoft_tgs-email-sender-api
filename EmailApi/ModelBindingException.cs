using System;

namespace TGS.EmailApi
{
    public sealed class ModelBindingException : Exception
    {
        public ModelBindingException(string message) : base(message)
        {
        }
    }
}
