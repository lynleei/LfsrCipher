﻿/// <summary>
/// Concepts of Parallel and Distributed Systems CSCI-251
///
/// Project 3: Stream Cipher-Linear Feedback Shift Registers (LFSR)
///
/// A program to implement encryption, decryption, and image processing using an LFSR-based stream cipher.
///
/// Usage:
/// dotnet run <option> <arguments>
/// 
/// Options:
///   cipher <seed> <tap>                               - Simulate one step of the LFSR.
///   generatekeystream <seed> <tap> <steps>            - Generate a keystream and save it to a file.
///   encrypt <plaintext>                               - Encrypt plaintext using the keystream.
///   decrypt <ciphertext>                              - Decrypt ciphertext using the keystream.
///   multiplebits <seed> <tap> <step> <iteration>      - Perform multiple LFSR steps and calculations.
///   encryptimage <imagefile> <seed> <tap>             - Encrypt an image file.
///   decryptimage <imagefile> <seed> <tap>             - Decrypt an image file.
///
/// Example Runs:
///     dotnet run generatekeystream 01101000010 9 10
///     dotnet run encrypt 1010101010
///     dotnet run decrypt 1100110011
///     dotnet run cipher 01101000010 9
///     dotnet run multiplebits 01101000010 9 5 3
///     dotnet run encryptimage image.png 01101000010 9
///     dotnet run decryptimage imageENCRYPTED.png 01101000010 9
/// 
/// @author Lynlee Hong
/// </summary>

using System;
using System.IO;
using SkiaSharp;

namespace Lfsr
{
    /// <summary>
    /// Main Program class to handle user input and manage operations such as encryption, decryption, 
    /// keystream generation, and image processing.
    /// </summary>
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

        /// <summary>
        /// Displays the help menu with usage instructions.
        /// </summary>
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

        /// <summary>
        /// Performs a single LFSR step and outputs the new seed and rightmost bit.
        /// </summary>
        static void Cipher(string seed, int tap)
        {
            Console.WriteLine($"{seed} – seed");
            var lfsr = new Lfsr(seed, tap);
            lfsr.Shift();
            Console.WriteLine($"{lfsr.Seed} {lfsr.RightmostBit}");
        }

        /// <summary>
        /// Generates a keystream of the specified length and saves it to a file.
        /// </summary>
        static void GenerateKeystream(string seed, int tap, int steps)
        {
            var lfsr = new Lfsr(seed, tap);
            Console.WriteLine($"{seed} – seed");
            string keystream = lfsr.GenerateKeystream(steps);
            File.WriteAllText("keystream.txt", keystream);
            Console.WriteLine($"The Keystream: {keystream}");
        }

        /// <summary>
        /// Encrypts plaintext using the keystream stored in a file.
        /// </summary>
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

        /// <summary>
        /// Decrypts plaintext using the keystream stored in file.
        /// </summary>
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

        /// <summary>
        /// Performs multiple iterations of LFSR steps and prints the accumulated results.
        /// </summary>
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

        /// <summary>
        /// Encrypts an image file using the LFSR-based stream cipher.
        /// </summary>
        static void EncryptImage(string imagePath, string seed, int tap)
        {
            ProcessImage(imagePath, seed, tap, "ENCRYPTED"); // The encrypted image filename
        }

        /// <summary>
        /// Decrypts an image file using the LFSR-based stream cipher.
        /// </summary>
        static void DecryptImage(string imagePath, string seed, int tap)
        {
            ProcessImage(imagePath, seed, tap, "NEW"); // The decrypted image filename
        }

        /// <summary>
        /// Processes an image file (encrypt or decrypt) by applying the LFSR cipher to each pixel.
        /// </summary>
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

        /// <summary>
        /// Performs a bitwise XOR operation between two binary strings.
        /// </summary>
        static string Xor(string input, string keystream)
        {
            // Pad the shorter string with zeros
            int maxLength = Math.Max(input.Length, keystream.Length);
            string paddedInput = input.PadLeft(maxLength, '0'); // plaintext is shorter than the keystream, pad the keystream to the left with zeros
            string paddedKeystream = keystream.PadLeft(maxLength, '0'); // keystream is shorter than the plaintext, pad the keystream to the left with zeros

            char[] result = new char[maxLength];
            for (int i = 0; i < maxLength; i++)
                result[i] = paddedInput[i] == paddedKeystream[i] ? '0' : '1';

            return new string(result);
        }
    }

    /// <summary>
    /// Implements a Linear Feedback Shift Register (LFSR) for generating pseudorandom sequences.
    /// </summary>
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
