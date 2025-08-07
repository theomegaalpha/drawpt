import { ref, watch, onBeforeUnmount } from 'vue'
import { useAudioStore } from '@/stores/audio'

export function useSfxAudio() {
  const audio = ref<HTMLAudioElement | null>(null)
  const audioStore = useAudioStore()

  const cleanupAudio = () => {
    if (audio.value) {
      audio.value.pause()
      audio.value.removeAttribute('src')
      audio.value.load()
      audio.value.onerror = null
      audio.value = null
    }
  }

  watch(
    () => ({ url: audioStore.sfxUrl, playing: audioStore.isPlayingSfx }),
    ({ url, playing }) => {
      if (playing) {
        if (url) {
          cleanupAudio()
          audio.value = new Audio(url)
          // set SFX volume
          const volume = Math.max(0, Math.min(100, audioStore.sfxVolume)) / 100
          audio.value.volume = volume
          // when ends, reset store
          audio.value.onended = () => {
            audioStore.stopSfx()
          }
          audio.value.onerror = (e) => {
            console.error('SFX audio error:', e, 'URL:', url)
            audioStore.stopSfx()
          }
          audio.value.play().catch((error) => {
            console.error('Error playing SFX:', error, 'URL:', url)
            audioStore.stopSfx()
          })
        } else {
          console.warn('isPlayingSfx is true but no sfxUrl is set')
          cleanupAudio()
          audioStore.stopSfx()
        }
      } else {
        if (audio.value && !audio.value.paused) {
          audio.value.pause()
        }
      }
    },
    { immediate: true, deep: true }
  )

  onBeforeUnmount(() => {
    cleanupAudio()
    if (audioStore.isPlayingSfx) {
      audioStore.stopSfx()
    }
  })
}
