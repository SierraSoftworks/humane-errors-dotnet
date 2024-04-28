using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

using SierraLib.HumaneErrors;
using System.Linq;

namespace SierraLib.HumanErrors.Tests;


public class HumaneErrorContextTests
{
    [Fact]
    public void WithNoAnnotations()
    {
        try
        {
            throw new Exception("This is a test exception");
        }
        catch (Exception ex)
        {
            ex.GetHumaneContexts().Should().BeEmpty();
        }
    }

    [Fact]
    public void WithAnnotatedRoot()
    {
        try
        {
            throw new Exception("This is a test exception")
                .Humanize("The test threw an expected exception.", new [] { "Make sure the test output was correct." });
        }
        catch (Exception ex)
        {
            var contexts = ex.GetHumaneContexts();
            var context = contexts.Single();
            context.Exception.Should().Be(ex);
            context.FailureMode.Should().Be("The test threw an expected exception.");
            context.Suggestions.Should().BeEquivalentTo(new [] { "Make sure the test output was correct." });
        }
    }

    [Fact]
    public void AnnotatedChild()
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
                throw new Exception("This is a test exception", ex);
            }
        }
        catch (Exception ex)
        {
            var contexts = ex.GetHumaneContexts();
            var context = contexts.Single();
            context.Exception.Should().Be(ex.InnerException);
            context.FailureMode.Should().Be("The test threw an expected exception.");
            context.Suggestions.Should().BeEquivalentTo(new [] { "Make sure the test output was correct." });
        }
    }

    [Fact]
    public void AnnotatedTree()
    {
        try
        {
            try
            {
                try
                {
                    throw new Exception("This is a test exception")
                        .Humanize("The test threw an expected exception at the lowest level.", new [] { "Make sure the test output was correct." });
                }
                catch (Exception ex)
                {
                    throw new Exception("This is a test exception", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("This is a test exception", ex)
                    .Humanize("The test threw an expected exception.", new [] { "Make sure the test output was correct." });
            }
        }
        catch (Exception ex)
        {
            var contexts = ex.GetHumaneContexts().ToArray();
            contexts.Should().HaveCount(2);

            var context = contexts[0];
            context.Exception.Should().Be(ex);
            context.FailureMode.Should().Be("The test threw an expected exception.");
            context.Suggestions.Should().BeEquivalentTo(new [] { "Make sure the test output was correct." });

            context = contexts[1];
            context.Exception.Should().Be(ex.InnerException?.InnerException);
            context.FailureMode.Should().Be("The test threw an expected exception at the lowest level.");
            context.Suggestions.Should().BeEquivalentTo(new [] { "Make sure the test output was correct." });
        }
    }
}