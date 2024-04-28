using System;
using FluentAssertions;
using Xunit;

namespace SierraLib.HumaneErrors.Tests;

public class HumaneErrorAnnotationTests
{
    [Fact]
    public void RetainsOriginalErrorReference()
    {
        var ex = new Exception("Original exception");

        var annotated = ex.Humanize("This is a test exception", new[] { "Make sure the test output was correct." });
        annotated.Should().BeSameAs(ex);
    }
}