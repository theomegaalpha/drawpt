// src/composables/useAudioPlayer.ts
import { ref, onMounted, onBeforeUnmount } from 'vue'
import { AudioPlayerService } from '@/services/audioPlayerService'

export function useAudioPlayer(sampleRate = 24000) {
  const player = ref(new AudioPlayerService())
  // initialize right away (handles non-Vue usage like signal events)
  player.value.init(sampleRate)

  onMounted(() => {
    player.value.init(sampleRate)
  })

  onBeforeUnmount(() => {
    player.value.destroy()
  })

  function playBase64(base64: string) {
    // buffer incoming Opus chunk
    player.value.appendOpusChunk(base64)
    player.value.playOpusStream()
  }

  function stop() {
    player.value.stop()
  }

  function streamCompleted() {
    player.value.playOpusStream()
  }

  return { play: playBase64, stop, streamCompleted }
}
