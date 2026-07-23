import { Plus } from "lucide-react"
import { useCallback, useEffect, useState } from "react"

import { listarServicos, traduzirErroDeServico, type ServicoResumo } from "@/api/servicos"
import { EstadoCarregando } from "@/components/estado/EstadoCarregando"
import { EstadoErro } from "@/components/estado/EstadoErro"
import { EstadoVazio } from "@/components/estado/EstadoVazio"
import { ServicoDialog } from "@/components/servicos/ServicoDialog"
import { ServicosLista } from "@/components/servicos/ServicosLista"
import { Button } from "@/components/ui/button"
import { useAuth } from "@/hooks/useAuth"
import { useProfissionalAtual } from "@/hooks/useProfissionalAtual"

export function ServicosPage() {
  const { accessToken } = useAuth()
  const { profissionalId, carregando: carregandoProfissional, erro: erroProfissional } = useProfissionalAtual()
  const [servicos, setServicos] = useState<ServicoResumo[] | null>(null)
  const [erro, setErro] = useState<string | null>(null)
  const [dialogAberto, setDialogAberto] = useState(false)
  const [servicoParaEditar, setServicoParaEditar] = useState<ServicoResumo | null>(null)

  const carregarServicos = useCallback(async () => {
    if (!accessToken || !profissionalId) {
      return
    }
    setErro(null)
    try {
      const resultado = await listarServicos(profissionalId, accessToken)
      setServicos(resultado)
    } catch (erroCapturado) {
      setErro(traduzirErroDeServico(erroCapturado))
    }
  }, [accessToken, profissionalId])

  useEffect(() => {
    void carregarServicos()
  }, [carregarServicos])

  function abrirParaCriar() {
    setServicoParaEditar(null)
    setDialogAberto(true)
  }

  function abrirParaEditar(servico: ServicoResumo) {
    setServicoParaEditar(servico)
    setDialogAberto(true)
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center justify-between">
        <div className="flex flex-col gap-1">
          <h1 className="text-2xl font-semibold tracking-tight text-foreground">Serviços</h1>
          <p className="text-sm text-muted-foreground">Cadastre e atualize os serviços oferecidos.</p>
        </div>
        {profissionalId && (
          <Button onClick={abrirParaCriar}>
            <Plus />
            Novo serviço
          </Button>
        )}
      </div>

      {(carregandoProfissional || servicos === null) && !erro && !erroProfissional && <EstadoCarregando />}

      {(erro || erroProfissional) && (
        <EstadoErro mensagem={erro ?? erroProfissional!} aoTentarNovamente={carregarServicos} />
      )}

      {!erro && !erroProfissional && servicos !== null && servicos.length === 0 && (
        <EstadoVazio
          titulo="Nenhum serviço cadastrado"
          descricao="Cadastre o primeiro serviço para começar a receber agendamentos."
          acao={<Button onClick={abrirParaCriar}>Novo serviço</Button>}
        />
      )}

      {!erro && !erroProfissional && servicos !== null && servicos.length > 0 && (
        <ServicosLista servicos={servicos} onEditar={abrirParaEditar} />
      )}

      {profissionalId && (
        <ServicoDialog
          profissionalId={profissionalId}
          servicoParaEditar={servicoParaEditar}
          open={dialogAberto}
          onOpenChange={setDialogAberto}
          onSalvo={carregarServicos}
        />
      )}
    </div>
  )
}
