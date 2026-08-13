import { FileImage, FileText, FileType2 } from 'lucide-vue-next'

/**
 * Icono y tono según la extensión del archivo.
 *
 * Antes esto era una función `fileIcon()` que devolvía emojis ('📕' para PDF,
 * '🖼️' para imagen, '📄' para el resto), con cuerpo idéntico duplicado en
 * AppointmentDetailView y MedicalDocumentsView.
 *
 * lucide no tiene un icono específico de PDF, así que el formato se distingue
 * por el TONO del IconTile, no por la silueta.
 */

const IMAGE_EXTENSIONS = ['jpg', 'jpeg', 'png', 'webp', 'gif', 'bmp']

const extensionOf = (name) => name?.split('.').pop()?.toLowerCase() ?? ''

export function fileIconFor(name) {
  const ext = extensionOf(name)
  if (ext === 'pdf') return FileType2
  if (IMAGE_EXTENSIONS.includes(ext)) return FileImage
  return FileText
}

export function fileToneFor(name) {
  const ext = extensionOf(name)
  if (ext === 'pdf') return 'danger'
  if (IMAGE_EXTENSIONS.includes(ext)) return 'info'
  return 'neutral'
}

export function isImageFile(name) {
  return IMAGE_EXTENSIONS.includes(extensionOf(name))
}

export function isPdfFile(name) {
  return extensionOf(name) === 'pdf'
}
