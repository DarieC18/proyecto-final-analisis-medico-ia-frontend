import { nextTick, onBeforeUnmount, ref, watch } from 'vue'

/**
 * Posiciona un panel flotante contra su disparador.
 *
 * Existe por un motivo concreto: un popup en `position: absolute` lo recorta
 * cualquier ancestro con `overflow: hidden` — y FormSection lo lleva (lo
 * necesita para que el fondo de su cabecera respete las esquinas redondeadas).
 * El calendario dentro de un FormSection salía cortado por abajo.
 *
 * La solución es sacar el panel del flujo con <Teleport to="body"> y situarlo
 * en `position: fixed` con coordenadas calculadas. Eso lo inmuniza además
 * frente a modales, contenedores con scroll y celdas de tabla.
 *
 * Devuelve el estilo a aplicar en el panel y `update()` para recalcular.
 */

const GAP = 4 // separación entre disparador y panel
const MARGIN = 8 // aire mínimo contra el borde del viewport

export function usePopover(triggerRef, panelRef, open, options = {}) {
  const { matchWidth = false, minWidth = 0 } = options
  const style = ref({})

  async function update() {
    if (!open.value) return
    const trigger = triggerRef.value
    if (!trigger) return

    // El panel se mide una vez pintado; sin esto, en la primera apertura
    // `offsetHeight` es 0 y el volteo nunca se activaría.
    await nextTick()
    const panel = panelRef.value
    const rect = trigger.getBoundingClientRect()
    const vw = window.innerWidth
    const vh = window.innerHeight

    const panelH = panel?.offsetHeight ?? 0
    const panelW = matchWidth ? rect.width : Math.max(panel?.offsetWidth ?? 0, minWidth)

    const espacioAbajo = vh - rect.bottom - GAP - MARGIN
    const espacioArriba = rect.top - GAP - MARGIN

    // Sólo se voltea si abajo no cabe Y arriba hay más sitio: si no, abrir
    // hacia arriba en una pantalla baja empeoraría el recorte.
    const arriba = panelH > espacioAbajo && espacioArriba > espacioAbajo

    const top = arriba ? Math.max(MARGIN, rect.top - GAP - panelH) : rect.bottom + GAP

    let left = rect.left
    if (left + panelW > vw - MARGIN) left = vw - MARGIN - panelW
    if (left < MARGIN) left = MARGIN

    style.value = {
      position: 'fixed',
      top: `${top}px`,
      left: `${left}px`,
      maxHeight: `${Math.max(160, arriba ? espacioArriba : espacioAbajo)}px`,
      ...(matchWidth ? { width: `${rect.width}px` } : {})
    }
  }

  // `capture: true` para enterarse también del scroll de ancestros, que no
  // burbujea hasta window.
  function bind() {
    window.addEventListener('scroll', update, true)
    window.addEventListener('resize', update)
  }
  function unbind() {
    window.removeEventListener('scroll', update, true)
    window.removeEventListener('resize', update)
  }

  watch(open, (abierto) => {
    if (abierto) {
      update()
      bind()
    } else {
      unbind()
    }
  })

  onBeforeUnmount(unbind)

  return { style, update }
}
