import api from './axios'

export const aiChatService = {
  ask(message) {
    return api.post('/api/v1/AiChat/ask', { message })
  }
}
