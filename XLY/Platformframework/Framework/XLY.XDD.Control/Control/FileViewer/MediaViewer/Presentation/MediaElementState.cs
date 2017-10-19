using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    //****************************************************************************
    /// <summary>
    ///   Represents the state of a <see cref="MediaElement"/>.
    /// </summary>
    public enum MediaElementState
    {

        //==========================================================================
        /// <summary>
        ///   No media has been loaded.
        /// </summary>
        Empty,

        //==========================================================================
        /// <summary>
        ///   The media element is currently opening a media.
        /// </summary>
        Opening,

        //==========================================================================
        /// <summary>
        ///   The media element is currently playing a media.
        /// </summary>
        Playing,

        //==========================================================================
        /// <summary>
        ///   Playback is currently paused.
        /// </summary>
        Paused,

        //==========================================================================
        /// <summary>
        ///   The end of the media has been reached.
        /// </summary>
        EndReached,

        //==========================================================================
        /// <summary>
        ///   The media has been stopped.
        /// </summary>
        Stopped,

        //==========================================================================
        /// <summary>
        ///   There has been an error opening or playing the media.
        /// </summary>
        EncounteredError,

    } // enum MediaElementState
}
