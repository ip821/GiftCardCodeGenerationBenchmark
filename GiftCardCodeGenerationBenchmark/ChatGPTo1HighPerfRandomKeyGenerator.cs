using System.Runtime.CompilerServices;
using System.Security.Cryptography;

public static class ChatGPTo1HighPerfRandomKeyGenerator
{
    private const string ALLOWED_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    [ThreadStatic]
    private static Random? t_random;

    private static Random Rng => t_random ??= new Random();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GenerateKey()
    {
        Span<char> buffer = stackalloc char[19];
        ReadOnlySpan<char> allowedChars = ALLOWED_CHARS.AsSpan();
        
        int allowedLength = allowedChars.Length;

        int index = 0;
        Random random = Rng;

        for (int i = 0; i < 16; i++)
        {
            if (i > 0 && i % 4 == 0)
            {
                buffer[index++] = '-';
            }

            int charPos = random.Next(allowedLength);
            buffer[index++] = allowedChars[charPos];
        }

        return new string(buffer);
    }
}

public static class ChatGPTo1HighPerfRandomKeyGeneratorSecure
{
    private const string ALLOWED_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GenerateKey()
    {
        Span<char> buffer = stackalloc char[19];
        ReadOnlySpan<char> allowedChars = ALLOWED_CHARS.AsSpan();
        
        int allowedLength = allowedChars.Length;

        int index = 0;
        for (int i = 0; i < 16; i++)
        {
            if (i > 0 && i % 4 == 0)
            {
                buffer[index++] = '-';
            }

            int charPos = RandomNumberGenerator.GetInt32(allowedLength);
            buffer[index++] = allowedChars[charPos];
        }

        return new string(buffer);
    }
}