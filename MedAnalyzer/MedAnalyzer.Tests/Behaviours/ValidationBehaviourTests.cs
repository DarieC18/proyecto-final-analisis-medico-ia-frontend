using FluentAssertions;
using FluentValidation;
using MedAnalyzer.Core.Application.Behaviours;
using MedAnalyzer.Core.Domain.Exceptions;
using MediatR;
using Moq;

namespace MedAnalyzer.Tests.Behaviours
{
    public class ValidationBehaviourTests
    {
        private record TestRequest(string Value) : IRequest<string>;

        private class PassValidator : AbstractValidator<TestRequest>
        {
            public PassValidator() { RuleFor(x => x.Value).NotEmpty(); }
        }

        private class FailValidator : AbstractValidator<TestRequest>
        {
            public FailValidator() { RuleFor(x => x.Value).Must(_ => false).WithMessage("Siempre falla."); }
        }

        [Fact]
        public async Task Handle_SinValidadores_LlamaNext()
        {
            var behaviour = new ValidationBehaviour<TestRequest, string>([]);
            var nextCalled = false;
            var result = await behaviour.Handle(
                new TestRequest("ok"),
                ct => { nextCalled = true; return Task.FromResult("ok"); },
                CancellationToken.None);

            nextCalled.Should().BeTrue();
            result.Should().Be("ok");
        }

        [Fact]
        public async Task Handle_ValidadorPasa_LlamaNext()
        {
            var behaviour = new ValidationBehaviour<TestRequest, string>([new PassValidator()]);
            var nextCalled = false;
            await behaviour.Handle(
                new TestRequest("valor"),
                ct => { nextCalled = true; return Task.FromResult("ok"); },
                CancellationToken.None);

            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ValidadorFalla_LanzaDomainValidationException()
        {
            var behaviour = new ValidationBehaviour<TestRequest, string>([new FailValidator()]);

            var act = async () => await behaviour.Handle(
                new TestRequest("cualquier cosa"),
                ct => Task.FromResult("ok"),
                CancellationToken.None);

            await act.Should().ThrowAsync<DomainValidationException>()
                .WithMessage("*Siempre falla*");
        }

        [Fact]
        public async Task Handle_MultipleErrores_MensajesUnidosConPuntoYComa()
        {
            var validator = new InlineValidator<TestRequest>();
            validator.RuleFor(x => x.Value).Must(_ => false).WithMessage("Error A.");
            validator.RuleFor(x => x.Value).Must(_ => false).WithMessage("Error B.");

            var behaviour = new ValidationBehaviour<TestRequest, string>([validator]);

            var act = async () => await behaviour.Handle(
                new TestRequest("x"),
                ct => Task.FromResult("ok"),
                CancellationToken.None);

            var ex = await act.Should().ThrowAsync<DomainValidationException>();
            ex.Which.Message.Should().Contain("Error A.");
            ex.Which.Message.Should().Contain("Error B.");
        }

        [Fact]
        public async Task Handle_ValidadorValido_NoLanzaExcepcion()
        {
            var behaviour = new ValidationBehaviour<TestRequest, string>([new PassValidator()]);

            var act = async () => await behaviour.Handle(
                new TestRequest("tiene valor"),
                ct => Task.FromResult("resultado"),
                CancellationToken.None);

            await act.Should().NotThrowAsync();
        }
    }
}
