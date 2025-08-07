<script setup lang="ts">
import GameCanvas from './canvas/GameCanvas.vue'
import RoundResults from './roundresults/RoundResults.vue'
import GambleResults from './gambleResults/GambleResults.vue'
import SelectTheme from './themescreen/SelectTheme.vue'
import ViewThemes from './themescreen/ViewThemes.vue'
import AskGamble from './askGamble/AskGamble.vue'
import PlayerPrompt from './playerPrompt/PlayerPrompt.vue'
import ImageLoader from './loader/ImageLoader.vue'
import { computed, onBeforeMount, onUnmounted, ref, watchEffect } from 'vue'
import { useNotificationStore } from '@/stores/notifications'
import { useGameStateStore } from '@/stores/gameState'
import { usePlayerStore } from '@/stores/player'
import service from '@/services/signalRService'
import type {
  GameGamble,
  GameQuestion,
  PlayerAnswerBase,
  PlayerQuestion
} from '@/models/gameModels'

const gameStateStore = useGameStateStore()
const notificationStore = useNotificationStore()
const playerStore = usePlayerStore()

// --- State from Pinia Store (via computed properties) ---
const themes = computed(() => gameStateStore.themes)
const lockGuess = computed(() => gameStateStore.isGuessLocked)
const bonusPoints = computed(() => gameStateStore.currentBonusPoints)

// --- Local state for UI interaction leading to promise resolution for SignalR ---
const imagePromptInput = ref<string>('')
const themeSelectionInput = ref<string>('') // User's current theme choice from UI to resolve askForThemeInternal
const currentGuessForPromise = ref<string>('') // User's current guess for askQuestionInternal promise
const gambleInput = ref<GameGamble>({
  gamblerId: playerStore.player.id,
  playerId: '',
  isHigh: true,
  createdAt: new Date().toISOString(),
  score: 0,
  bonusPoints: 0
})

// --- Refs for timeout management for interactive promises ---
const questionTimeoutRef = ref<NodeJS.Timeout>()
const themeTimeoutRef = ref<NodeJS.Timeout>()
const promptTimeoutRef = ref<NodeJS.Timeout>()

// --- Constants for timeouts (consider moving to a config or gameStateStore if they vary) ---
const timeoutPerQuestion = computed(
  () => gameStateStore.gameConfiguration.questionTimeout * 1000 || 40000
)
const timeoutForTheme = computed(
  () => gameStateStore.gameConfiguration.themeTimeout * 1000 || 30000
)
const timeoutForPrompt = computed(
  () => gameStateStore.gameConfiguration.promptTimeout * 1000 || 90000
)

// --- Method to handle theme selection from the SelectTheme component ---
function handleThemeSelectedFromUI(newTheme: string) {
  themeSelectionInput.value = newTheme // This will trigger the watchEffect in askForThemeInternal
}

function handlePromptSubmitted(prompt: string) {
  imagePromptInput.value = prompt
}

const handleGuessSubmitted = async (valueFromInput: string) => {
  if (valueFromInput === '' || lockGuess.value) {
    return
  }
  gameStateStore.setGuessLock(true)
  currentGuessForPromise.value = valueFromInput
  guessInputFromComponent.value = ''
}

const handleGambleSubmitted = async (gamble: GameGamble) => {
  gambleInput.value.isHigh = gamble.isHigh
  gambleInput.value.playerId = gamble.playerId
}

// --- Internal promise-based function for prompt selection ---
async function askForPromptInternal(): Promise<string> {
  return new Promise((resolve, reject) => {
    if (promptTimeoutRef.value) clearTimeout(promptTimeoutRef.value)
    let stopEffect: (() => void) | null = null

    promptTimeoutRef.value = setTimeout(() => {
      notificationStore.addGameNotification("Uh oh! Prompt creation time's up!", true)
      if (stopEffect) stopEffect()
      reject(new Error('Prompt creation timed out'))
    }, timeoutForPrompt.value)

    stopEffect = watchEffect(() => {
      const currentPrompt = imagePromptInput.value
      if (currentPrompt) {
        gameStateStore.playerSubmittedPrompt()
        clearTimeout(promptTimeoutRef.value)
        if (stopEffect) stopEffect() // Stop this effect itself
        resolve(currentPrompt)
      }
    })
  })
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
    }, timeoutForTheme.value)

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
    }, timeoutPerQuestion.value)

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

async function askForGambleInternal(): Promise<GameGamble> {
  gambleInput.value.playerId = ''
  gambleInput.value.isHigh = true
  return new Promise((resolve, reject) => {
    if (questionTimeoutRef.value) clearTimeout(questionTimeoutRef.value)
    let stopEffect: (() => void) | null = null

    questionTimeoutRef.value = setTimeout(() => {
      gameStateStore.setGuessLock(true)
      notificationStore.addGameNotification("Uh oh! Question time's up!", true)
      if (stopEffect) stopEffect()
      reject(new Error('Question timed out'))
    }, timeoutPerQuestion.value)

    stopEffect = watchEffect(() => {
      if (gambleInput.value.playerId) {
        console.log('new gambleInput', gambleInput.value)
        const result: GameGamble = { ...gambleInput.value }
        clearTimeout(questionTimeoutRef.value)
        if (stopEffect) stopEffect()
        resolve(result)
      }
    })
  })
}

onBeforeMount(() => {
  // Interactive SignalR handlers that expect a return value
  service.on('askPrompt', async () => {
    imagePromptInput.value = ''
    gameStateStore.prepareForPlayerImagePrompt()
    try {
      const prompt = await askForPromptInternal()
      return prompt
    } catch (error) {
      console.error('Error in askForPrompt process:', error)
      return ''
    }
  })

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

  service.on('askGamble', async (question: GameQuestion) => {
    gambleInput.value.playerId = ''
    gambleInput.value.isHigh = true
    gameStateStore.prepareForPlayerGamble(question)
    try {
      const gamble = await askForGambleInternal()
      return gamble
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
  service.off('askImagePrompt')
  service.off('askTheme')
  service.off('askQuestion')
  service.off('askGamble')

  // Clear any active timeouts
  if (themeTimeoutRef.value) clearTimeout(themeTimeoutRef.value)
  if (questionTimeoutRef.value) clearTimeout(questionTimeoutRef.value)
})

// --- UI Action: Submitting a guess ---
const guessInputFromComponent = ref<string>('')
</script>

<template>
  <ImageLoader v-if="gameStateStore.showImageLoader" />
  <div v-else>
    <PlayerPrompt
      v-if="gameStateStore.askingImagePrompt"
      @promptSubmitted="handlePromptSubmitted"
    />
    <SelectTheme
      v-if="gameStateStore.areThemesSelectable"
      :themes="themes"
      @themeSelected="handleThemeSelectedFromUI"
    />
    <AskGamble
      v-if="gameStateStore.askingGamble"
      :gamble="gambleInput"
      @gambleSubmitted="handleGambleSubmitted"
    />
    <ViewThemes v-if="gameStateStore.areThemesVisible" :themes="themes" />
    <GameCanvas v-if="gameStateStore.showGameCanvas" @guessSubmitted="handleGuessSubmitted" />
    <RoundResults v-if="gameStateStore.shouldShowResults" />
    <GambleResults v-if="gameStateStore.shouldShowGambleResults" />
  </div>
</template>
