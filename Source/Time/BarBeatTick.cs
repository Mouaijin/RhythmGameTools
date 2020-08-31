using System;

namespace RhythmGameTools.Time
{
    public readonly struct BarBeatTick
    {
        private readonly uint?   _bar;
        private readonly uint?   _beat;
        private readonly ushort? _tick;

        /// <summary>
        /// The bar (measure) of the given time
        /// </summary>
        public uint Bar => _bar ?? 0;

        /// <summary>
        /// The beat of the given time
        /// </summary>
        public uint Beat => _beat ?? 0;

        /// <summary>
        /// The number of ticks after the nearest beat of the given time
        /// </summary>
        public ushort Tick => _tick ?? 0;

        /// <summary>
        /// Create a new BarBeatTick object
        /// </summary>
        /// <param name="bar">The bar of the timestamp</param>
        /// <param name="beat">The beat of the timestamp</param>
        /// <param name="tick">The number of ticks after the nearest beat of the timestamp</param>
        public BarBeatTick(uint bar, uint beat, ushort tick)
        {
            _bar  = bar;
            _beat = beat;
            _tick = tick;
        }

        public ulong AsTicks(uint beatsPerBar, ushort ticksPerBeat)
        {
            return Tick + ( Bar * beatsPerBar + Beat ) * ticksPerBeat;
        }

        public BarBeatTick Add(BarBeatTick b, uint beatsPerBar = 4, ushort ticksPerBeat = 24000)
        {
            var tick         = Tick + b.Tick;
            var carryBeat    = tick / ticksPerBeat;
            var adjustedTick = (ushort) ( tick % ticksPerBeat );
            var beat         = (uint) ( Beat + b.Beat + carryBeat );
            var carryBar     = beat / beatsPerBar;
            var adjustedBeat = beat % beatsPerBar;
            var bar          = Bar + b.Bar + carryBar;
            return new BarBeatTick(bar, adjustedBeat, adjustedTick);
        }

        /// <summary>
        /// Subtracts the lesser of the two timestamps from the larger
        /// </summary>
        /// <returns>Difference between two BarBeatTick objects</returns>
        public BarBeatTick Difference(BarBeatTick b, uint beatsPerBar = 4, ushort ticksPerBeat = 24000)
        {
            static (ulong min, ulong max) GetMinMax(ulong x, ulong y) => ( Math.Min(x, y), Math.Max(x, y) );

            var (smallTicks, bigTicks)
                = GetMinMax(AsTicks(beatsPerBar, ticksPerBeat), b.AsTicks(beatsPerBar, ticksPerBeat));
            var diffTicks = bigTicks - smallTicks;
            var tick      = diffTicks % ticksPerBeat;
            var diffBeats = diffTicks / ticksPerBeat;
            var bar       = diffBeats / beatsPerBar;
            var beat      = diffBeats % beatsPerBar;
            return new BarBeatTick((uint) bar, (uint) beat, (ushort) tick);
        }

        public bool Equals(BarBeatTick other)
        {
            return Bar == other.Bar && Beat == other.Beat && Tick == other.Tick;
        }

        public override bool Equals(object obj)
        {
            return obj is BarBeatTick other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int) Bar;
                hashCode = ( hashCode * 397 ) ^ (int) Beat;
                hashCode = ( hashCode * 397 ) ^ Tick.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(BarBeatTick left, BarBeatTick right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BarBeatTick left, BarBeatTick right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(BarBeatTick a, BarBeatTick b)
        {
            return a.Bar < b.Bar || a.Beat < b.Beat || a.Tick < b.Tick;
        }

        public static bool operator >(BarBeatTick a, BarBeatTick b)
        {
            return a.Bar > b.Bar || a.Beat > b.Beat || a.Tick > b.Tick;
        }

        public static bool operator <=(BarBeatTick a, BarBeatTick b)
        {
            return a < b || a == b;
        }

        public static bool operator >=(BarBeatTick a, BarBeatTick b)
        {
            return a > b || a == b;
        }

        public static implicit operator BarBeatTick((uint bar, uint beat, ushort tick) tuple)
        {
            return new BarBeatTick(tuple.bar, tuple.beat, tuple.tick);
        }

        public static implicit operator (uint bar, uint beat, ushort tick)(BarBeatTick bbt)
        {
            return ( bbt.Bar, bbt.Beat, bbt.Tick );
        }

        public override string ToString()
        {
            return $"{Bar}:{Beat}:{Tick}";
        }
    }
}