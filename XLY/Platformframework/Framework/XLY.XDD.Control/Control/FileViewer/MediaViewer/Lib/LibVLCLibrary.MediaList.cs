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

        //==========================================================================
        // LIBVLC_API void libvlc_media_list_release(libvlc_media_list_t*	p_ml)

        //==========================================================================
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_list_release_signature(IntPtr p_ml);

        //==========================================================================
        private readonly libvlc_media_list_release_signature m_libvlc_media_list_release;

        //==========================================================================
        public void libvlc_media_list_release(IntPtr p_ml)
        {
            VerifyAccess();

            m_libvlc_media_list_release(p_ml);
        }


        //==========================================================================
        // LIBVLC_API void 	libvlc_media_list_lock (libvlc_media_list_t *p_ml)

        //==========================================================================
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_list_lock_signature(IntPtr p_ml);

        //==========================================================================
        private readonly libvlc_media_list_lock_signature m_libvlc_media_list_lock;

        //==========================================================================
        public void libvlc_media_list_lock(IntPtr p_ml)
        {
            VerifyAccess();

            m_libvlc_media_list_lock(p_ml);
        }


        //==========================================================================
        // LIBVLC_API void libvlc_media_list_unlock	(	libvlc_media_list_t * 	p_ml	 ) 	

        //==========================================================================
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_list_unlock_signature(IntPtr p_ml);

        //==========================================================================
        private readonly libvlc_media_list_unlock_signature m_libvlc_media_list_unlock;

        //==========================================================================
        public void libvlc_media_list_unlock(IntPtr p_ml)
        {
            VerifyAccess();

            m_libvlc_media_list_unlock(p_ml);
        }


        //==========================================================================
        // LIBVLC_API int libvlc_media_list_count	(	libvlc_media_list_t * 	p_ml	 ) 	

        //==========================================================================
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_media_list_count_signature(IntPtr p_ml);

        //==========================================================================
        private readonly libvlc_media_list_count_signature m_libvlc_media_list_count;

        //==========================================================================
        public int libvlc_media_list_count(IntPtr p_ml)
        {
            VerifyAccess();

            return m_libvlc_media_list_count(p_ml);
        }


        //==========================================================================
        // LIBVLC_API libvlc_media_t* libvlc_media_list_item_at_index(libvlc_media_list_t * p_ml, int i_pos)	

        //==========================================================================
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr libvlc_media_list_item_at_index_signature(IntPtr p_ml, int i_pos);

        //==========================================================================
        private readonly libvlc_media_list_item_at_index_signature m_libvlc_media_list_item_at_index;

        //==========================================================================
        public IntPtr libvlc_media_list_item_at_index(IntPtr p_ml, int i_pos)
        {
            VerifyAccess();

            return m_libvlc_media_list_item_at_index(p_ml, i_pos);
        }

    } // class LibVLCLibrary
}
