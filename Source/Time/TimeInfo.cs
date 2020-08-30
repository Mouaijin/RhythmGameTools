namespace Time
{
    public readonly struct TimeInfo
    {
        /// <summary>
        /// The tempo of the song in BPM (beats-per-minute)
        /// </summary>
        public double Tempo { get; }

        /// <summary>
        /// The time at which the tempo is set in seconds
        /// </summary>
        public double Time { get; }

        /// <summary>
        /// Number of beats per bar/measure in the current time signature
        /// </summary>
        public uint BeatsPerBar { get; }

        /// <summary>
        /// Create a new TimeInfo object
        /// </summary>
        /// <param name="tempo">The tempo of the song in BPM (beats-per-minute)</param>
        /// <param name="time">The time at which the tempo is set in seconds</param>
        /// <param name="beatsPerBar">Number of beats per bar/measure in the current time signature</param>
        public TimeInfo(double tempo, double time, uint beatsPerBar)
        {
            Tempo       = tempo;
            Time        = time;
            BeatsPerBar = beatsPerBar;
        }
    }
}