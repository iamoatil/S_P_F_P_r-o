using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


namespace XLY.XDD.Control
{

    //****************************************************************************
    /// <summary>
    ///   Base class for all stream classes like a <see cref="VideoStream"/>, 
    ///   an <see cref="AudioStream"/> or a <see cref="SubtitleStream"/>.
    /// </summary>
    public class MediaStream
    {

        //==========================================================================
        private readonly Track m_Track;

        //==========================================================================
        internal MediaStream(Track track)
        {
            if (track == null)
                throw new ArgumentNullException("track");
            m_Track = track;
        }

        //==========================================================================
        /// <summary>
        ///   Overrides <see cref="Object.ToString"/> and returns a string
        ///   representing the media stream.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_Track.ToString();
        }

        #region Properties

        #region Track

        //==========================================================================                
        /// <summary>
        ///   Gets the <c>LibVLC</c> track encapsulated by the media stream.
        /// </summary>
        protected internal Track Track
        {
            get
            {
                return m_Track;
            }
        }

        #endregion // Track

        #region Name

        //==========================================================================                
        /// <summary>
        ///   Gets the name of the track.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Track.Title;
            }
        }

        #endregion // Name

        #region Codec

        //==========================================================================                
        /// <summary>
        ///   Gets the codec of the track.
        /// </summary>
        public string Codec
        {
            get
            {
                return m_Track.Codec;
            }
        }

        #endregion // Codec

        #region Language

        //==========================================================================                
        /// <summary>
        ///   Gets the language of the track; may be <c>null</c> if the 
        ///   language could not be determined.
        /// </summary>
        /// <seealso cref="Culture"/>
        public string Language
        {
            get
            {
                return m_Track.Language;
            }
        }

        #endregion // Language

        #region Culture

        //==========================================================================                
        /// <summary>
        ///   Gets the culture of the track; may be <c>null</c> if the 
        ///   Culture could not be determined.
        /// </summary>
        /// <seealso cref="Language"/>
        public CultureInfo Culture
        {
            get
            {
                return m_Track.Culture;
            }
        }

        #endregion // Culture

        #endregion // Properties


    } // class MediaStream
}
