namespace Day6TuningTrouble;

internal sealed class MessageDecoder
{
    private const char CharA = 'a';
    private const int AlphabetLetterCount = 'z' - CharA + 1;

    private readonly int[] letterCounts;
    private int duplicateCount = 0;
    private bool needsResetting;

    public MessageDecoder(int windowSize)
    {
        WindowSize = windowSize;
        letterCounts = new int[AlphabetLetterCount];
    }

    private int WindowSize { get; }
    public int? MarkerLastCharacterIndex { get; private set; }

    public void ProcessNewMessage(string message)
    {
        Reset();
        needsResetting = true;

        if (message.Length < WindowSize)
        {
            return;
        }

        int letterIndex = 0;
        for (; letterIndex < WindowSize; letterIndex++)
        {
            AddLetter(message[letterIndex]);
        }

        letterIndex--;
        while (duplicateCount > 0)
        {
            letterIndex++;
            if (letterIndex == message.Length)
            {
                break;
            }

            RemoveLetter(message[letterIndex - WindowSize]);
            AddLetter(message[letterIndex]);
        }

        if (duplicateCount == 0)
        {
            MarkerLastCharacterIndex = letterIndex + 1;
        }
    }

    private void AddLetter(char letter)
    {
        int letterIndex = LetterIndex(letter);
        if (++letterCounts[letterIndex] == 2)
        {
            duplicateCount++;
        }
    }

    private void RemoveLetter(char letter)
    {
        int letterIndex = LetterIndex(letter);
        if (--letterCounts[letterIndex] == 1)
        {
            duplicateCount--;
        }
    }

    private static int LetterIndex(char letter)
    {
        return letter - CharA;
    }

    private void Reset()
    {
        if (!needsResetting)
        {
            return;
        }

        for (int i = 0; i < letterCounts.Length; i++)
        {
            letterCounts[i] = 0;
        }
        duplicateCount = 0;
        MarkerLastCharacterIndex = null;
        needsResetting = false;
    }
}
