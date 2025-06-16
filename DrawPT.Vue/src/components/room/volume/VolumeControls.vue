<script setup lang="ts">
import { Volume2 } from 'lucide-vue-next'
import { useVolumeStore } from '@/stores/volume'

import { ref } from 'vue'

const { sfxVolume, musicVolume, setSfxVolume, setMusicVolume } = useVolumeStore()
const isModalOpen = ref(false)
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
    <button @click="openModal" class="btn-default rounded px-4 py-2 text-white">
      <Volume2 :size="24" />
    </button>

    <div
      v-if="isModalOpen"
      class="fixed inset-0 z-50 flex h-full w-full items-center justify-center bg-black bg-opacity-50"
      @click.self="closeModal"
    >
      <div class="min-w-[300px] rounded-lg bg-white p-5 shadow-lg">
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
        <button @click="closeModal" class="btn-default rounded">Close</button>
      </div>
    </div>
  </div>
</template>
