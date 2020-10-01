﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace ImpedanceApp
{
    /// <summary>
    /// Interface all segments
    /// </summary>
    public interface ISegment : ICloneable, IEquatable<ISegment>
    {

        /// <summary>
        /// Set and return <see cref="Name"/> of the <see cref="ISegment"/>
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Return <see cref="SubSegments"/> of <see cref="ISegment"/>
        /// </summary>
        SegmentObservableCollection SubSegments { get; }

        /// <summary>
        /// Return segment the object is
        /// </summary>
        Segment Segment { get;}

        /// <summary>
        /// Event fires when segment changes
        /// </summary>
        event EventHandler SegmentChanged;

        /// <summary>
        /// Calculate impedance one segment
        /// </summary>
        /// <param name="frequency"> is frequency for segment</param>
        /// <returns> <see cref="List{Complex}"/> impedance this <see cref="ISegment"/></returns>
        Complex CalculateZ(double frequency);
    }
}
