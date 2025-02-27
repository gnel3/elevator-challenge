namespace Elevator.Challenge.Presentation.Extensions;

/// <summary>
/// Provides extension methods for console operations.
/// </summary>
public static class ConsoleExtensions
{
    /// <summary>
    /// Writes the specified message to the console, padded to the width of the console window.
    /// </summary>
    /// <param name="message">The message to write to the console.</param>
    public static void WritePadded(string message)
    {
        Console.Write(message.PadRight(Console.WindowWidth));
    }
}