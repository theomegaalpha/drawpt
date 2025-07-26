<script setup lang="ts">
import GameCanvas from './canvas/GameCanvas.vue'
import RoundResults from './roundresults/RoundResults.vue'
import SelectTheme from './themescreen/SelectTheme.vue'
import ViewThemes from './themescreen/ViewThemes.vue'
import ImageLoader from './loader/ImageLoader.vue'
import { computed, onBeforeMount, onUnmounted, ref, watchEffect } from 'vue'
import { useNotificationStore } from '@/stores/notifications'
import { useGameStateStore } from '@/stores/gameState'
import service from '@/services/signalRService'

import type { PlayerAnswerBase, PlayerQuestion } from '@/models/gameModels'

const gameStateStore = useGameStateStore()
const notificationStore = useNotificationStore()

// --- State from Pinia Store (via computed properties) ---
const themes = computed(() => gameStateStore.themes)
const lockGuess = computed(() => gameStateStore.isGuessLocked)
const bonusPoints = computed(() => gameStateStore.currentBonusPoints)

// --- Local state for UI interaction leading to promise resolution for SignalR ---
const themeSelectionInput = ref<string>('') // User's current theme choice from UI to resolve askForThemeInternal
const currentGuessForPromise = ref<string>('') // User's current guess for askQuestionInternal promise

// --- Refs for timeout management for interactive promises ---
const questionTimeoutRef = ref<NodeJS.Timeout>()
const themeTimeoutRef = ref<NodeJS.Timeout>()

// --- Constants for timeouts (consider moving to a config or gameStateStore if they vary) ---
const timeoutPerQuestion = 40000
const timeoutForTheme = 30000

// --- Method to handle theme selection from the SelectTheme component ---
function handleThemeSelectedFromUI(newTheme: string) {
  themeSelectionInput.value = newTheme // This will trigger the watchEffect in askForThemeInternal
}

// --- Internal promise-based function for theme selection ---
async function askForThemeInternal(): Promise<string> {
  return new Promise((resolve, reject) => {
    if (themeTimeoutRef.value) clearTimeout(themeTimeoutRef.value)
    let stopEffect: (() => void) | null = null

    themeTimeoutRef.value = setTimeout(() => {
      notificationStore.addGameNotification("Uh oh! Theme selection time's up!", true)
      gameStateStore.clearThemes()
      if (stopEffect) stopEffect()
      reject(new Error('Theme selection timed out'))
    }, timeoutForTheme)

    stopEffect = watchEffect(() => {
      const currentTheme = themeSelectionInput.value
      if (currentTheme) {
        gameStateStore.playerSelectedTheme(currentTheme) // Action in store to clear selectable themes
        gameStateStore.clearThemes() // Or directly clear here
        clearTimeout(themeTimeoutRef.value)
        if (stopEffect) stopEffect() // Stop this effect itself
        resolve(currentTheme)
      }
    })
  })
}

// --- Internal promise-based function for asking a question ---
async function askQuestionInternal(): Promise<PlayerAnswerBase> {
  currentGuessForPromise.value = '' // Reset guess before starting
  return new Promise((resolve, reject) => {
    if (questionTimeoutRef.value) clearTimeout(questionTimeoutRef.value)
    let stopEffect: (() => void) | null = null

    questionTimeoutRef.value = setTimeout(() => {
      gameStateStore.setGuessLock(true) // Lock guess in store
      notificationStore.addGameNotification("Uh oh! Question time's up!", true)
      if (stopEffect) stopEffect()
      reject(new Error('Question timed out'))
    }, timeoutPerQuestion)

    stopEffect = watchEffect(() => {
      const guess = currentGuessForPromise.value
      if (guess) {
        const answer: PlayerAnswerBase = {
          guess: guess,
          isGambling: false // TODO: Implement gambling logic if needed
        }
        clearTimeout(questionTimeoutRef.value)
        if (stopEffect) stopEffect() // Stop this effect itself
        resolve(answer)
      }
    })
  })
}

onBeforeMount(() => {
  // Interactive SignalR handlers that expect a return value
  service.on('askTheme', async (themes: string[]) => {
    themeSelectionInput.value = ''
    gameStateStore.prepareForThemeSelection(themes)
    try {
      const theme = await askForThemeInternal()
      return theme
    } catch (error) {
      console.error('Error in askForTheme process:', error)
      return ''
    }
  })

  service.on('askQuestion', async (question: PlayerQuestion): Promise<PlayerAnswerBase> => {
    gameStateStore.prepareForQuestion(question)
    try {
      const answer = await askQuestionInternal()
      return answer
    } catch (error) {
      console.error('Error in askQuestion process:', error)
      return { guess: '', isGambling: false }
    }
  })

  watchEffect(() => {
    if (bonusPoints.value > 0) {
      setTimeout(() => {
        gameStateStore.clearBonusPointsDisplay()
      }, 20000)
    }
  })
})

onUnmounted(() => {
  service.off('askTheme')
  service.off('askQuestion')

  // Clear any active timeouts
  if (themeTimeoutRef.value) clearTimeout(themeTimeoutRef.value)
  if (questionTimeoutRef.value) clearTimeout(questionTimeoutRef.value)
})

// --- UI Action: Submitting a guess ---
const guessInputFromComponent = ref<string>('')

const submitGuess = async (valueFromInput: string) => {
  if (valueFromInput === '' || lockGuess.value) {
    // Check lockGuess from store
    return
  }
  gameStateStore.setGuessLock(true)
  currentGuessForPromise.value = valueFromInput
  guessInputFromComponent.value = ''
}
</script>

<template>
  <ImageLoader v-if="gameStateStore.showImageLoader" />
  <div v-else>
    <RoundResults v-if="gameStateStore.shouldShowResults" />
    <SelectTheme
      v-if="gameStateStore.areThemesSelectable"
      :themes="themes"
      @themeSelected="handleThemeSelectedFromUI"
    />
    <ViewThemes v-if="gameStateStore.areThemesVisible" :themes="themes" />
    <GameCanvas v-if="gameStateStore.showGameCanvas" @guessSubmitted="submitGuess" />
  </div>
</template>
