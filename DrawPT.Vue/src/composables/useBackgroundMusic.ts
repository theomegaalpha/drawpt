import { ref, onUnmounted, watch } from 'vue'
import { useVolumeStore } from '@/stores/volumeStore'

export function useBackgroundMusic() {
  const audio = ref<HTMLAudioElement | null>(null)
  const volumeStore = useVolumeStore()

  const loadAndPlayMusic = (url: string, initialVolumePercent?: number) => {
    if (audio.value) {
      audio.value.pause()
    }
    audio.value = new Audio(url)
    // Use provided initial volume or fall back to store's current music volume
    setVolume(initialVolumePercent !== undefined ? initialVolumePercent : volumeStore.musicVolume)
    audio.value.loop = true

    audio.value.play().catch((error) => {
      console.error('Error attempting to play background music from URL:', url, error)
    })
  }

  const setVolume = (volumePercent: number) => {
    const volumeNormalized = Math.max(0, Math.min(100, volumePercent)) / 100
    if (audio.value) {
      audio.value.volume = volumeNormalized
    }
    // Update the store; the store expects a value between 0-100
    if (volumeStore.musicVolume !== volumePercent) {
      volumeStore.setMusicVolume(volumePercent)
    }
  }

  // Watch for changes in the store's musicVolume and update the audio element
  watch(
    () => volumeStore.musicVolume,
    (newVolumePercent) => {
      if (audio.value) {
        const volumeNormalized = Math.max(0, Math.min(100, newVolumePercent)) / 100
        audio.value.volume = volumeNormalized
      }
    }
  )

  const pauseMusic = () => {
    if (audio.value && !audio.value.paused) {
      audio.value.pause()
    }
  }

  const resumeMusic = () => {
    if (audio.value && audio.value.paused) {
      audio.value.play().catch((error) => console.error('Error resuming music:', error))
    }
  }

  const stopMusicAndCleanup = () => {
    if (audio.value) {
      audio.value.pause()
      audio.value.src = ''
      audio.value.load()
      audio.value = null
    }
  }

  // Cleanup when the component using this composable is unmounted
  onUnmounted(() => {
    stopMusicAndCleanup()
  })

  return {
    loadAndPlayMusic,
    setVolume,
    pauseMusic,
    resumeMusic,
    stopMusicAndCleanup
  }
}
