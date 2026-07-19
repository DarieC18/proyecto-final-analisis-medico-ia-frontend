using FluentAssertions;
using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Application.Features.Patients.Queries;
using MedAnalyzer.Core.Application.Interfaces;
using Moq;

namespace MedAnalyzer.Tests.Handlers
{
    public class GetAllPatientsQueryHandlerTests
    {
        private readonly Mock<IPatientService> _serviceMock = new();
        private readonly GetAllPatientsQueryHandler _handler;

        public GetAllPatientsQueryHandlerTests()
        {
            _handler = new GetAllPatientsQueryHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_CuandoHayPacientes_RetornaLista()
        {
            var pacientes = new List<PatientDto>
            {
                new() { Id = 1, UserId = "uid-1", PatientType = "Nuevo", IsActive = true },
                new() { Id = 2, UserId = "uid-2", PatientType = "Recurrente", IsActive = true }
            };
            _serviceMock.Setup(s => s.GetActivePatients()).ReturnsAsync(pacientes);

            var result = await _handler.Handle(new GetAllPatientsQuery(), CancellationToken.None);

            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(pacientes);
        }

        [Fact]
        public async Task Handle_CuandoNoHayPacientes_RetornaListaVacia()
        {
            _serviceMock.Setup(s => s.GetActivePatients()).ReturnsAsync([]);

            var result = await _handler.Handle(new GetAllPatientsQuery(), CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_LlamaGetActivePatientsUnaVez()
        {
            _serviceMock.Setup(s => s.GetActivePatients()).ReturnsAsync([]);

            await _handler.Handle(new GetAllPatientsQuery(), CancellationToken.None);

            _serviceMock.Verify(s => s.GetActivePatients(), Times.Once);
        }
    }
}
