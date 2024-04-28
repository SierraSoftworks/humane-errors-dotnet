namespace SierraLib.HumaneErrors;

using System;

/// <summary>
/// An internal representation of the humane error information which can be attached to an exception.s
/// </summary>
readonly struct HumaneErrorInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HumaneErrorInformation"/> struct.
    /// </summary>
    /// <param name="memberName">The name of the method or property within which the failure mode was reported.</param>
    /// <param name="filePath">The file within which the failure mode was reported.</param>
    /// <param name="lineNumber">The line number within the file where the failure mode was reported.</param>
    /// <param name="failureMode">The description of the failure mode which was triggered.</param>
    /// <param name="suggestions">The list of suggestions which explain how to resolve the failure.</param>
    public HumaneErrorInformation(string memberName, string filePath, int lineNumber, string failureMode, string[]? suggestions = null)
    {
        this.MemberName = memberName;
        this.FilePath = filePath;
        this.LineNumber = lineNumber;
        this.FailureMode = failureMode;
        this.Suggestions = suggestions ?? Array.Empty<string>();
    }

    /// <summary>
    /// The unique key used to store the humane error information in the <see cref="Exception.Data"/> dictionary.
    /// </summary>
    public const string ExceptionDataKey = "SierraLib.HumaneErrors.HumaneErrorInformation";

    /// <summary>
    /// Gets a description of what failed from the perspective of the user.
    /// </summary>
    public string FailureMode { get; }

    /// <summary>
    /// Gets the list of suggested remedial actions which may be taken to resolve the failure.
    /// </summary>
    public string[] Suggestions { get; }

    /// <summary>
    /// Gets the name of the method or property within which the failure mode was reported.
    /// </summary>
    public string MemberName { get; }

    /// <summary>
    /// Gets the file within which the failure mode was reported.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets the line number within the file where the failure mode was reported.
    /// </summary>
    public int LineNumber { get; }

    /// <summary>
    /// Attaches the humane error information to the provided exception.
    /// </summary>
    /// <param name="exception">The exception to which this data should be attached.</param>
    public void Attach(Exception exception)
    {
        exception.Data[ExceptionDataKey] = this;
    }

    /// <summary>
    /// Attempts to retrieve the humane error information from the provided exception.
    /// </summary>
    /// <param name="exception">The exception to retrieve the humane error information from.</param>
    /// <param name="humaneInformation">The humane error information which was retrieved (if any).</param>
    /// <returns>Returns <c>true</c> if the information was present on the exception, otherwise <c>false</c>.</returns>
    public static bool TryGet(Exception exception, out HumaneErrorInformation humaneInformation)
    {
        if (exception.Data.Contains(ExceptionDataKey) && exception.Data[ExceptionDataKey] is HumaneErrorInformation info)
        {
            humaneInformation = info;
            return true;
        }

        humaneInformation = default;
        return false;
    }
}
