Project Overview

This project implements a stream cipher using a Linear Feedback Shift Register (LFSR) to encrypt and decrypt data. It demonstrates the principles of symmetric key cryptography with LFSR as the core pseudorandom bit generator. The project is implemented as a .NET command-line application and includes functionality to:

    - Generate a keystream using the LFSR.
    
    - Encrypt plaintext using the generated keystream.
    
    - Decrypt ciphertext to retrieve the original plaintext.
    
    - Encrypt and decrypt images by applying XOR operations to pixel values.
    
    - Perform multiple iterations of LFSR operations for testing and evaluation.

The LFSR generates a pseudorandom sequence based on an initial seed and tap position, 
which ensures the security and correctness of the cipher. 
The XOR-based encryption ensures lightweight and fast processing while maintaining confidentiality.
--------------------------------------------------------------------------------------------------------------------

Features

    - LFSR Simulation: Step-by-step simulation of the LFSR to understand its functionality.
    
    - Keystream Generation: Generate pseudorandom bitstreams for encryption.
    
    - Text Encryption and Decryption: Perform encryption and decryption of binary plaintext and ciphertext.
    
    - Image Processing: Encrypt and decrypt image files, preserving their structure while ensuring confidentiality.
    
    - Command-Line Interface: User-friendly CLI to run the program with various options.
--------------------------------------------------------------------------------------------------------------------

Sample Runs with Outputs:

cipher:
    dotnet run cipher 01101000010 9
    01101000010 – seed
    11010000101 1

generatekeystream (the keystream gets saved into keystream.txt so encrypt and decrypt can read from it later):
    dotnet run generatekeystream 01101000010 9 10
    01101000010 – seed
    11010000101 1
    10100001011 1
    01000010110 0
    10000101100 0
    00001011001 1
    00010110010 0
    00101100100 0
    01011001001 1
    10110010010 0
    01100100100 0
    The Keystream: 1100100100

encrypt:
    dotnet run encrypt 0100010000
    The ciphertext is: 1000110100

    dotnet run encrypt 0110010010001011
    The ciphertext is: 0110011110101111

    dotnet run encrypt 1110101
    The ciphertext is: 1101010001

    dotnet run encrypt 1010101010
    The ciphertext is: 0110001110

decrypt:
    dotnet run decrypt 1000110100
    The plaintext is: 0100010000

    dotnet run decrypt 0110011110101111
    The plaintext is: 0110010010001011

    dotnet run decrypt 1101010001
    The plaintext is: 0001110101

    dotnet run decrypt 1100110011
    The plaintext is: 0000010111

multiplebits:
    dotnet run multiplebits 01101000010 9 5 3
    01101000010 - seed
    00001011001 25
    01100100100 4
    10010011110 30

    dotnet run multiplebits 01101000010 9 5 10
    01101000010 - seed
    00001011001 25
    01100100100 4
    10010011110 30
    01111011011 27
    01101110010 18
    11001011010 26
    01101011100 28
    01110011000 24
    01100010111 23
    01011111101 29

Image encryption and decryption:
    dotnet run encryptimage image.jpg 01101000010 9
    -> saved as imageENCRYPTED.png

    dotnet run decryptimage imageENCRYPTED.png 01101000010 9
    -> saved as imageENCRYPTEDNEW.png
--------------------------------------------------------------------------------------------------------------------

Notes:
All generated image files are saved as .png.

Supports encryption and decryption for .jpg, .jpeg, and .png formats.

Images used for testing are from artist WLOP