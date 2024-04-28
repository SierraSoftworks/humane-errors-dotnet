namespace SierraLib.HumaneErrors;

using System;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Extension methods which are used to introduce humane information into
/// exceptions to help guide the user to a resolution.
/// </summary>
public static class HumaneErrorsExtensions
{
    /// <summary>
    /// Converts an exception into a humane representation which includes any appropriate context
    /// describing the failure and how to mitigate it.
    /// </summary>
    /// <param name="exception">The exception which may (or may not) contain additional humane context.</param>
    /// <typeparam name="T">The type of exception to convert into a humane representation.</typeparam>
    /// <returns>A humane representation of the exception which includes suggestions on how to mitigate the failure.</returns>
    public static string ToHumaneString<T>(this T? exception)
        where T : Exception
    {
        if (exception is null)
        {
            return string.Empty;
        }

        return HumaneErrorStringBuilder.Build(exception);
    }

    /// <summary>
    /// Humanizes an exception by adding a context-specific description of the failure and
    /// a series of suggestions for how to mitigate the problem. This information can be
    /// retrieved by calling <see cref="ToHumaneString{T}(T)"/>
    /// </summary>
    /// <param name="exception">The exception that the humane information should be attached to.</param>
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
    public static T Humanize<T>(this T exception, string failureMode, string[] suggestions, [CallerMemberName] string memberName = "<unknown>", [CallerFilePath] string filePath = "<unknown>", [CallerLineNumber] int lineNumber = 0)
        where T : Exception
    {
        var information = new HumaneErrorInformation(memberName, filePath, lineNumber, failureMode, suggestions);
        information.Attach(exception);

        return exception;
    }
}
