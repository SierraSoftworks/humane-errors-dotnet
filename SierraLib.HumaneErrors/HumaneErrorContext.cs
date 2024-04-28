namespace SierraLib.HumaneErrors;

/// <summary>
/// The context which describes a failure mode and the associated exception and suggestions
/// for resolving it.
/// </summary>
/// <remarks>
/// When retrieving the humane error contexts for an exception, you will receive a series
/// of these <see cref="HumaneErrorContext"/> entities in a breadth-first order.
/// </remarks>
public class HumaneErrorContext
{
    private readonly HumaneErrorInformation humaneErrorInformation;

    internal HumaneErrorContext(Exception exception, HumaneErrorInformation humaneErrorInformation)
    {
        this.Exception = exception;
        this.humaneErrorInformation = humaneErrorInformation;
    }

    /// <summary>
    /// Gets the exception associated with the context at a given level of the stack.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Gets the description of the failure mode which was triggered by this exception.
    /// </summary>
    public string FailureMode => this.humaneErrorInformation.FailureMode;

    /// <summary>
    /// Gets the list of suggestions which explain actions which may be taken to resolve the failure.
    /// </summary>
    public string[] Suggestions => this.humaneErrorInformation.Suggestions;

    /// <summary>
    /// Gets the file path where the failure mode was reported.
    /// </summary>
    public string FilePath => this.humaneErrorInformation.FilePath;

    /// <summary>
    /// Gets the line number within the file where the failure mode was reported.
    /// </summary>
    public int LineNumber => this.humaneErrorInformation.LineNumber;
}