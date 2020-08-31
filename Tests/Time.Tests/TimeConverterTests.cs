using System.Linq;
using RhythmGameTools.Time;
using Xunit;

namespace Time.Tests
{
    public class TimeConverterTests
    {
        [Fact]
        public void ConstantTempoCalculatesCorrectly()
        {
            var timestamp = TimeConverter.CalculateBarBeatTickWithConstantTempo(1.5f, 120);
            var expected  = new BarBeatTick(0, 3, 0);
            Assert.Equal(expected, timestamp);
            timestamp = TimeConverter.CalculateBarBeatTickWithConstantTempo(7.5f, 60);
            expected  = new BarBeatTick(1, 3, 12000);
            Assert.Equal(expected, timestamp);
            timestamp = TimeConverter.CalculateBarBeatTickWithConstantTempo(7.75f, 60, 3);
            expected  = new BarBeatTick(2, 1, 18000);
            Assert.Equal(expected, timestamp);
        }

        [Fact]
        public void VariableTimeSignatureCalculatesCorrectly()
        {
            var timeConverter = new TimeConverter(new TimeInfo[]
                                                  {
                                                      new TimeInfo(0, 60, 4),
                                                      ( 4, 60, 3 ),
                                                      ( 10, 60, 5 ),
                                                  });
            var actualTimes = new[] {1, 5, 10, 20}.Select(x => timeConverter.CalculateBarBeatTick(x)).ToArray();
            var expectedTimes = new BarBeatTick[]
                                {
                                    ( 0, 1, 0 ),
                                    ( 1, 1, 0 ),
                                    ( 3, 0, 0 ),
                                    ( 5, 0, 0 ),
                                };
            Assert.Equal(expectedTimes, actualTimes);
        }
    }
}