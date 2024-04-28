namespace SierraLib.HumaneErrors;

using System;
using System.Text;

class HumaneErrorStringBuilder
{
    private readonly List<string> failureModes = new List<string>();

    private readonly HashSet<string> addedSuggestions = new HashSet<string>();

    private readonly List<string> suggestionsList = new List<string>();

    private readonly Exception rootException;

    private HumaneErrorStringBuilder(Exception rootException)
    {  
        this.rootException = rootException;
    }

    /// <summary>
    /// Builds a representation of the failure context which can then be presented to the user.
    /// </summary>
    /// <param name="exception">The exception which occurred.</param>
    /// <returns>A humane error context builder which can be converted into a human readable error message.</returns>
    public static string Build(Exception exception)
    {
        if (exception is null)
        {
            throw new ArgumentNullException(nameof(exception));
        }
        
        var hasHumaneAnnotations = false;
        var builder = new HumaneErrorStringBuilder(exception);

        while (exception is not null)
        {
            if (HumaneErrorInformation.TryGet(exception, out var humaneInformation))
            {
                hasHumaneAnnotations = true;
                builder.AddCause($"{exception.GetType().FullName}: {humaneInformation.FailureMode} ({exception.Message})\n   at {humaneInformation.FilePath}:line {humaneInformation.LineNumber}");
                foreach (var suggestion in humaneInformation.Suggestions)
                {
                    builder.AddSuggestion(suggestion);
                }
            }
            else
            {
                builder.AddCause($"{exception.GetType().FullName}: {exception.Message}\n{exception.StackTrace?.Split('\n').FirstOrDefault()}".TrimEnd());
            }

            exception = exception.InnerException;
        }

        if (!hasHumaneAnnotations)
        {
            return builder.rootException.ToString();
        }

        return builder.ToString();
    }

    /// <summary>
    /// Returns a string representation of the humane error context.
    /// </summary>
    /// <returns>A string containing a description of the exception(s) and how to resolve them.</returns>
    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.AppendLine(failureModes.First());
        builder.AppendLine();

        foreach (var failureMode in failureModes.Skip(1))
        {
            builder.AppendLine("This was caused by:");
            builder.AppendLine(failureMode);
            builder.AppendLine();
        }

        if (this.suggestionsList.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("Suggestions:");
            foreach (var suggestion in this.suggestionsList)
            {
                builder.AppendLine($"   - {suggestion}");
            }
        }

        builder.AppendLine();
        builder.AppendLine("Original Exception:");
        builder.AppendLine(this.rootException.ToString());

        return builder.ToString();
    }

    /// <summary>
    /// Ads a cause to the list of known causes in this context.
    /// </summary>
    /// <param name="cause">The cause which should be added to the causal list.</param>
    private void AddCause(string cause)
    {
        this.failureModes.Add(cause);
    }

    /// <summary>
    /// Adds a suggestion to the list of suggestions in this context
    /// (if it has not been added previously).
    /// </summary>
    /// <param name="suggestion">The suggestion which should be added.</param>
    private void AddSuggestion(string suggestion)
    {
        if (this.addedSuggestions.Add(suggestion))
        {
            this.suggestionsList.Add(suggestion);
        }
    }
}