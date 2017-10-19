using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace XLY.XDD.Control
{
    //****************************************************************************
    partial class LibVLCLibrary
    {

        // const char * libvlc_errmsg (void)

        //==========================================================================
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate string libvlc_errmsg_signature();

        //==========================================================================
        private readonly libvlc_errmsg_signature m_libvlc_errmsg;

        //==========================================================================
        public string libvlc_errmsg()
        {
            VerifyAccess();

            string result = m_libvlc_errmsg();
            return result;
        }

        // void libvlc_clearerr (void)

        //==========================================================================
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate string libvlc_clearerr_signature();

        //==========================================================================
        private readonly libvlc_clearerr_signature m_libvlc_clearerr;

        //==========================================================================
        public void libvlc_clearerr()
        {
            VerifyAccess();

            m_libvlc_clearerr();
        }

    } // class LibVLCLibrary
}
