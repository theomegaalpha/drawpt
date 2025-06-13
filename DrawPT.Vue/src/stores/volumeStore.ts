import { defineStore } from 'pinia'

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
    isPlayingMusic: false
  }),
  getters: {
    musicVolumePercent: (state) => state.musicVolume / 100,
    sfxVolumePercent: (state) => state.sfxVolume / 100,
    isModalOpen: (state) => state.modalOpen,
    getCurrentMusicUrl: (state) => state.musicUrl,
    getIsPlayingMusic: (state) => state.isPlayingMusic
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
    togglePlayMusic() {
      this.isPlayingMusic = !this.isPlayingMusic
    }
  }
})
