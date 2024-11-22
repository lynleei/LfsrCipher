using System;
using System.IO;
using SkiaSharp;

namespace Lfsr
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

                case "multiplebits":
                    if (args.Length != 5)
                        Console.WriteLine("Usage: dotnet run multiplebits <seed> <tap> <step> <iteration>");
                    else
                        MultipleBits(args[1], int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]));
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
            Console.WriteLine("  multiplebits <seed> <tap> <step> <iteration>");
            Console.WriteLine("  encryptimage <imagefile> <seed> <tap>");
            Console.WriteLine("  decryptimage <imagefile> <seed> <tap>");
        }

        static void Cipher(string seed, int tap)
        {
            Console.WriteLine($"{seed} – seed");
            var lfsr = new Lfsr(seed, tap);
            lfsr.Shift();
            Console.WriteLine($"{lfsr.Seed} {lfsr.RightmostBit}");
        }

        static void GenerateKeystream(string seed, int tap, int steps)
        {
            var lfsr = new Lfsr(seed, tap);
            Console.WriteLine($"{seed} – seed");
            string keystream = lfsr.GenerateKeystream(steps);
            File.WriteAllText("keystream.txt", keystream);
            Console.WriteLine($"The Keystream: {keystream}");
        }

        static void Encrypt(string plaintext)
        {
            if (!File.Exists("keystream.txt"))
            {
                Console.WriteLine("Error: Keystream file not found.");
                return;
            }
            string keystream = File.ReadAllText("keystream.txt");
            string ciphertext = Xor(plaintext, keystream);
            Console.WriteLine($"The ciphertext is: {ciphertext}");
        }

        static void Decrypt(string ciphertext)
        {
            if (!File.Exists("keystream.txt"))
            {
                Console.WriteLine("Error: Keystream file not found.");
                return;
            }
            string keystream = File.ReadAllText("keystream.txt");
            string plaintext = Xor(ciphertext, keystream);
            Console.WriteLine($"The plaintext is: {plaintext}");
        }

        static void MultipleBits(string seed, int tap, int steps, int iterations)
        {
            Console.WriteLine($"{seed} - seed");
            for (int i = 0; i < iterations; i++)
            {
                var lfsr = new Lfsr(seed, tap);
                int accumulatedValue = 0;
                for (int j = 0; j < steps; j++)
                {
                    lfsr.Shift();
                    accumulatedValue = accumulatedValue * 2 + lfsr.RightmostBit;
                }
                Console.WriteLine($"{lfsr.Seed} {accumulatedValue}");
                seed = lfsr.Seed; // Use the new seed for the next iteration
            }
        }

        static void EncryptImage(string imagePath, string seed, int tap)
        {
            ProcessImage(imagePath, seed, tap, "ENCRYPTED"); // The encrypted image filename
        }

        static void DecryptImage(string imagePath, string seed, int tap)
        {
            ProcessImage(imagePath, seed, tap, "NEW"); // The decrypted image filename
        }

        static void ProcessImage(string imagePath, string seed, int tap, string outputSuffix)
        {
            var lfsr = new Lfsr(seed, tap);
            using var bitmap = SKBitmap.Decode(imagePath);

            if (bitmap == null)
            {
                Console.WriteLine($"Error: Unable to load image from path '{imagePath}'.");
                return;
            }

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    byte newRed = (byte)(pixel.Red ^ lfsr.ShiftRandomByte());
                    byte newGreen = (byte)(pixel.Green ^ lfsr.ShiftRandomByte());
                    byte newBlue = (byte)(pixel.Blue ^ lfsr.ShiftRandomByte());
                    bitmap.SetPixel(x, y, new SKColor(newRed, newGreen, newBlue));
                }
            }

            string outputFileName = Path.GetFileNameWithoutExtension(imagePath) + outputSuffix + ".png";
            string outputFilePath = Path.Combine(Path.GetDirectoryName(imagePath), outputFileName);

            using var stream = File.Create(outputFilePath);
            bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
        }

        static string Xor(string input, string keystream)
        {
            // Pad the shorter string with zeros
            int maxLength = Math.Max(input.Length, keystream.Length);
            string paddedInput = input.PadLeft(maxLength, '0');
            string paddedKeystream = keystream.PadLeft(maxLength, '0');

            char[] result = new char[maxLength];
            for (int i = 0; i < maxLength; i++)
                result[i] = paddedInput[i] == paddedKeystream[i] ? '0' : '1';

            return new string(result);
        }
    }

    class Lfsr
    {
        private string seed;
        private int tap;

        public string Seed => seed;
        public int RightmostBit { get; private set; }

        public Lfsr(string seed, int tap)
        {
            this.seed = seed;
            this.tap = tap;
        }

        public void Shift()
        {
            int shiftedOffBit = seed[0] - '0';
            int tapIndex = seed.Length - tap; // Adjust tap position
            int tapBit = seed[tapIndex] - '0';
            int newBit = shiftedOffBit ^ tapBit;
            seed = seed.Substring(1) + newBit.ToString();
            RightmostBit = newBit;
        }

        public string GenerateKeystream(int steps)
        {
            string keystream = string.Empty;
            for (int i = 0; i < steps; i++)
            {
                Shift();
                keystream += RightmostBit;
                Console.WriteLine($"{seed} {RightmostBit}");
            }
            return keystream;
        }

        public int ShiftRandomByte()
        {
            int value = 0;
            for (int i = 0; i < 8; i++)
            {
                Shift();
                value = (value << 1) | RightmostBit;
            }
            return value;
        }
    }
}
