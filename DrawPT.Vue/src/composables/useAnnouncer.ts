import { ref } from 'vue'

/**
 * Composable for managing and playing streamed audio chunks from the server.
 */
export function useAnnouncer() {
  const chunks = ref<Uint8Array[]>([])
  const isPlaying = ref(false)

  /**
   * Enqueue a new audio chunk received from the server.
   */
  function enqueueChunk(chunk: Uint8Array | number[]) {
    const data = Array.isArray(chunk) ? new Uint8Array(chunk) : chunk
    chunks.value.push(data)
  }

  /**
   * Called when the server signals that all audio chunks have been sent.
   * Assembles the chunks into a Blob and plays the resulting audio.
   */
  async function streamCompleted() {
    if (chunks.value.length === 0) return
    const blob = new Blob(chunks.value, { type: 'audio/mpeg' })
    const url = URL.createObjectURL(blob)
    const audio = new Audio(url)
    isPlaying.value = true
    try {
      await audio.play()
    } catch {
      console.warn('Failed to autoplay audio, user interaction may be required.')
    }
    audio.onended = () => {
      isPlaying.value = false
      URL.revokeObjectURL(url)
      chunks.value = []
    }
  }

  return { enqueueChunk, streamCompleted, isPlaying }
}
