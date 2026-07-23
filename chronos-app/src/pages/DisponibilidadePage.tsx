import { Plus } from "lucide-react"
import { useCallback, useEffect, useState } from "react"

import {
  listarDisponibilidades,
  removerDisponibilidade,
  traduzirErroDeDisponibilidade,
  type DisponibilidadeResumo,
} from "@/api/disponibilidades"
import { DisponibilidadeDialog } from "@/components/disponibilidade/DisponibilidadeDialog"
import { DisponibilidadeLista } from "@/components/disponibilidade/DisponibilidadeLista"
import { EstadoCarregando } from "@/components/estado/EstadoCarregando"
import { EstadoErro } from "@/components/estado/EstadoErro"
import { EstadoVazio } from "@/components/estado/EstadoVazio"
import { Button } from "@/components/ui/button"
import { useAuth } from "@/hooks/useAuth"
import { useProfissionalAtual } from "@/hooks/useProfissionalAtual"

export function DisponibilidadePage() {
  const { accessToken } = useAuth()
  const { profissionalId, carregando: carregandoProfissional, erro: erroProfissional } = useProfissionalAtual()
  const [disponibilidades, setDisponibilidades] = useState<DisponibilidadeResumo[] | null>(null)
  const [erro, setErro] = useState<string | null>(null)
  const [dialogAberto, setDialogAberto] = useState(false)
  const [disponibilidadeParaEditar, setDisponibilidadeParaEditar] = useState<DisponibilidadeResumo | null>(null)

  const carregarDisponibilidades = useCallback(async () => {
    if (!accessToken || !profissionalId) {
      return
    }
    setErro(null)
    try {
      const resultado = await listarDisponibilidades(profissionalId, accessToken)
      setDisponibilidades(resultado)
    } catch (erroCapturado) {
      setErro(traduzirErroDeDisponibilidade(erroCapturado))
    }
  }, [accessToken, profissionalId])

  useEffect(() => {
    void carregarDisponibilidades()
  }, [carregarDisponibilidades])

  function abrirParaCriar() {
    setDisponibilidadeParaEditar(null)
    setDialogAberto(true)
  }

  function abrirParaEditar(disponibilidade: DisponibilidadeResumo) {
    setDisponibilidadeParaEditar(disponibilidade)
    setDialogAberto(true)
  }

  async function remover(disponibilidade: DisponibilidadeResumo) {
    if (!accessToken || !profissionalId) {
      return
    }
    try {
      await removerDisponibilidade(disponibilidade.disponibilidadeId, profissionalId, accessToken)
      await carregarDisponibilidades()
    } catch (erroCapturado) {
      setErro(traduzirErroDeDisponibilidade(erroCapturado))
    }
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <div className="flex flex-col gap-1">
          <h1 className="text-2xl font-semibold tracking-tight text-foreground">Disponibilidade</h1>
          <p className="text-sm text-muted-foreground">Configure as janelas de expediente do profissional.</p>
        </div>
        {profissionalId && (
          <Button onClick={abrirParaCriar}>
            <Plus />
            Nova disponibilidade
          </Button>
        )}
      </div>

      {(carregandoProfissional || disponibilidades === null) && !erro && !erroProfissional && <EstadoCarregando />}

      {(erro || erroProfissional) && (
        <EstadoErro mensagem={erro ?? erroProfissional!} aoTentarNovamente={carregarDisponibilidades} />
      )}

      {!erro && !erroProfissional && disponibilidades !== null && disponibilidades.length === 0 && (
        <EstadoVazio
          titulo="Nenhuma disponibilidade configurada"
          descricao="Configure ao menos uma janela de expediente para começar a receber agendamentos."
          acao={<Button onClick={abrirParaCriar}>Nova disponibilidade</Button>}
        />
      )}

      {!erro && !erroProfissional && disponibilidades !== null && disponibilidades.length > 0 && (
        <DisponibilidadeLista disponibilidades={disponibilidades} onEditar={abrirParaEditar} onRemover={remover} />
      )}

      {profissionalId && (
        <DisponibilidadeDialog
          profissionalId={profissionalId}
          disponibilidadeParaEditar={disponibilidadeParaEditar}
          open={dialogAberto}
          onOpenChange={setDialogAberto}
          onSalvo={carregarDisponibilidades}
        />
      )}
    </div>
  )
}
