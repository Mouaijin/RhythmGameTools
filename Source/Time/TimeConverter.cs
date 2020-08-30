namespace Time
{
    public class TimeConverter
    {
        private TimeInfo[] _timeSignatureAndTempoChanges;
        private ushort     _ticksPerBeat;

        private BarBeatTick[] _calculatedTimeSignatureAndTempoChangeOffsets;

        public TimeConverter(TimeInfo[] timeSignatureAndTempoChanges, ushort ticksPerBeat = 1024)
        {
            _timeSignatureAndTempoChanges                 = timeSignatureAndTempoChanges;
            _ticksPerBeat                                 = ticksPerBeat;
            _calculatedTimeSignatureAndTempoChangeOffsets = CalculateTimeChangeOffsets(_timeSignatureAndTempoChanges);
        }

        /// <summary>
        /// Calculates the bar-beat-tick representation of a time given the loaded tempo/time-signature data
        /// </summary>
        /// <param name="time">The time offset to calculate</param>
        /// <returns>A BarBeatTick object</returns>
        public BarBeatTick CalculateBarBeatTick(double time)
        {
            //Find the section to which this belongs
            int index = 0;
            for (int i = 0; i < _timeSignatureAndTempoChanges.Length; i++)
            {
                if (_timeSignatureAndTempoChanges[i].Time <= time)
                    index = i;
                else
                    break;
            }

            //Do calculation
            return CalculateBarBeatTickWithVariableTimeSignatureOrTempo(time, _timeSignatureAndTempoChanges[index], _calculatedTimeSignatureAndTempoChangeOffsets[index]);
        }

        public double CalculateDiscreteTime(BarBeatTick timestamp)
        {
            //Find the section to which this belongs
            int index = 0;
            for (int i = 0; i < _calculatedTimeSignatureAndTempoChangeOffsets.Length; i++)
            {
                if (_calculatedTimeSignatureAndTempoChangeOffsets[i] <= timestamp)
                    index = i;
                else
                    break;
            }

            //Do calculation
            return CalculateDiscreteTime(timestamp, _timeSignatureAndTempoChanges[index], _calculatedTimeSignatureAndTempoChangeOffsets[index], _ticksPerBeat);
        }

        /// <summary>
        /// Calculates the encoded timestamps at which each section begins
        /// </summary>
        /// <param name="timeSignatureAndTempoChanges">Time/Tempo/TimeSignature definitions for the song</param>
        /// <param name="ticksPerBeat">Resolution of ticks (keep consistent throughout app to avoid errors)</param>
        /// <returns>An array of timestamps corresponding to the supplied data</returns>
        public static BarBeatTick[] CalculateTimeChangeOffsets(TimeInfo[] timeSignatureAndTempoChanges, ushort ticksPerBeat = 1024)
        {
            //Prepare response
            var offsets = new BarBeatTick[timeSignatureAndTempoChanges.Length];
            //Early exit for empty array
            if (timeSignatureAndTempoChanges.Length == 0)
            {
                return offsets;
            }

            //Preload base-case
            if (timeSignatureAndTempoChanges.Length >= 1)
            {
                offsets[0] = CalculateBarBeatTickWithConstantTempo(timeSignatureAndTempoChanges[0].Time, timeSignatureAndTempoChanges[0].Tempo, timeSignatureAndTempoChanges[0].BeatsPerBar,
                                                                   ticksPerBeat);
            }

            //Back-reference each time and tempo to calculate the BBT timestamp of the current time and tempo
            for (int i = 1; i < timeSignatureAndTempoChanges.Length; i++)
            {
                var timestamp = CalculateBarBeatTickWithConstantTempo(timeSignatureAndTempoChanges[i].Time - timeSignatureAndTempoChanges[i - 1].Time,
                                                                                                          timeSignatureAndTempoChanges[i - 1].Tempo,
                                                                                                          timeSignatureAndTempoChanges[i - 1].BeatsPerBar,
                                                                                                          ticksPerBeat);
                offsets[i] = offsets[i - 1].Add(timestamp, timeSignatureAndTempoChanges[i - 1].BeatsPerBar, ticksPerBeat);
            }

            return offsets;
        }

        /// <summary>
        /// Calculates the Bar-Beat-Tick representation of a time using the given bpm assuming bpm remains constant
        /// </summary>
        /// <param name="time">Time offset to calculate</param>
        /// <param name="bpm">Tempo of song in BPM (beats-per-minute)</param>
        /// <param name="beatsPerBar">Number of beats in a bar</param>
        /// <param name="ticksPerBeat">Number of ticks per beat (default 1024)</param>
        /// <returns>Bar-Beat-Tick representation of the time</returns>
        public static BarBeatTick CalculateBarBeatTickWithConstantTempo(double time, double bpm, uint beatsPerBar = 4, ushort ticksPerBeat = 1024)
        {
            var beatLengthInSeconds = 60 / bpm;
            var exactBeatAtTime     = time / beatLengthInSeconds;
            var bar                 = (uint) ( exactBeatAtTime / beatsPerBar );
            var beatDouble          = exactBeatAtTime % beatsPerBar;
            var beat                = (uint) beatDouble;
            var tick                = (ushort) ( ( beatDouble - beat ) * ticksPerBeat );
            return new BarBeatTick(bar, beat, tick);
        }

        /// <summary>
        /// Calculates the Bar-Beat-Tick representation of a time using the given bpm remains constant
        /// </summary>
        /// <param name="time">Time offset to calculate</param>
        /// <param name="timeSignatureOrTempoChange">Tempo/Time signature at time</param>
        /// <param name="calculatedTimeSignatureOrTempoChangeOffset">Beat-Bar-Tick offset for timeSignatureOrTempoChange</param>
        /// <param name="ticksPerBeat">Number of ticks per beat (default 1024)</param>
        /// <returns>Bar-Beat-Tick representation of the time</returns>
        public static BarBeatTick CalculateBarBeatTickWithVariableTimeSignatureOrTempo(double time, TimeInfo timeSignatureOrTempoChange, BarBeatTick calculatedTimeSignatureOrTempoChangeOffset,
                                                                                       ushort ticksPerBeat = 1024)
        {
            var adjustedTime = time - timeSignatureOrTempoChange.Time;
            var offset       = CalculateBarBeatTickWithConstantTempo(adjustedTime, timeSignatureOrTempoChange.Tempo, timeSignatureOrTempoChange.BeatsPerBar, ticksPerBeat);
            return offset.Add(calculatedTimeSignatureOrTempoChangeOffset);
        }

        /// <summary>
        /// Calculates the discrete time in seconds given generated and supplied time data
        /// </summary>
        /// <param name="barBeatTick">Timestamp to convert</param>
        /// <param name="timeSignatureOrTempoChange">Tempo/Time signature at time</param>
        /// <param name="calculatedTimeSignatureOrTempoChangeOffset">Beat-Bar-Tick offset for timeSignatureOrTempoChange</param>
        /// <param name="ticksPerBeat">Number of ticks per beat (default 1024)</param>
        /// <returns>Discrete time representation in seconds</returns>
        public static double CalculateDiscreteTime(BarBeatTick barBeatTick, TimeInfo timeSignatureOrTempoChange, BarBeatTick calculatedTimeSignatureOrTempoChangeOffset,
                                                   ushort      ticksPerBeat = 1024)
        {
            var rawTicks            = barBeatTick.AsTicks(timeSignatureOrTempoChange.BeatsPerBar, ticksPerBeat);
            var rawTicksOfOffset    = calculatedTimeSignatureOrTempoChangeOffset.AsTicks(timeSignatureOrTempoChange.BeatsPerBar, ticksPerBeat);
            var tickDifference      = rawTicks - rawTicksOfOffset;
            var differenceAsSeconds = (double) ( tickDifference / ticksPerBeat );
            var realTime            = differenceAsSeconds + timeSignatureOrTempoChange.Time;
            return realTime;
        }
    }
}