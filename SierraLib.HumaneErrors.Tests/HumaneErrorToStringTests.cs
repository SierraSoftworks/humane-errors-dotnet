using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

using SierraLib.HumaneErrors;
using System.Linq;

namespace SierraLib.HumanErrors.Tests;


public class HumaneErrorToStringTests
{
    [Fact]
    public void BareExceptions()
    {
        try
        {
            throw new Exception("This is a test exception");
        }
        catch (Exception ex)
        {
            var humanized = ex.ToHumaneString();
            humanized.Should().BeEquivalentTo(ex.ToString(), "when no annotations are present, it should use the default exception formatting");
        }
    }

    [Fact]
    public void BareAggregatedExceptions()
    {
        try
        {
            throw new AggregateException("Several exceptions occurred", new Exception("This is a test exception"), new Exception("This is another test exception"));
        }
        catch (Exception ex)
        {
            var humanized = ex.ToHumaneString();
            humanized.Should().BeEquivalentTo(ex.ToString(), "when no annotations are present, it should use the default exception formatting");
        }
    }

    [Fact]
    public void AnnotatedRootException()
    {
        try
        {
            throw new Exception("This is a test exception")
                .Humanize("The test threw an expected exception.", new [] { "Make sure the test output was correct." });
        }
        catch (Exception ex)
        {
            ex.GetHumaneContexts().Should().NotBeEmpty();

            var humanized = ex.ToHumaneString();
            humanized.Should().StartWith("System.Exception: The test threw an expected exception. (This is a test exception)");
            humanized.Should().Contain("Suggestions:\n   - Make sure the test output was correct.");

            humanized.Should().Contain($"Original Exception:\nSystem.Exception: This is a test exception\n   at SierraLib.HumanErrors.Tests.{nameof(HumaneErrorToStringTests)}.{nameof(AnnotatedRootException)}()", "it should contain the original error message and stack trace as well");
        }
    }

    [Fact]
    public void AnnotatedChildException()
    {
        try
        {
            try
            {
                throw new Exception("This is a test exception")
                    .Humanize("The test threw an expected exception.", new [] { "Make sure the test output was correct." });
            }
            catch (Exception ex)
            {
                throw new Exception("This is a test exception at the root level", ex);
            }
        }
        catch (Exception ex)
        {
            ex.GetHumaneContexts().Should().NotBeEmpty();

            var humanized = ex.ToHumaneString();
            humanized.Should().StartWith("System.Exception: This is a test exception at the root level");
            humanized.Should().Contain("This was caused by:\nSystem.Exception: The test threw an expected exception. (This is a test exception)");
            humanized.Should().Contain("Suggestions:\n   - Make sure the test output was correct.");

            humanized.Should().Contain($"Original Exception:\nSystem.Exception: This is a test exception at the root level\n ---> System.Exception: This is a test exception\n   at SierraLib.HumanErrors.Tests.{nameof(HumaneErrorToStringTests)}.{nameof(AnnotatedChildException)}()", "it should contain the original error message and stack trace as well");
        }
    }
}