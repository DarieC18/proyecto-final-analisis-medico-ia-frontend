using FluentAssertions;
using MedAnalyzer.Core.Application.Dto.Alert;
using MedAnalyzer.Core.Application.Features.Alerts.Commands;
using MedAnalyzer.Core.Application.Interfaces;
using Moq;

namespace MedAnalyzer.Tests.Handlers
{
    public class ResolveAlertCommandHandlerTests
    {
        private readonly Mock<IAlertService> _serviceMock = new();
        private readonly Mock<IAuditLogService> _auditMock = new();
        private readonly ResolveAlertCommandHandler _handler;

        public ResolveAlertCommandHandlerTests()
        {
            _handler = new ResolveAlertCommandHandler(_serviceMock.Object, _auditMock.Object);
        }

        [Fact]
        public async Task Handle_AlertaExiste_MarcaComoResueltaYRetornaTrue()
        {
            var dto = new AlertDto { Id = 5, IsResolved = false, Title = "Alerta test", Description = "Desc", Severity = "Moderado" };
            _serviceMock.Setup(s => s.GetDtoById(5)).ReturnsAsync(dto);
            _serviceMock.Setup(s => s.UpdateDtoAsync(It.IsAny<AlertDto>(), 5)).ReturnsAsync(dto);

            var result = await _handler.Handle(new ResolveAlertCommand(5, "uid-doc"), CancellationToken.None);

            result.Should().BeTrue();
            _serviceMock.Verify(s => s.UpdateDtoAsync(It.Is<AlertDto>(a => a.IsResolved == true), 5), Times.Once);
        }

        [Fact]
        public async Task Handle_AlertaNoExiste_RetornaFalse()
        {
            _serviceMock.Setup(s => s.GetDtoById(99)).ReturnsAsync((AlertDto?)null);

            var result = await _handler.Handle(new ResolveAlertCommand(99, "uid-doc"), CancellationToken.None);

            result.Should().BeFalse();
            _serviceMock.Verify(s => s.UpdateDtoAsync(It.IsAny<AlertDto>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AlertaNoExiste_NoLlamaUpdate()
        {
            _serviceMock.Setup(s => s.GetDtoById(It.IsAny<int>())).ReturnsAsync((AlertDto?)null);

            await _handler.Handle(new ResolveAlertCommand(7, "uid-doc"), CancellationToken.None);

            _serviceMock.Verify(s => s.UpdateDtoAsync(It.IsAny<AlertDto>(), It.IsAny<int>()), Times.Never);
        }
    }
}
