using Rommanel.Domain.Enum;

namespace Rommanel.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public TipoCliente Tipo { get; set; }
        public string Nome { get; set; }
        public string CPFCNPJ { get; set; }
        public string? IE { get; set; }
        public DateTime DataNascimento { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
    }
}