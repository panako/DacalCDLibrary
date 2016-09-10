using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DacalCDLibrary
{
    /// <summary>
    /// Exception will be thrown in case of problems with CD Library
    /// </summary>
    [Serializable]
    public class CDLibraryException : Exception
    {
        /// <summary>
        /// Exception will be thrown in case of problems with CD Library.
        /// </summary>
        public CDLibraryException() { }
        /// <summary>
        /// Exception will be thrown in case of problems with CD Library.
        /// </summary>
        /// <param name="message">information about problem</param>
        public CDLibraryException(string message) : base(message) { }
        /// <summary>
        /// Exception will be thrown in case of problems with CD Library.
        /// Some kind of wrapper. It contains other exception that was throw during execution.
        /// </summary>
        /// <param name="message">information about problem</param>
        /// <param name="inner">other exception that was cought by this DLL</param>
        public CDLibraryException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Exception will be thrown in case of problems with CD Library.
        /// </summary>
        /// <param name="info">information about problem</param>
        /// <param name="context"></param>
        protected CDLibraryException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
