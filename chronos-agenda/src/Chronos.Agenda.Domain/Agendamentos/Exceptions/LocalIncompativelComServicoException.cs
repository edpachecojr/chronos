using Chronos.Agenda.Domain.Compartilhado.Exceptions;
using Chronos.Agenda.Domain.Servicos.Enums;

namespace Chronos.Agenda.Domain.Agendamentos.Exceptions;

/// <summary>Indica que o local do atendimento não corresponde à modalidade contratada do serviço.</summary>
public sealed class LocalIncompativelComServicoException(TipoAtendimento tipoAtendimentoServico, TipoAtendimento tipoLocal)
    : DomainException($"O local do atendimento ({tipoLocal}) não é compatível com a modalidade do serviço ({tipoAtendimentoServico}).");
