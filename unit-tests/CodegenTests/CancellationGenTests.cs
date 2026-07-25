using NUnit.Framework;
using SqlcGenCsharp.Drivers.Generators;

namespace CodegenTests;

public class CancellationGenTests
{
    [Test]
    public void EnabledEmitsOptionalTokenParameter()
    {
        var cancellation = new CancellationGen(enabled: true);
        Assert.That(cancellation.MethodParameter(), Is.EqualTo("CancellationToken cancellationToken = default"));
    }

    [Test]
    public void DisabledEmitsNoTokenParameter()
    {
        var cancellation = new CancellationGen(enabled: false);
        Assert.That(cancellation.MethodParameter(), Is.Empty);
    }

    [Test]
    public void EnabledAppendsTokenAfterExistingParameters()
    {
        var cancellation = new CancellationGen(enabled: true);
        Assert.That(cancellation.TrailingMethodParameter(), Is.EqualTo(", CancellationToken cancellationToken = default"));
    }

    [Test]
    public void DisabledAppendsNoParameter()
    {
        var cancellation = new CancellationGen(enabled: false);
        Assert.That(cancellation.TrailingMethodParameter(), Is.Empty);
    }

    [Test]
    public void EnabledPassesTokenAsSoleArgument()
    {
        var cancellation = new CancellationGen(enabled: true);
        Assert.That(cancellation.Argument(), Is.EqualTo("cancellationToken"));
    }

    [Test]
    public void DisabledPassesNoArgument()
    {
        var cancellation = new CancellationGen(enabled: false);
        Assert.That(cancellation.Argument(), Is.Empty);
    }

    [Test]
    public void EnabledAppendsTokenAfterExistingArguments()
    {
        var cancellation = new CancellationGen(enabled: true);
        Assert.That(cancellation.TrailingArgument(), Is.EqualTo(", cancellationToken"));
    }

    [Test]
    public void DisabledAppendsNothing()
    {
        var cancellation = new CancellationGen(enabled: false);
        Assert.That(cancellation.TrailingArgument(), Is.Empty);
    }

    [Test]
    public void EnabledWrapsDapperArgsInCommandDefinitionCarryingToken()
    {
        var cancellation = new CancellationGen(enabled: true);
        Assert.That(
            cancellation.WrapDapperArgs("SqlText, queryParams"),
            Is.EqualTo("new CommandDefinition(SqlText, queryParams, cancellationToken: cancellationToken)"));
    }

    [Test]
    public void DisabledLeavesDapperArgsUntouched()
    {
        var cancellation = new CancellationGen(enabled: false);
        Assert.That(cancellation.WrapDapperArgs("SqlText, queryParams"), Is.EqualTo("SqlText, queryParams"));
    }
}