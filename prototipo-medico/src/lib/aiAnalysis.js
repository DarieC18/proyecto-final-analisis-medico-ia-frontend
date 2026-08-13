/**
 * Parseo de la respuesta de IA.
 *
 * `aiResponse` llega como texto: normalmente un JSON con
 * `{ summary, riskLevel, recommendations: [{ title, description }] }`,
 * a veces envuelto en una valla ```json``` porque así lo devuelve el modelo.
 * Si no es JSON válido, se devuelve null y quien llama muestra el texto crudo
 * como respaldo.
 */
export function parseAiResponse(raw) {
  if (!raw) return null
  try {
    const cleaned = raw
      .trim()
      .replace(/^```json\s*/i, '')
      .replace(/^```\s*/, '')
      .replace(/\s*```$/, '')
    return JSON.parse(cleaned)
  } catch {
    return null
  }
}

export const isAiError = (raw) => !!raw?.startsWith('ERROR')

const RISK_TONES = { Bajo: 'success', Medio: 'warning', Alto: 'danger' }
export const riskTone = (level) => RISK_TONES[level] ?? 'secondary'
