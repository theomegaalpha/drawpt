import { ref, onUnmounted, watch, computed } from 'vue'
import { useAudioStore } from '@/stores/audio'

export function useBackgroundMusic() {
  const audio = ref<HTMLAudioElement | null>(null)
  const volumeStore = useAudioStore()

  // Background music list from store
  const backgroundMusicList = computed(() => volumeStore.backgroundMusicList)

  const cleanupAudio = () => {
    if (audio.value) {
      audio.value.pause()
      audio.value.removeAttribute('src') // Clear the source
      audio.value.load() // Abort current network activity and reset element
      audio.value.onerror = null // Remove previous error handler
      audio.value = null
    }
  }

  // Watch for changes in playback state (isPlayingMusic) and music URL from the store
  watch(
    () => ({ isPlaying: volumeStore.isPlayingMusic, url: volumeStore.musicUrl }),
    (newState) => {
      const { isPlaying, url } = newState

      if (isPlaying) {
        if (url) {
          // If audio needs to be (re)loaded (no current audio, or URL has changed)
          if (!audio.value || audio.value.src !== url) {
            cleanupAudio() // Clean up any existing audio instance
            audio.value = new Audio(url)
            // Configure loop or shuffle on end based on shuffleMode flag
            if (volumeStore.shuffleMode) {
              audio.value.loop = false
              audio.value.onended = () => {
                volumeStore.shuffleMusic()
              }
            } else {
              audio.value.loop = true
              audio.value.onended = null
            }
            // Set initial volume
            const volumeNormalized = Math.max(0, Math.min(100, volumeStore.musicVolume)) / 100
            audio.value.volume = volumeNormalized
            // Error handling
            audio.value.onerror = (e) => {
              console.error('Audio element error:', e, 'URL:', url)
              if (volumeStore.isPlayingMusic) volumeStore.togglePlayMusic()
            }
          }

          // Attempt to play if audio is loaded and currently paused
          if (audio.value && audio.value.paused) {
            audio.value
              .play()
              .then(() => {
                // Play started successfully
              })
              .catch((error) => {
                console.error('Error attempting to play audio:', error, 'URL:', url)
                if (volumeStore.isPlayingMusic) {
                  volumeStore.togglePlayMusic() // Set isPlayingMusic to false if play fails
                }
              })
          }
        } else {
          console.warn(
            'Play music requested (isPlayingMusic is true), but no musicUrl is set in the store.'
          )
          cleanupAudio()
          if (volumeStore.isPlayingMusic) {
            volumeStore.togglePlayMusic()
          }
        }
      } else {
        // isPlaying is false
        if (audio.value && !audio.value.paused) {
          audio.value.pause()
        }
        // If isPlaying is false and URL changes, audio will be loaded if/when isPlaying becomes true.
        // If isPlaying is false and URL is removed, and music was playing, it would have been paused here.
      }
    },
    { deep: true, immediate: true } // immediate: true to handle initial store state on mount
  )

  // Watch for changes in the store's musicVolume and update the audio element
  watch(
    () => volumeStore.musicVolume,
    (newVolumePercent) => {
      if (audio.value) {
        const volumeNormalized = Math.max(0, Math.min(100, newVolumePercent)) / 100
        audio.value.volume = volumeNormalized
      }
    },
    { immediate: true } // Set initial volume when composable is used
  )

  const setVolume = (volumePercent: number) => {
    volumeStore.setMusicVolume(volumePercent)
  }

  onUnmounted(() => {
    cleanupAudio()
    volumeStore.setMusicUrl(null)
    if (volumeStore.isPlayingMusic) volumeStore.togglePlayMusic()
  })

  return {
    setVolume,
    backgroundMusicList
  }
}
