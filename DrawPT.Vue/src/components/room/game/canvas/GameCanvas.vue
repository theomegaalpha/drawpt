<script setup lang="ts">
import GameBonusPoints from '../canvas/GameBonusPoints.vue'
import GameTimer from '../GameTimer.vue'
import GuessInput from '@/components/common/GuessInput.vue'
import { computed, onMounted, onUnmounted, ref, watchEffect } from 'vue'
import { useNotificationStore } from '@/stores/notifications'
import { useGameStateStore } from '@/stores/gameState'
import service from '@/services/signalRService'

import type { PlayerAnswerBase, PlayerQuestion } from '@/models/gameModels'

const gameStateStore = useGameStateStore()
const notificationStore = useNotificationStore()

// --- State from Pinia Store (via computed properties) ---
const imageUrl = computed(() => gameStateStore.currentImageUrl)
const lockGuess = computed(() => gameStateStore.isGuessLocked)
const bonusPoints = computed(() => gameStateStore.currentBonusPoints)

// --- Local state for UI interaction leading to promise resolution for SignalR ---
const currentGuessForPromise = ref<string>('') // User's current guess for askQuestionInternal promise
const questionTimeoutRef = ref<NodeJS.Timeout>()
const timeoutPerQuestion = 40000

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

onMounted(() => {
  service.on('askQuestion', async (question: PlayerQuestion): Promise<PlayerAnswerBase> => {
    console.log('askQuestion received from server:', question)
    gameStateStore.prepareForQuestion(question) // Prepare store state for the question
    // scoreboardStore.setRound(question.roundNumber); // This is now done inside prepareForQuestion action
    try {
      const answer = await askQuestionInternal()
      return answer // Return player's answer to server
    } catch (error) {
      console.error('Error in askQuestion process:', error)
      // Ensure a value is always returned
      return { guess: '', isGambling: false } // Default/fallback answer
    }
  })
})

onUnmounted(() => {
  service.off('askQuestion')
  if (questionTimeoutRef.value) clearTimeout(questionTimeoutRef.value)
})

// --- UI Action: Submitting a guess ---
const guessInputFromComponent = ref<string>('') // v-model for the GuessInput component

const submitGuess = async (valueFromInput: string) => {
  if (valueFromInput === '' || lockGuess.value) {
    // Check lockGuess from store
    return
  }
  gameStateStore.setGuessLock(true) // Lock guess in store
  currentGuessForPromise.value = valueFromInput // Set the value to resolve the askQuestionInternal promise
  guessInputFromComponent.value = '' // Clear the input field in UI
}
</script>

<template>
  <div class="flex min-h-screen w-full items-center justify-center">
    <GameTimer
      :max-time="timeoutPerQuestion"
      v-if="!lockGuess"
      class="fixed left-0 right-0 top-0"
    />
    <GameBonusPoints v-if="bonusPoints > 0" :points="bonusPoints" />
    <div class="flex w-full max-w-5xl flex-col items-center px-4">
      <div class="mb-2">
        <img
          v-if="imageUrl !== '' && !gameStateStore.shouldShowResults"
          class="aspect-auto max-h-[70vh] max-w-[1048px] object-contain"
          :src="imageUrl"
        />
      </div>
      <div v-if="!lockGuess">
        <GuessInput v-model="guessInputFromComponent" :submitAction="submitGuess" />
      </div>
    </div>
  </div>
</template>
