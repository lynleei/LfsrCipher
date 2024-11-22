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

    dotnet run encryptimage image.png 01101000010 9
    dotnet run decryptimage imageENCRYPTED.png 01101000010 9