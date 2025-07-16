// src/composables/useAudioPlayer.ts
import { ref, onMounted, onBeforeUnmount, watch } from 'vue'
import { AudioPlayerService } from '@/services/audioPlayerService'
import { useAudioStore } from '@/stores/audio'

export function useAudioPlayer(sampleRate = 24000) {
  const player = ref(new AudioPlayerService())
  player.value.init(sampleRate)
  const audioStore = useAudioStore()
  player.value.setVolume(audioStore.announcerVolume / 100)

  watch(
    () => audioStore.announcerVolume,
    (newVol) => {
      player.value.setVolume(newVol / 100)
    }
  )

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
    console.log('stopping audio')
  }

  function streamCompleted() {
    player.value.playOpusStream()
  }

  return { play: playBase64, stop, streamCompleted }
}
