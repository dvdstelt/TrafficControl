namespace Shared;

public static class LicensePlateGenerator
{
    static readonly Random Random = Random.Shared;

    public static string GenerateLicensePlate()
    {
        var format = Random.Next(4);
        return format switch
        {
            0 => $"{Random.Next(100, 999)}-{RandomLetters(2)}-{Random.Next(10)}",
            1 => $"{Random.Next(1, 10)}-{RandomLetters(2)}-{Random.Next(100, 999)}",
            2 => $"{RandomLetters(1)}-{Random.Next(10, 100)}-{RandomLetters(3)}",
            _ => $"{RandomLetters(3)}-{Random.Next(10, 100)}-{RandomLetters(1)}"
        };
    }

    private static string RandomLetters(int count)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return new string(Enumerable.Repeat(chars, count)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}