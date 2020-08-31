namespace RhythmGameTools.Time
{
    public readonly struct TimeInfo
    {
        /// <summary>
        /// The time at which the tempo is set in seconds
        /// </summary>
        public double Time { get; }

        /// <summary>
        /// The tempo of the song in BPM (beats-per-minute)
        /// </summary>
        public double Tempo { get; }

        /// <summary>
        /// Number of beats per bar/measure in the current time signature
        /// </summary>
        public uint BeatsPerBar { get; }

        /// <summary>
        /// Create a new TimeInfo object
        /// </summary>
        /// <param name="time">The time at which the tempo is set in seconds</param>
        /// <param name="tempo">The tempo of the song in BPM (beats-per-minute)</param>
        /// <param name="beatsPerBar">Number of beats per bar/measure in the current time signature</param>
        public TimeInfo(double time, double tempo, uint beatsPerBar)
        {
            Tempo       = tempo;
            Time        = time;
            BeatsPerBar = beatsPerBar;
        }

        /// <summary>
        /// Implicit creation from tuples
        /// </summary>
        /// <param name="tuple">(time,tempo,beats-per-bar) Tuple to create TimeInfo from</param>
        /// <returns>TimeInfo object</returns>
        public static implicit operator TimeInfo((double time, double tempo, uint beatsPerBar) tuple)
        {
            return new TimeInfo(tuple.time, tuple.tempo, tuple.beatsPerBar);
        }

        /// <summary>
        /// Tuple deconstructor
        /// </summary>
        /// <param name="timeInfo">TimeInfo to deconstruct into a tuple</param>
        /// <returns>(time,tempo,beatsPerBar) tuple</returns>
        public static implicit operator (double time, double tempo, uint beatsPerBar)(TimeInfo timeInfo)
        {
            return ( timeInfo.Time, timeInfo.Tempo, timeInfo.BeatsPerBar );
        }
    }
}