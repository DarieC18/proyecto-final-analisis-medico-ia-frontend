using FluentAssertions;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Infraestructure.Persistences.Context;
using Microsoft.EntityFrameworkCore;

namespace MedAnalyzer.Tests.Integration;

public class PatientRepositoryIntegrationTests
{
    private static MedAnalyzerContextDb CreateContext()
    {
        var options = new DbContextOptionsBuilder<MedAnalyzerContextDb>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new MedAnalyzerContextDb(options);
    }

    private static Patient BuildPatient(int id, string userId) => new()
    {
        Id = id,
        UserId = userId,
        BirthDate = new DateOnly(1990, 1, 1),
        Gender = "Masculino",
        PhoneNumber = "8091234567",
        IdentificationType = "Cédula",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task SaveEntity_DebeGuardarPacienteEnBd()
    {
        await using var context = CreateContext();
        context.Patients.Add(BuildPatient(1, "uid-test"));
        await context.SaveChangesAsync();

        var result = await context.Patients.FindAsync(1);
        result.Should().NotBeNull();
        result!.PhoneNumber.Should().Be("8091234567");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_DebeRetornarTodosLosPacientesGuardados()
    {
        await using var context = CreateContext();
        context.Patients.AddRange(
            BuildPatient(1, "uid-1"),
            BuildPatient(2, "uid-2"),
            BuildPatient(3, "uid-3")
        );
        await context.SaveChangesAsync();

        var result = await context.Patients.ToListAsync();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task UpdateEntity_DebeModificarCampoEnBd()
    {
        await using var context = CreateContext();
        var patient = BuildPatient(1, "uid-1");
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        patient.PhoneNumber = "8099999999";
        context.Patients.Update(patient);
        await context.SaveChangesAsync();

        var result = await context.Patients.FindAsync(1);
        result!.PhoneNumber.Should().Be("8099999999");
    }

    [Fact]
    public async Task RemoveEntity_DebeEliminarPacienteDeBd()
    {
        await using var context = CreateContext();
        var patient = BuildPatient(1, "uid-1");
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        context.Patients.Remove(patient);
        await context.SaveChangesAsync();

        var result = await context.Patients.FindAsync(1);
        result.Should().BeNull();
    }

    [Fact]
    public async Task FindById_DebeRetornarNullSiNoExiste()
    {
        await using var context = CreateContext();

        var result = await context.Patients.FindAsync(999);
        result.Should().BeNull();
    }
}
