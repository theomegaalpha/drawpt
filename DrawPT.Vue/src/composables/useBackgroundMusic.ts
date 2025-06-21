import { ref, onUnmounted, watch } from 'vue'
import { useVolumeStore } from '@/stores/volume'

export function useBackgroundMusic() {
  const audio = ref<HTMLAudioElement | null>(null)
  const sfxAudio = ref<HTMLAudioElement | null>(null)
  const volumeStore = useVolumeStore()

  const cleanupSfxAudio = () => {
    if (sfxAudio.value) {
      sfxAudio.value.pause()
      sfxAudio.value.removeAttribute('src')
      sfxAudio.value.load()
      sfxAudio.value.onerror = null
      sfxAudio.value = null
    }
  }

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
            audio.value.loop = true

            // Set volume from store (volume watcher will also handle this, but good for initial setup)
            const currentVolumePercent = volumeStore.musicVolume
            const volumeNormalized = Math.max(0, Math.min(100, currentVolumePercent)) / 100
            audio.value.volume = volumeNormalized

            audio.value.onerror = (e) => {
              console.error('Audio element error:', e, 'URL:', url)
              if (volumeStore.isPlayingMusic) {
                volumeStore.togglePlayMusic() // Set isPlayingMusic to false via action
              }
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

  // Watch for SFX playback state and URL, mirror music logic
  watch(
    () => ({ isPlaying: volumeStore.isPlayingSfx, url: volumeStore.sfxUrl }),
    (newState) => {
      const { isPlaying, url } = newState
      if (isPlaying) {
        if (url) {
          if (!sfxAudio.value || sfxAudio.value.src !== url) {
            cleanupSfxAudio()
            sfxAudio.value = new Audio(url)
            const vol = Math.max(0, Math.min(100, volumeStore.sfxVolume)) / 100
            sfxAudio.value.volume = vol
            // When SFX ends, reset playback state
            sfxAudio.value.onended = () => {
              volumeStore.stopSfx()
            }
          }
          sfxAudio.value?.play().catch((error) => {
            console.error('Error playing SFX:', error, 'URL:', url)
            if (volumeStore.isPlayingSfx) volumeStore.stopSfx()
          })
        } else {
          cleanupSfxAudio()
          if (volumeStore.isPlayingSfx) volumeStore.stopSfx()
        }
      } else {
        if (sfxAudio.value && !sfxAudio.value.paused) {
          sfxAudio.value.pause()
        }
      }
    },
    { deep: true, immediate: true }
  )

  // Watch for changes in SFX volume
  watch(
    () => volumeStore.sfxVolume,
    (newVol) => {
      if (sfxAudio.value) {
        sfxAudio.value.volume = Math.max(0, Math.min(100, newVol)) / 100
      }
    },
    { immediate: true }
  )

  const setVolume = (volumePercent: number) => {
    volumeStore.setMusicVolume(volumePercent)
  }

  onUnmounted(() => {
    cleanupAudio()
    volumeStore.setMusicUrl(null)
    if (volumeStore.isPlayingMusic) volumeStore.togglePlayMusic()
    cleanupSfxAudio()
    volumeStore.stopSfx()
  })

  return {
    setVolume
  }
}
