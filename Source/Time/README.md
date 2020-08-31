# RhythmGameTools.Time

Contains TimeConverter, which provides logic necessary for computation of Bar-Beat-Tick notation of arbitrary timestamps within constant or variable tempo/time-signature songs. 

## Domain Information

### Bar-Beat-Tick notation

Bar-Beat-Tick notation describes a position in a musical piece defined by the bar (or measure), beat, and number of ticks of offset after the beat. This library is 0-based for all aspects of this notation (however, displaying a 1-based beat column only requires you to create a duplicate BarBeatTick struct with 1 added to the beat).

### Ticks

Ticks default to 24000 subdivisions of a beat, but can be set at will. It's worth noting, however, that anything more than 24,000 is likely overkill, as 24,000 is already an absurdly fine-grain. Using a 24,000 ticks/beat, each tick is approximately 0.02ms long (at 120 BPM). For reference, the StepMania timing windows in milliseconds are `{22ms, 45ms, 90ms, 135ms, 180ms}`. That's 1100 chances for a Marvelous using the default tick length, at 120 BPM!

## Usage

All the logic needed for time conversion can be found inside RhythmGameTools.Time.TimeConverter. 

Constant-tempo/time-signature time conversions can be accessed via static methods, without the need to ever call  `new TimeConverter()`. Variable tempo/time-signature songs require an instantiation, but after the initial setup calls to convert time are simple and straight-forward. 



### Constant tempo and constant time signature
```c#
//Time and BPM each accept a double. Beats per bar is optional and defaults to 4. Ticks per beat is also optional, and defaults to 24000.
BarBeatTick timestamp = RhythmGameTools.TimeConverter.CalculateBarBeatTickWithConstantTempo(time: 1.5D, bpm: 120.0D, beatsPerBar: 4);
Console.WriteLine($"{timestamp} should equal Bar: 0, Beat: 3, Tick: 0");
```


### Variable tempo/time signature
```c#
//Set up the TimeConverter object with a collection of TimeInfo objects
//This will define each tempo, the time at which the song begins that tempo, and the number of beats per bar starting at this time
var timeConverter = new TimeConverter(new TimeInfo[]
                                                  {
                                                      //Time info accepts a time in seconds as a double, a tempo (in beats per minute) as a double, and the number of beats per bar as a uint
                                                      new TimeInfo ( time: 0, tempo: 60, beatsPerBar: 4 ), //60 BPM at 0 seconds, 4 beats/bar
                                                      //TimeInfo objects can be written as tuples of (time,tempo,beatsPerBar)
                                                      ( 4, 120, 3 ),  //60 BPM at 4 seconds, 3 beats/bar
                                                      ( 10, 60, 5 ), //60 BPM at 10 seconds, 5 beats/bar
                                                  });
//Calculate the timestamp at 5 seconds of playtime
var timestamp = timeConverter.CalculateBarBeatTick(5.0D);
Console.WriteLine($"{timestamp} should equal Bar: 1, Beat: 2, Tick: 0");
```