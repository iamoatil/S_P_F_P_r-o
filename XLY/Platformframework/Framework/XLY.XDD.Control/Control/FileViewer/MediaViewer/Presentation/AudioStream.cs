using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    //****************************************************************************
    /// <summary>
    ///   Represents an audio stream of a media loaded within a media element.
    /// </summary>
    /// <seealso cref="MediaElement"/>
    public sealed class AudioStream
      : MediaStream
    {

        //==========================================================================
        internal AudioStream(AudioTrack audioTrack)
            : base(audioTrack)
        {
            // ...
        }

        #region Properties

        #region AudioTrack

        //==========================================================================                
        internal AudioTrack AudioTrack
        {
            get
            {
                return Track as AudioTrack;
            }
        }

        #endregion // AudioTrack

        #region Channels

        //==========================================================================                
        /// <summary>
        ///   Gets the number of channels of the audio track.
        /// </summary>
        public int Channels
        {
            get
            {
                return AudioTrack.Channels;
            }
        }

        #endregion // Channels

        #region BitRate

        //==========================================================================                
        /// <summary>
        ///   Gets bit rate of the audio track.
        /// </summary>
        public int BitRate
        {
            get
            {
                return AudioTrack.BitRate;
            }
        }

        #endregion // BitRate

        #endregion // Properties



    } // class AudioStream
}
