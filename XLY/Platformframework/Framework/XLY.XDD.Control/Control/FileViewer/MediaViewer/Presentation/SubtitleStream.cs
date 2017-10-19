using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace XLY.XDD.Control
{
    //****************************************************************************
    /// <summary>
    ///   Represents a subtitle stream of a media loaded within a media element.
    /// </summary>
    /// <seealso cref="MediaElement"/>
    public sealed class SubtitleStream
      : MediaStream
    {

        //==========================================================================
        internal SubtitleStream(SubtitleTrack subtitleTrack)
            : base(subtitleTrack)
        {
            // ...
        }

        #region Properties

        #region SubtitleTrack

        //==========================================================================                
        internal SubtitleTrack SubtitleTrack
        {
            get
            {
                return Track as SubtitleTrack;
            }
        }

        #endregion // SubtitleTrack

        #endregion // Properties

    } // class SubtitleStream
}
