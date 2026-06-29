using LexManager.Modules.Identity.Domain.Clients;
using LexManager.Modules.Identity.Domain.Clients.Events;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Identity.UnitTests.Domain;

public class ClientTests
{
    private static Email ValidEmail => Email.Create("jean.dupont@example.com");

    [Fact]
    public void CreatePhysicalPerson_Should_SetIdentityAndRaiseEvent()
    {
        Client client = Client.CreatePhysicalPerson(
            PersonName.Create("Jean", "Dupont"),
            "CNIE-12345",
            ValidEmail,
            "+33102030405");

        client.Type.Should().Be(ClientType.PhysicalPerson);
        client.DisplayName.Should().Be("Jean Dupont");
        client.IdentityKey.Should().Be("CNIE-12345");
        client.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ClientCreatedDomainEvent>()
            .Which.DisplayName.Should().Be("Jean Dupont");
    }

    [Fact]
    public void CreateLegalPerson_Should_UseCompanyNameAsDisplayName()
    {
        Client client = Client.CreateLegalPerson(
            Organization.Create("SNE Avocats", "12345678901234", "Maître Martin"),
            ValidEmail);

        client.Type.Should().Be(ClientType.LegalPerson);
        client.DisplayName.Should().Be("SNE Avocats");
        client.IdentityKey.Should().Be("12345678901234");
    }

    [Fact]
    public void CreatePhysicalPerson_Should_Throw_WhenIdentityNumberMissing()
    {
        Action act = () => Client.CreatePhysicalPerson(PersonName.Create("Jean", "Dupont"), "  ", ValidEmail);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(ClientErrors.MissingIdentityNumber);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("")]
    public void Email_Create_Should_Throw_OnInvalidFormat(string value)
    {
        Action act = () => Email.Create(value);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(ClientErrors.InvalidEmail);
    }

    [Theory]
    [InlineData("123")]              // too short
    [InlineData("1234567890123A")]   // non-digit
    public void Organization_Create_Should_Throw_OnInvalidSiret(string siret)
    {
        Action act = () => Organization.Create("SNE Avocats", siret, "Maître Martin");

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(ClientErrors.InvalidSiret);
    }

    [Fact]
    public void Organization_Create_Should_NormalizeSiretSpaces()
    {
        Organization organization = Organization.Create("SNE Avocats", "123 456 789 01234", "Maître Martin");

        organization.RegistrationNumber.Should().Be("12345678901234");
    }
}
