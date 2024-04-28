using System.Runtime.CompilerServices;

namespace SierraLib.HumaneErrors;

/// <summary>
/// Extension methods which allow you to integrate Humane Errors into your async C# code
/// with minimal effort.
/// </summary>
public static class HumaneErrorsAsyncExtensions
{
    /// <summary>
    /// Humanizes any exceptions thrown by an async <see cref="Task"/>  by adding a context-specific description of the failure and
    /// a series of suggestions for how to mitigate the problem. This information can be retrieved by calling <see cref="ToHumaneString{T}(T)"/>
    /// </summary>
    /// <param name="task">The async <see cref="Task"/> whose exceptions should be annotated with the provided context.</param>
    /// <param name="failureMode">The description of the system's failure mode.</param>
    /// <param name="suggestions">The suggestions about how best to mitigate the failure mode.</param>
    /// <param name="memberName">The name of the method or property where the failure mode was reported.</param>
    /// <param name="filePath">The file path where the failure mode was reported.</param>
    /// <param name="lineNumber">The line number within the file where the failure mode was reported.</param>
    /// <typeparam name="T">The type of exception which should be annotated.</typeparam>
    /// <returns>The original exception, annotated with the provided humane context.</returns>
    /// <remarks>
    /// This method works by adding a new entry to the the exception's <see cref="Exception.Data"/>
    /// which can later be retrieved by calling <see cref="ToHumaneString{T}(T)"/>.
    /// </remarks>
    public static async Task<T> HumanizeAsync<T>(this Task<T> task, string failureMode, string[] suggestions, [CallerMemberName] string memberName = "<unknown>", [CallerFilePath] string filePath = "<unknown>", [CallerLineNumber] int lineNumber = 0)
        where T : Exception
    {
        try
        {
            return await task;
        }
        catch (T exception)
        {
            exception.Humanize(failureMode, suggestions, memberName, filePath, lineNumber);
            throw;
        }
    }
}