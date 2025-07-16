<script setup lang="ts">
import { Volume2 } from 'lucide-vue-next'
import { useAudioStore } from '@/stores/audio'

import { ref } from 'vue'

// destructure announcerVolume and its setter along with existing volumes
const {
  announcerVolume,
  sfxVolume,
  musicVolume,
  setAnnouncerVolume,
  setSfxVolume,
  setMusicVolume
} = useAudioStore()
const isModalOpen = ref(false)
const announcerVolumeRef = ref(announcerVolume)
const sfxVolumeRef = ref(sfxVolume)
const musicVolumeRef = ref(musicVolume)

const openModal = () => {
  isModalOpen.value = true
}
const closeModal = () => {
  isModalOpen.value = false
}
</script>

<template>
  <div>
    <button @click="openModal" class="btn-default text-color-default rounded px-4 py-2">
      <Volume2 :size="24" />
    </button>

    <div
      v-if="isModalOpen"
      class="fixed inset-0 z-50 flex h-full w-full items-center justify-center bg-black bg-opacity-50"
      @click.self="closeModal"
    >
      <div
        class="min-w-[300px] rounded-lg border border-black/10 bg-white p-5 shadow-lg dark:border-white/10 dark:bg-black"
      >
        <h2 class="mb-4 mt-0 text-xl font-semibold"></h2>
        <div class="mb-4">
          <label for="sfxVolume" class="mb-1 block text-sm font-medium text-gray-700"
            >Sound FX Volume:</label
          >
          <div class="flex items-center">
            <input
              type="range"
              id="sfxVolume"
              min="0"
              max="100"
              v-model="sfxVolumeRef"
              @input="setSfxVolume(sfxVolumeRef)"
              class="mr-2.5 h-2 w-full cursor-pointer appearance-none rounded-lg bg-gray-200 dark:bg-gray-700"
            />
            <span class="inline-block w-[40px] text-right text-sm text-gray-600"
              >{{ sfxVolumeRef }}%</span
            >
          </div>
        </div>
        <div class="mb-4">
          <label for="musicVolume" class="mb-1 block text-sm font-medium text-gray-700"
            >Background Music Volume:</label
          >
          <div class="flex items-center">
            <input
              type="range"
              id="musicVolume"
              min="0"
              max="100"
              v-model="musicVolumeRef"
              @input="setMusicVolume(musicVolumeRef)"
              class="mr-2.5 h-2 w-full cursor-pointer appearance-none rounded-lg bg-gray-200 dark:bg-gray-700"
            />
            <span class="inline-block w-[40px] text-right text-sm text-gray-600"
              >{{ musicVolumeRef }}%</span
            >
          </div>
        </div>
        <div class="mb-4">
          <label for="announcerVolume" class="mb-1 block text-sm font-medium text-gray-700"
            >Announcer Volume:</label
          >
          <div class="flex items-center">
            <input
              type="range"
              id="announcerVolume"
              min="0"
              max="100"
              v-model="announcerVolumeRef"
              @input="setAnnouncerVolume(announcerVolumeRef)"
              class="mr-2.5 h-2 w-full cursor-pointer appearance-none rounded-lg bg-gray-200 dark:bg-gray-700"
            />
            <span class="inline-block w-[40px] text-right text-sm text-gray-600"
              >{{ announcerVolumeRef }}%</span
            >
          </div>
        </div>
        <button @click="closeModal" class="btn-default rounded">Close</button>
      </div>
    </div>
  </div>
</template>
