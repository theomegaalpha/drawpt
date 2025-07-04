import signalRService from '@/services/signalRService'
import { useAudioPlayer } from '@/composables/useAudioPlayer'

// Singleton announcer instance to buffer and play audio chunks
const announcer = useAudioPlayer()

export function registerAudioEvents() {
  // Stream incoming audio chunks into announcer
  signalRService.on('receiveAudio', (chunk: string) => {
    console.debug('[AudioEvent] receiveAudio - chunk length:', chunk.length)
    announcer.play(chunk)
  })
  // Play assembled audio when stream completes
  signalRService.on('audioStreamCompleted', () => {
    console.debug('[AudioEvent] audioStreamCompleted - invoking streamCompleted')
    announcer.streamCompleted()
  })
}

export function unregisterAudioEvents() {
  signalRService.off('receiveAudio')
  signalRService.off('audioStreamCompleted')
}
