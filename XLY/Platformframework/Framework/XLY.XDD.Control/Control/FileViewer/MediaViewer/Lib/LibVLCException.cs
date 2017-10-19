using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    //****************************************************************************
    /// <summary>
    ///   Represents an error returned by a native <c>LibVLC</c> function.
    /// </summary>
    public class LibVLCException
      : Exception
    {

        //==========================================================================
        private static string GetErrorMessage(LibVLCLibrary library)
        {
            if (library == null)
                throw new ArgumentNullException("library");

            return library.libvlc_errmsg();
        }

        //==========================================================================
        /// <summary>
        ///   Initializes a new <see cref="LibVLCException"/> with the provided
        ///   error message.
        /// </summary>
        /// <param name="errorMessage">
        ///   The message which will be returned by 
        ///   <see cref="Exception.Message"/>.
        /// </param>
        public LibVLCException(string errorMessage)
            : base(errorMessage)
        {
            // ...
        }

        //==========================================================================
        /// <summary>
        ///   Initializes a new <see cref="LibVLCException"/> with the most recent
        ///   <c>LibVLC</c> error.
        /// </summary>
        /// <param name="library">
        ///   The <see cref="LibVLCLibrary"/> which will be used to obtain the 
        ///   most recent error for the calling thread.
        /// </param>
        /// <remarks>
        ///   <see cref="LibVLCLibrary.libvlc_errmsg"/> is called to get the 
        ///   message to initialize <see cref="Exception.Message"/> with.
        /// </remarks>
        public LibVLCException(LibVLCLibrary library)
            : this(GetErrorMessage(library))
        {
            // ...
        }

    } // class LibVLCException
}
