using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using GiftCardCodeGenerationBenchmark;


Console.WriteLine(new OriginalGenerator().OriginalGenerate());
Console.WriteLine(new OriginalGenerator().Generate16_ChatGPT4o());
Console.WriteLine(new OriginalGenerator().Generate16_Claude35Haiku());
Console.WriteLine(new OriginalGenerator().Generate16_HP_Claude35Sonnet());
Console.WriteLine(new OriginalGenerator().Generate16_HP_ChatGPTo1());

_ = BenchmarkRunner.Run<OriginalGenerator>();

public class OriginalGenerator
{
    private const string ValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    [Benchmark(Baseline = true)]
    public string OriginalGenerate()
    {
        var segments = new string[4];
        for (var i = 0; i < segments.Length; i++)
            segments[i] = RandomNumberGenerator.GetString(ValidChars, 4);
        return string.Join('-', segments);
    }
    
    [Benchmark]
    public string Generate16_ChatGPT4o()
    {
        var code = RandomNumberGenerator.GetString(ValidChars, 16);
        var result = new StringBuilder(20);

        for (var i = 0; i < 16; i++)
        {
            result.Append(code[i]);
            if(i != 0 && (i +1) % 4 == 0 && i < 15)
                result.Append('-');
        }
        
        return result.ToString();
    }
    
    [Benchmark]
    public string Generate16_Claude35Haiku()
    {
        Span<byte> randomBytes = stackalloc byte[16];
        RandomNumberGenerator.Fill(randomBytes);
        
        Span<char> result = stackalloc char[19];
        for (int i = 0; i < 16; i++)
        {
            var index = i + (i / 4); 
            result[index] = ValidChars[randomBytes[i] % ValidChars.Length];
            
            if (i > 0 && (i + 1) % 4 == 0 && i < 15)
            {
                result[index + 1] = '-';
            }
        }

        
        return result.ToString();
    }
    
    [Benchmark]
    public string Generate16_ChatGPTo1()
    {
        Span<byte> randomBytes = stackalloc byte[16];
        RandomNumberGenerator.Fill(randomBytes);

        Span<char> buffer = stackalloc char[19];

        var bufferIndex = 0;
        for (int i = 0; i < 16; i++)
        {
            if (i > 0 && i % 4 == 0)
            {
                buffer[bufferIndex++] = '-';
            }

            buffer[bufferIndex++] = ValidChars[randomBytes[i] % ValidChars.Length];
        }

        return new string(buffer);
    }

    [Benchmark]
    public string Generate16_HP_Claude35Sonnet() => Claude35SonnetHighPerformanceRandomGenerator.Generate();
    
    [Benchmark]
    public string Generate16_HP_ChatGPTo1() => ChatGPTo1HighPerfRandomKeyGenerator.GenerateKey();
}