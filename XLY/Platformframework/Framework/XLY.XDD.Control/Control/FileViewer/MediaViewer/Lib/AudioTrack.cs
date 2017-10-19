using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.XDD.Control
{
    //****************************************************************************
    /// <summary>
    ///   Represents an audio track.
    /// </summary>
    public class AudioTrack
      : Track
    {

        //==========================================================================
        internal AudioTrack(int index, LibVLCLibrary.libvlc_track_description_t trackDescription, LibVLCLibrary.libvlc_media_track_info_t? trackInfo)
            : base(index, trackDescription, trackInfo)
        {
            if (trackInfo.HasValue)
            {
                m_BitRate = (int)trackInfo.Value.audio.i_rate;
                m_Channels = (int)trackInfo.Value.audio.i_channels;
            }
        }

        //==========================================================================
        /// <summary>
        ///   Overrides <see cref="Track.ToString"/> and returns a string
        ///   representing the audio track.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return new StringBuilder(base.ToString()).Append(" (")
              .Append(Codec).Append(", ").Append(m_Channels).Append(" Channels, ").Append(m_BitRate).Append(" bps")
              .Append(")")

            .ToString();
        }

        #region Properties

        #region Channels

        //==========================================================================
        private readonly int m_Channels;

        //==========================================================================                
        /// <summary>
        ///   Gets the number of channels of the audio track.
        /// </summary>
        public int Channels
        {
            get
            {
                return m_Channels;
            }
        }

        #endregion // Channels

        #region BitRate

        //==========================================================================
        private readonly int m_BitRate;

        //==========================================================================                
        /// <summary>
        ///   Gets bit rate of the audio track.
        /// </summary>
        public int BitRate
        {
            get
            {
                return m_BitRate;
            }
        }

        #endregion // BitRate

        #endregion // Properties

    } // class AudioStream
}
