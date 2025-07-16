import { defineStore } from 'pinia'
import api from '@/api/api'

const getInitialVolume = (key: string, defaultValue: number): number => {
  const storedValue = localStorage.getItem(key)
  if (storedValue !== null) {
    const parsedValue = parseInt(storedValue, 10)
    if (!isNaN(parsedValue) && parsedValue >= 0 && parsedValue <= 100) {
      return parsedValue
    }
  }
  return defaultValue
}

export const useVolumeStore = defineStore('volume', {
  state: () => ({
    musicVolume: getInitialVolume('musicVolume', 50),
    sfxVolume: getInitialVolume('sfxVolume', 75),
    modalOpen: false,
    musicUrl: null as string | null,
    sfxUrl: null as string | null,
    isPlayingMusic: false,
    isPlayingSfx: false,
    backgroundMusicList: [] as string[],
    currentTrackIndex: -1 as number
  }),
  getters: {
    musicVolumePercent: (state) => state.musicVolume / 100,
    sfxVolumePercent: (state) => state.sfxVolume / 100,
    isModalOpen: (state) => state.modalOpen,
    getCurrentMusicUrl: (state) => state.musicUrl,
    getCurrentSfxUrl: (state) => state.sfxUrl,
    getIsPlayingMusic: (state) => state.isPlayingMusic,
    getIsPlayingSfx: (state) => state.isPlayingSfx,
    getBackgroundMusicList: (state) => state.backgroundMusicList,
    getCurrentTrackIndex: (state) => state.currentTrackIndex
  },
  actions: {
    setMusicVolume(newVolume: number) {
      this.musicVolume = Math.max(0, Math.min(100, newVolume))
      localStorage.setItem('musicVolume', this.musicVolume.toString())
    },
    setSfxVolume(newVolume: number) {
      this.sfxVolume = Math.max(0, Math.min(100, newVolume))
      localStorage.setItem('sfxVolume', this.sfxVolume.toString())
    },
    toggleModal() {
      this.modalOpen = !this.modalOpen
    },
    setMusicUrl(newUrl: string | null) {
      this.musicUrl = newUrl
    },
    setSfxUrl(newUrl: string | null) {
      this.sfxUrl = newUrl
      this.isPlayingSfx = newUrl !== null
    },
    togglePlayMusic() {
      this.isPlayingMusic = !this.isPlayingMusic
    },
    stopMusic() {
      this.isPlayingMusic = false
      this.musicUrl = null
    },
    stopSfx() {
      this.isPlayingSfx = false
      this.sfxUrl = null
    },
    // Fetch and initialize background music list
    async loadBackgroundMusic() {
      try {
        const urls = await api.getBackgroundMusic()
        this.backgroundMusicList = urls
        // If no URL set yet, pick random initially
        if (urls.length > 0 && this.musicUrl == null) {
          const idx = Math.floor(Math.random() * urls.length)
          this.currentTrackIndex = idx
          this.musicUrl = urls[idx]
        }
      } catch (error) {
        console.error('Failed to load background music', error)
      }
    }
  }
})
