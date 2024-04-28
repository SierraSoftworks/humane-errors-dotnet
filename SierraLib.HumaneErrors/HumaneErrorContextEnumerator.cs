namespace SierraLib.HumaneErrors;

public static class HumaneErrorsEnumeratorExtensions
{
    /// <summary>
    /// Enumerates the humane error contexts for an exception in a breadth-first order.
    /// </summary>
    /// <param name="exception">The exception to enumerate the humane error contexts for.</param>
    /// <returns>The sequence of humane error contexts which appear within the exception stack.</returns>
    /// <remarks>
    /// When an <see cref="AggregateException"/> appears within the stack, the order of <see cref="HumaneErrorContext"/>s
    /// will be breadth-first (that is, this method will return the first context from each of the aggregated exceptions
    /// before proceeding further down the stack of each). This is chosen intentionally to ensure that more actionable
    /// suggestions appear above less actionable ones.
    /// </remarks>
    public static IEnumerable<HumaneErrorContext> GetHumaneContexts(this Exception exception)
    {
        if (exception is null)
        {
            yield break;
        }

        var exceptionQueue = new Queue<Exception>();
        exceptionQueue.Enqueue(exception);

        while (exceptionQueue.Count > 0 && exceptionQueue.Dequeue() is Exception currentException)
        {
            if (HumaneErrorInformation.TryGet(currentException, out var humaneInformation))
            {
                yield return new HumaneErrorContext(currentException, humaneInformation);
            }

            if (currentException is AggregateException aggregateException)
            {
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    exceptionQueue.Enqueue(innerException);
                }
            }
            else if (currentException.InnerException is Exception innerException)
            {
                exceptionQueue.Enqueue(innerException);
            }
        }
    }
}