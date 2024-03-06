namespace SqlcGenCsharpTest;

public static class StreamsExtensions
{
    public static bool ContentEquals(this MemoryStream ms1, MemoryStream ms2)
    {
        if (ms1.Length != ms2.Length)
            return false;
        ms1.Position = 0;
        ms2.Position = 0;

        var msArray1 = ms1.ToArray();
        var msArray2 = ms2.ToArray();

        return msArray1.SequenceEqual(msArray2);
    }
}