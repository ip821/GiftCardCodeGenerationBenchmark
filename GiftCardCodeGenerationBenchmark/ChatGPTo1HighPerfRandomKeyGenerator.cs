using System.Runtime.CompilerServices;

public static class ChatGPTo1HighPerfRandomKeyGenerator
{
    // We only want uppercase letters + digits
    private const string ALLOWED_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    // Thread-local Random to avoid contention in multi-threaded environments
    [ThreadStatic]
    private static Random? t_random;

    // Accessor for our thread-local Random
    private static Random Rng => t_random ??= new Random();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GenerateKey()
    {
        // We'll store the result in a 19-character span
        // (16 random characters + 3 dashes)
        Span<char> buffer = stackalloc char[19];
        
        // Cache the AllowedChars and its length in local variables
        ReadOnlySpan<char> allowedChars = ALLOWED_CHARS.AsSpan();
        int allowedLength = allowedChars.Length;

        // We'll keep track of where to write in the buffer
        int index = 0;
        Random random = Rng;

        // Generate 16 characters, inserting a dash after every 4
        for (int i = 0; i < 16; i++)
        {
            // Insert a dash after each 4-character block
            if (i > 0 && i % 4 == 0)
            {
                buffer[index++] = '-';
            }

            // Select a character from allowedChars
            int charPos = random.Next(allowedLength); // range [0, allowedLength)
            buffer[index++] = allowedChars[charPos];
        }

        // Finally, create a string from our stack-allocated buffer
        return new string(buffer);
    }
}