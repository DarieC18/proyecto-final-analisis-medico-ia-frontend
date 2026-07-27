using FluentAssertions;
using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Features.Appointments.Queries;
using MedAnalyzer.Core.Application.Interfaces;
using Moq;

namespace MedAnalyzer.Tests.Handlers
{
    public class GetAppointmentsByDoctorQueryHandlerTests
    {
        private readonly Mock<IAppointmentService> _serviceMock = new();
        private readonly GetAppointmentsByDoctorQueryHandler _handler;

        public GetAppointmentsByDoctorQueryHandlerTests()
        {
            _handler = new GetAppointmentsByDoctorQueryHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_RetornaCitasDelDoctor()
        {
            const string doctorId = "doctor-uid-1";
            var citas = new List<AppointmentListItemDto>
            {
                new() { Id = 1, DoctorId = doctorId, PatientName = "Carlos Mendoza", Status = "Pending" },
                new() { Id = 2, DoctorId = doctorId, PatientName = "Ana Torres", Status = "Completed" }
            };
            _serviceMock.Setup(s => s.GetAllByDoctorAsync(doctorId)).ReturnsAsync(citas);

            var result = await _handler.Handle(new GetAppointmentsByDoctorQuery(doctorId), CancellationToken.None);

            result.Should().HaveCount(2);
            result.Should().AllSatisfy(c => c.DoctorId.Should().Be(doctorId));
        }

        [Fact]
        public async Task Handle_SinCitas_RetornaListaVacia()
        {
            _serviceMock.Setup(s => s.GetAllByDoctorAsync(It.IsAny<string>())).ReturnsAsync([]);

            var result = await _handler.Handle(new GetAppointmentsByDoctorQuery("uid-sin-citas"), CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_LlamaGetAllByDoctorConElIdCorrecto()
        {
            const string doctorId = "doctor-uid-2";
            _serviceMock.Setup(s => s.GetAllByDoctorAsync(doctorId)).ReturnsAsync([]);

            await _handler.Handle(new GetAppointmentsByDoctorQuery(doctorId), CancellationToken.None);

            _serviceMock.Verify(s => s.GetAllByDoctorAsync(doctorId), Times.Once);
        }
    }
}
