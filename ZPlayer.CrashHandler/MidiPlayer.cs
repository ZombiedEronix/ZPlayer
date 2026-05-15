using NAudio.Midi;

public class MidiPlayer
{
    private static MidiOut midiOut;
    private static MidiFile midiFile;
    static int bpm = 120;
    static int currentTempo = 60000000 / bpm;
    static int division = 0;
    public static void Play(Stream filePath)
    {
        midiOut = new MidiOut(0);
        midiFile = new MidiFile(filePath, false);
        division = midiFile.DeltaTicksPerQuarterNote;

        Thread playThread = new Thread(() =>
        {
            do
            {
                foreach (var midiEvent in midiFile.Events[0])
                {
                    double waitTime = (midiEvent.DeltaTime * currentTempo) / (division * 1000.0);
                    if (midiOut == null) break;
                    Thread.Sleep((int)waitTime);
                    midiOut.Send(midiEvent.GetAsShortMessage());


                }
            } while (midiOut != null);
        });

        playThread.IsBackground = true;
        playThread.Start();
    }

    public static void Stop()
    {
        if (midiOut != null)
        {
            midiOut.Reset();
            midiOut.Dispose();
            midiOut = null;
        }
    }
};
