import api from './axios'

export const aiChatService = {
  /**
   * El timeout global de la instancia son 15 s (ver ./axios), pensado para CRUD.
   * Una respuesta del modelo tarda habitualmente más que eso, y al saltar el
   * timeout axios aborta con ECONNABORTED — que es indistinguible de un fallo
   * real desde la vista. Por eso esta llamada lleva su propio margen.
   */
  ask(message) {
    return api.post('/api/v1/AiChat/ask', { message }, { timeout: 120000 })
  }
}
