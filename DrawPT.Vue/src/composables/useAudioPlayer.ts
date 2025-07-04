// src/composables/useAudioPlayer.ts
import { ref, onMounted, onBeforeUnmount } from 'vue'
import { AudioPlayerService } from '@/services/audioPlayerService'

export function useAudioPlayer(sampleRate = 24000) {
  const player = ref(new AudioPlayerService())

  onMounted(() => {
    player.value.init(sampleRate)
  })

  onBeforeUnmount(() => {
    player.value.destroy()
  })

  function playBase64(base64: string) {
    console.log('Playing audio:', base64)
    const bin = atob(base64)
    const u8 = Uint8Array.from(bin, (c) => c.charCodeAt(0))
    player.value.playBuffer(new Int16Array(u8.buffer))
  }

  function stop() {
    player.value.stop()
  }

  return { play: playBase64, stop }
}
