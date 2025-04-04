using Application.Base;
using FluentValidation;
using Rommanel.Domain.Enum;

namespace Rommanel.Application.Validations
{
    public class ClienteValidator<T> : AbstractValidator<T> where T : ClienteBaseCommand
    {
        public ClienteValidator()
        {
            RuleFor(x => x.Nome)
                .Must(nome => !string.IsNullOrEmpty(nome))
                .WithMessage(x => $"Campo {(x.Tipo == TipoCliente.Fisica ? "Nome" : "Razão Social")} é obrigatório.")
                .MaximumLength(200).WithMessage("O nome não pode exceder 200 caracteres");

            RuleFor(x => x.Nome)
                .Must(nome => nome.All(c => char.IsLetter(c) || c == ' '))
                .When(c => c.Tipo == TipoCliente.Fisica)
                .WithMessage("O nome deve conter apenas letras e espaços");

            RuleFor(x => x.CPFCNPJ)
                .Must(doc => !string.IsNullOrEmpty(doc))
                .WithMessage(x => $"{(x.Tipo == TipoCliente.Fisica ? "CPF" : "CNPJ")} inválido.")
                .Must((cliente, doc) => ValidarTamanhoDocumento(doc, cliente.Tipo))
                .WithMessage(x => $"{(x.Tipo == TipoCliente.Fisica ? "CPF" : "CNPJ")} com tamanho inválido.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email em formato inválido")
                .MaximumLength(254).WithMessage("Email não pode exceder 254 caracteres");

            RuleFor(x => x.DataNascimento)
                .NotEmpty()
                    .When(x => x.Tipo == TipoCliente.Fisica)
                    .WithMessage("Data de nascimento é obrigatória para Pessoa Física")
                .LessThan(DateTime.Today)
                    .When(x => x.Tipo == TipoCliente.Fisica)
                    .WithMessage("Data de nascimento deve ser anterior à data atual")
                .Must(BeValidAge)
                    .When(x => x.Tipo == TipoCliente.Fisica)
                    .WithMessage("O cliente deve ser maior de 18 anos.");

            RuleFor(x => x.IE)
                .Must(doc => !string.IsNullOrEmpty(doc))
                .When(x => x.Tipo == TipoCliente.Juridica)
                .WithMessage("Inscrição Estadual é obrigatória para Pessoa Jurídica");
        }

        private bool ValidarTamanhoDocumento(string documento, TipoCliente tipo)
        {
            return tipo switch
            {
                TipoCliente.Fisica => documento.Length == 11,
                TipoCliente.Juridica => documento.Length == 14,
                _ => false
            };
        }

        private bool BeValidAge(DateTime dataNascimento)
        {
            var idade = DateTime.Today.Year - dataNascimento.Year;
            if (dataNascimento.Date > DateTime.Today.AddYears(-idade)) idade--;

            return idade >= 18;
        }
    }
}