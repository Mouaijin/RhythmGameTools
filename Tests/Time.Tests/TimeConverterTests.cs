using System.Linq;
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
            expected  = new BarBeatTick(1, 3, 512);
            Assert.Equal(expected, timestamp);
            timestamp = TimeConverter.CalculateBarBeatTickWithConstantTempo(7.75f, 60, 3);
            expected  = new BarBeatTick(2, 1, 768);
            Assert.Equal(expected, timestamp);
        }

        [Fact]
        public void VariableTimeSignatureCalculatesCorrectly()
        {
            var timeConverter = new TimeConverter(new[]
                                                  {
                                                      new TimeInfo(60, 0, 4),
                                                      new TimeInfo(60, 4, 3),
                                                      new TimeInfo(60, 10, 5),
                                                  });
            var actualTimes = new[] {1, 5, 10, 20}.Select(x => timeConverter.CalculateBarBeatTick(x)).ToArray();
            var expectedTimes = new[]
                                {
                                    new BarBeatTick(0, 1, 0),
                                    new BarBeatTick(1, 1, 0),
                                    new BarBeatTick(3, 0, 0),
                                    new BarBeatTick(5, 0, 0),
                                };
            Assert.Equal(expectedTimes, actualTimes);
        }
    }
}