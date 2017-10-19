using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace XLY.XDD.Control
{
    //****************************************************************************
    /// <summary>
    ///   Represents a video stream of a media loaded within a media element.
    /// </summary>
    /// <seealso cref="MediaElement"/>
    public sealed class VideoStream
      : MediaStream
    {

        //==========================================================================
        internal VideoStream(VideoTrack videoTrack)
            : base(videoTrack)
        {
            // ...
        }

        #region Properties

        #region VideoTrack

        //==========================================================================                
        internal VideoTrack VideoTrack
        {
            get
            {
                return Track as VideoTrack;
            }
        }

        #endregion // VideoTrack


        #region Width

        //==========================================================================                
        /// <summary>
        ///   Gets the width (in pixels) of the video stream.
        /// </summary>
        public int Width
        {
            get
            {
                return VideoTrack.Width;
            }
        }

        #endregion // Width

        #region Height

        //==========================================================================                
        /// <summary>
        ///   Gets the height (in pixels) of the video stream.
        /// </summary>
        public int Height
        {
            get
            {
                return VideoTrack.Height;
            }
        }

        #endregion // Height

        #endregion // Properties


    } // class VideoStream
}
