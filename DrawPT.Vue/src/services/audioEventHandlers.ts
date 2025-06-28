import signalRService from '@/services/signalRService'
import { useAnnouncer } from '@/composables/useAnnouncer'

// Singleton announcer instance to buffer and play audio chunks
const announcer = useAnnouncer()

export function registerAudioEvents() {
  // Stream incoming audio chunks into announcer
  signalRService.on('receiveAudio', (chunk: Uint8Array | number[]) => {
    announcer.enqueueChunk(chunk)
  })
  // Play assembled audio when stream completes
  signalRService.on('audioStreamCompleted', () => {
    announcer.streamCompleted()
  })
}

export function unregisterAudioEvents() {
  signalRService.off('receiveAudio')
  signalRService.off('audioStreamCompleted')
}
