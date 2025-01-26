using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace GiftCardCodeGenerationBenchmark;

public static class Claude35SonnetHighPerformanceRandomGenerator
{
    // Compile-time constants for better performance
    private const int OutputLength = 19;  // 16 chars + 3 dashes
    private const int CharBlockSize = 4;
    private const int BlockCount = 4;
    
    // Using a readonly field instead of a property eliminates property access overhead
    private static readonly uint[] AllowedCharsLookup = CreateAllowedCharsLookup();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Generate()
    {
        // Stack allocate our working buffer
        Span<char> output = stackalloc char[OutputLength];
        
        // Get 8 random bytes (64 bits) which will give us 16 chars (4 bits per char)
        Span<byte> randomBytes = stackalloc byte[8];
        RandomNumberGenerator.Fill(randomBytes);
        
        // Process two 32-bit chunks for better throughput
        ref uint randomUInt1 = ref Unsafe.As<byte, uint>(ref MemoryMarshal.GetReference(randomBytes));
        ref uint randomUInt2 = ref Unsafe.As<byte, uint>(ref MemoryMarshal.GetReference(randomBytes[4..]));
        
        // Fill first half (8 chars + 2 dashes)
        ProcessUInt32(randomUInt1, output);
        
        output[9] = '-';
        
        // Fill second half (8 chars + 1 dash)
        ProcessUInt32(randomUInt2, output.Slice(10));
        
        return new string(output);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ProcessUInt32(uint value, Span<char> output)
    {
        // Process 4 bits at a time to get indices into our allowed chars
        for (int i = 0; i < 8; i++)
        {
            // Extract 4 bits and use them to index into our lookup table
            int charIndex = (int)((value >> (i * 4)) & 0xF);
            output[i + (i / 4)] = (char)AllowedCharsLookup[charIndex];
            
            // Add dash after every 4 characters (except at the end)
            if ((i + 1) % 4 == 0 && i < 7)
            {
                output[i + (i / 4) + 1] = '-';
            }
        }
    }
    
    // Creates a lookup table for fast character selection
    private static uint[] CreateAllowedCharsLookup()
    {
        // We only need 16 entries since we use 4 bits per character
        var lookup = new uint[16];
        var allowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        // Fill lookup table with evenly distributed characters
        for (int i = 0; i < 16; i++)
        {
            int index = (i * allowedChars.Length) / 16;
            lookup[i] = allowedChars[index];
        }
        
        return lookup;
    }
}