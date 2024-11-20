using System;
using System.IO;
using SkiaSharp;

namespace LfsrCipher
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            switch (args[0].ToLower())
            {
                case "cipher":
                    if (args.Length != 3)
                        Console.WriteLine("Usage: dotnet run cipher <seed> <tap>");
                    else
                        Cipher(args[1], int.Parse(args[2]));
                    break;

                case "generatekeystream":
                    if (args.Length != 4)
                        Console.WriteLine("Usage: dotnet run generatekeystream <seed> <tap> <steps>");
                    else
                        GenerateKeystream(args[1], int.Parse(args[2]), int.Parse(args[3]));
                    break;

                case "encrypt":
                    if (args.Length != 2)
                        Console.WriteLine("Usage: dotnet run encrypt <plaintext>");
                    else
                        Encrypt(args[1]);
                    break;

                case "decrypt":
                    if (args.Length != 2)
                        Console.WriteLine("Usage: dotnet run decrypt <ciphertext>");
                    else
                        Decrypt(args[1]);
                    break;

                case "encryptimage":
                    if (args.Length != 4)
                        Console.WriteLine("Usage: dotnet run encryptimage <imagefile> <seed> <tap>");
                    else
                        EncryptImage(args[1], args[2], int.Parse(args[3]));
                    break;

                case "decryptimage":
                    if (args.Length != 4)
                        Console.WriteLine("Usage: dotnet run decryptimage <imagefile> <seed> <tap>");
                    else
                        DecryptImage(args[1], args[2], int.Parse(args[3]));
                    break;

                default:
                    ShowHelp();
                    break;
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage: dotnet run <option> <arguments>");
            Console.WriteLine("Options:");
            Console.WriteLine("  cipher <seed> <tap>");
            Console.WriteLine("  generatekeystream <seed> <tap> <steps>");
            Console.WriteLine("  encrypt <plaintext>");
            Console.WriteLine("  decrypt <ciphertext>");
            Console.WriteLine("  encryptimage <imagefile> <seed> <tap>");
            Console.WriteLine("  decryptimage <imagefile> <seed> <tap>");
        }

        static void Cipher(string seed, int tap)
        {
            var lfsr = new Lfsr(seed, tap);
            Console.WriteLine($"New Seed: {lfsr.Shift()}, Rightmost Bit: {lfsr.RightmostBit}");
        }

        static void GenerateKeystream(string seed, int tap, int steps)
        {
            var lfsr = new Lfsr(seed, tap);
            string keystream = lfsr.GenerateKeystream(steps);
            File.WriteAllText("keystream.txt", keystream);
            Console.WriteLine($"Keystream saved: {keystream}");
        }

        static void Encrypt(string plaintext)
        {
            if (!File.Exists("keystream.txt"))
            {
                Console.WriteLine("Error: Keystream file not found.");
                return;
            }
            string keystream = File.ReadAllText("keystream.txt");
            Console.WriteLine($"Ciphertext: {Xor(plaintext, keystream)}");
        }

        static void Decrypt(string ciphertext)
        {
            if (!File.Exists("keystream.txt"))
            {
                Console.WriteLine("Error: Keystream file not found.");
                return;
            }
            string keystream = File.ReadAllText("keystream.txt");
            Console.WriteLine($"Plaintext: {Xor(ciphertext, keystream)}");
        }

        static void EncryptImage(string imagePath, string seed, int tap)
        {
            ProcessImage(imagePath, seed, tap, "ENCRYPTED.png");
        }

        static void DecryptImage(string imagePath, string seed, int tap)
        {
            ProcessImage(imagePath, seed, tap, "DECRYPTED.png");
        }

        static void ProcessImage(string imagePath, string seed, int tap, string outputSuffix)
        {
            var lfsr = new Lfsr(seed, tap);
            using var bitmap = SKBitmap.Decode(imagePath);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    int newRed = pixel.Red ^ lfsr.ShiftRandomByte();
                    int newGreen = pixel.Green ^ lfsr.ShiftRandomByte();
                    int newBlue = pixel.Blue ^ lfsr.ShiftRandomByte();
                    bitmap.SetPixel(x, y, new SKColor((byte)newRed, (byte)newGreen, (byte)newBlue));
                }
            }
            bitmap.Encode(new FileStream(Path.GetFileNameWithoutExtension(imagePath) + outputSuffix, FileMode.Create), SKEncodedImageFormat.Png, 100);
        }

        static string Xor(string input, string keystream)
        {
            char[] result = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = input[i] == keystream[i] ? '0' : '1';
            return new string(result);
        }
    }

    class Lfsr
    {
        private string seed;
        private int tap;

        public int RightmostBit { get; private set; }

        public Lfsr(string seed, int tap)
        {
            this.seed = seed;
            this.tap = tap;
        }

        public string Shift()
        {
            int bit1 = seed[0] - '0';
            int bit2 = seed[tap] - '0';
            int newBit = bit1 ^ bit2;
            RightmostBit = seed[^1] - '0';
            seed = seed.Substring(1) + newBit;
            return seed;
        }

        public string GenerateKeystream(int steps)
        {
            string keystream = string.Empty;
            for (int i = 0; i < steps; i++)
            {
                Shift();
                keystream += RightmostBit;
            }
            return keystream;
        }

        public int ShiftRandomByte()
        {
            Shift();
            return Convert.ToInt32(GenerateKeystream(8), 2);
        }
    }
}
