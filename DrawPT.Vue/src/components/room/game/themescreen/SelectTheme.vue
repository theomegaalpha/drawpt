<script setup lang="ts">
import GameTimer from '../GameTimer.vue'
import { computed, onMounted, onUnmounted, ref, watchEffect } from 'vue'
import { useNotificationStore } from '@/stores/notifications'
import { useGameStateStore } from '@/stores/gameState' // Import the new store
import service from '@/services/signalRService'

const timeoutForTheme = 20000
const props = defineProps({
  themes: {
    type: Array<string>,
    required: true
  }
})
const gameStateStore = useGameStateStore()
const animationsReadyToPlay = ref(false)

onMounted(() => {
  requestAnimationFrame(() => {
    animationsReadyToPlay.value = true
  })
})

const notificationStore = useNotificationStore()

const selectableThemeOptions = computed(() => gameStateStore.selectableThemeOptions)
const themeSelectionInput = ref<string>('')

// --- Refs for timeout management for interactive promises ---
const themeTimeoutRef = ref<NodeJS.Timeout>()

function handleThemeSelected(newTheme: string) {
  themeSelectionInput.value = newTheme
  console.log('theme selection:', newTheme)
}

// --- Internal promise-based function for theme selection ---
async function askForThemeInternal(): Promise<string> {
  return new Promise((resolve, reject) => {
    if (themeTimeoutRef.value) clearTimeout(themeTimeoutRef.value)
    let stopEffect: (() => void) | null = null

    themeTimeoutRef.value = setTimeout(() => {
      notificationStore.addGameNotification("Uh oh! Theme selection time's up!", true)
      gameStateStore.clearSelectableThemes() // Clear options in store
      if (stopEffect) stopEffect()
      reject(new Error('Theme selection timed out'))
    }, timeoutForTheme)

    stopEffect = watchEffect(() => {
      console.log('watchEffect triggered for theme selection:', themeSelectionInput.value)
      const currentTheme = themeSelectionInput.value
      if (currentTheme) {
        gameStateStore.playerSelectedTheme(currentTheme)
        gameStateStore.clearSelectableThemes()
        clearTimeout(themeTimeoutRef.value)
        if (stopEffect) stopEffect()
        resolve(currentTheme)
      }
    })
  })
}

onMounted(() => {
  // Interactive SignalR handlers that expect a return value
  service.on('askTheme', async (themes: string[]) => {
    themeSelectionInput.value = '' // Reset local UI state for theme selection
    gameStateStore.prepareForThemeSelection(themes) // Prepare store state for theme selection
    try {
      const theme = await askForThemeInternal()
      return theme // Return selected theme to server
    } catch (error) {
      console.error('Error in askForTheme process:', error)
      // Ensure a value is always returned to the server, even on error/timeout
      return '' // Or a specific "timeout" string if the server expects it
    }
  })
})

onUnmounted(() => {
  service.off('askTheme')

  // Clear any active timeouts
  if (themeTimeoutRef.value) clearTimeout(themeTimeoutRef.value)
})
</script>

<template>
  <div class="flex min-h-screen flex-col items-center justify-center text-center text-xl font-bold">
    <GameTimer
      :max-time="timeoutForTheme"
      v-if="selectableThemeOptions.length > 0"
      class="fixed left-0 right-0 top-0"
    />
    <h1 class="text-2xl font-bold">Select a Theme</h1>
    <div class="relative mb-16 w-[40rem]">
      <div
        class="absolute inset-x-20 top-0 h-[2px] w-3/4 bg-gradient-to-r from-transparent via-indigo-500 to-transparent blur-sm"
      />
      <div
        class="absolute inset-x-20 top-0 h-px w-3/4 bg-gradient-to-r from-transparent via-indigo-500 to-transparent"
      />
      <div
        class="absolute inset-x-60 top-0 h-[5px] w-1/4 bg-gradient-to-r from-transparent via-sky-500 to-transparent blur-sm"
      />
      <div
        class="absolute inset-x-60 top-0 h-px w-1/4 bg-gradient-to-r from-transparent via-sky-500 to-transparent"
      />
    </div>
    <div v-for="(theme, index) in props.themes" :key="index" @click="handleThemeSelected(theme)">
      <div
        class="animate-blur-in cursor-default"
        :style="{
          animationDelay: `${index * 50}ms`,
          animationPlayState: animationsReadyToPlay ? 'running' : 'paused',
          animationFillMode: 'backwards'
        }"
      >
        <div
          class="bg-surface-default shimmer-glow mb-4 animate-blur-in cursor-pointer rounded-lg px-9 py-2"
          :style="{
            animationDelay: `${index * 50}ms`,
            animationPlayState: animationsReadyToPlay ? 'running' : 'paused',
            animationFillMode: 'backwards'
          }"
        >
          {{ theme }}
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.shimmer-glow {
  text-decoration: none;
  border: 1px solid rgb(146, 148, 248);
  position: relative;
  overflow: hidden;
}

.shimmer-glow:hover {
  box-shadow: 1px 1px 5px 2px rgba(146, 148, 248, 0.4);
}

.shimmer-glow:before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(120deg, transparent, rgba(146, 148, 248, 0.4), transparent);
  transition: all 150ms;
}

.shimmer-glow:hover:before {
  left: 100%;
}
</style>
