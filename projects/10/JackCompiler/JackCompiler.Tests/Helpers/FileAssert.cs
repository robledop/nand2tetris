using System.Security.Cryptography;
using System.Text;
using FluentAssertions;

namespace JackCompiler.Tests.Helpers;

public static class FileAssert
{
    static string GetFileHash(string filename)
    {
        File.Exists(filename).Should().BeTrue();

#pragma warning disable SYSLIB0021
        using var hash = new SHA1Managed();
#pragma warning restore SYSLIB0021
        var clearBytes = File.ReadAllBytes(filename);
        var hashedBytes = hash.ComputeHash(clearBytes);
        return ConvertBytesToHex(hashedBytes);
    }

    static string ConvertBytesToHex(byte[] bytes)
    {
        var sb = new StringBuilder();

        foreach (var t in bytes)
        {
            sb.Append(t.ToString("x"));
        }
        return sb.ToString();
    }

    public static void AreEqual(string filename1, string filename2)
    {
        string hash1 = GetFileHash(filename1);
        string hash2 = GetFileHash(filename2);

        hash1.Should().BeEquivalentTo(hash2);
    }
}