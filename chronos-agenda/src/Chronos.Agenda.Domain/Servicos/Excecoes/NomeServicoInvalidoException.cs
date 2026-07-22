using Chronos.Agenda.Domain.Compartilhado.Excecoes;

namespace Chronos.Agenda.Domain.Servicos.Excecoes;

/// <summary>Indica um nome de serviço ausente ou fora do limite permitido.</summary>
public sealed class NomeServicoInvalidoException(int comprimento)
    : DomainException($"O nome do serviço deve ter entre 1 e 120 caracteres; comprimento recebido: {comprimento}.");
