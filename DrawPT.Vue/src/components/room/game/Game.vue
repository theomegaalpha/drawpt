<script setup lang="ts">
import RoundResults from './postround/RoundResults.vue'
import GameBonusPoints from './midround/GameBonusPoints.vue'
import SelectTheme from './midround/SelectTheme.vue'
import ViewThemes from './midround/ViewThemes.vue'
import GameTimer from './midround/GameTimer.vue'
import ImageLoader from './midround/ImageLoader.vue'
import { onMounted, onUnmounted, ref, watch } from 'vue'
import { useNotificationStore } from '@/stores/notifications'
import { useScoreboardStore } from '@/stores/scoreboard'
import service from '@/services/signalRService'
import { useSpeechRecognition } from '@/composables/useSpeechRecognition' // Added import

import type { GameAnswerBase } from '@/models/gameModels'

const { addRoundResult, setRound } = useScoreboardStore()
const { addGameNotification } = useNotificationStore()
const guess = ref<string>('')
const guessInput = ref<string>('')
const themeOptions = ref<string[]>([])
const selectableThemeOptions = ref<string[]>([])
const themeSelection = ref<string>('')
const showResults = ref<boolean>(false)

const bonusPoints = ref<number>(0)
const imageUrl = ref<string>('')
const lockGuess = ref<boolean>(true)
const timeoutPerQuestion = 40000
const questionTimeout = ref<NodeJS.Timeout>()
const timeoutForTheme = 20000
const themeTimeout = ref<NodeJS.Timeout>()

// Speech Recognition integration
const { transcribedText, isListening, toggleListening } = useSpeechRecognition()

watch(transcribedText, (newText) => {
  if (newText) {
    guessInput.value += newText
  }
})

const handleRecordButtonMouseDown = () => {
  if (!isListening.value) {
    toggleListening()
  }
}

const handleRecordButtonMouseUp = () => {
  if (isListening.value) {
    toggleListening()
  }
}

async function askForTheme(): Promise<string> {
  return new Promise((resolve, reject) => {
    themeTimeout.value = setTimeout(() => {
      addGameNotification("Uh oh!  Time's up!", true)
      selectableThemeOptions.value = []
      reject(new Error('Operation timed out'))
    }, timeoutForTheme)

    watch(themeSelection, (newValue) => {
      if (newValue) {
        selectableThemeOptions.value = []
        clearTimeout(themeTimeout.value)
        resolve(newValue)
      }
    })
  })
}

async function askQuestion(): Promise<GameAnswerBase> {
  return new Promise((resolve, reject) => {
    questionTimeout.value = setTimeout(() => {
      lockGuess.value = true
      addGameNotification("Uh oh!  Time's up!", true)
      reject(new Error('Operation timed out'))
    }, timeoutPerQuestion)

    watch(guess, (newValue) => {
      if (newValue) {
        const answer: GameAnswerBase = {
          guess: newValue,
          isGambling: false
        }
        clearTimeout(questionTimeout.value)
        resolve(answer)
      }
    })
  })
}

onMounted(() => {
  service.on('themeSelection', (themes: string[]) => {
    imageUrl.value = ''
    showResults.value = false
    themeOptions.value = themes
  })

  service.on('themeSelected', (theme: string) => {
    themeOptions.value = []
    addGameNotification('Selected theme: ' + theme)
  })

  service.on('askTheme', async (themes: string[]) => {
    imageUrl.value = ''
    showResults.value = false
    themeSelection.value = ''
    selectableThemeOptions.value = themes
    const theme = await askForTheme()
    return theme
  })

  service.on(
    'askQuestion',
    async (currentRound: number, newImageUrl: string): Promise<GameAnswerBase> => {
      showResults.value = false
      lockGuess.value = false
      imageUrl.value = newImageUrl
      setRound(currentRound)
      const answer = await askQuestion()
      return answer
    }
  )

  service.on('broadcastRoundResults', (gameRound) => {
    addRoundResult(gameRound)
    showResults.value = true
  })

  service.on('awardBonusPoints', (points: number) => {
    bonusPoints.value = points
    setTimeout(() => {
      bonusPoints.value = 0
    }, 20000)
  })
})

onUnmounted(() => {
  service.off('themeSelection')
  service.off('themeSelected')
  service.off('askTheme')
  service.off('askQuestion')
  service.off('broadcastRoundResults')
  service.off('awardBonusPoints')

  clearTimeout(themeTimeout.value)
  clearTimeout(questionTimeout.value)
})

const submitGuess = async (value: string) => {
  if (value === '') {
    return
  }
  lockGuess.value = true
  guess.value = value
  guessInput.value = ''
}
</script>

<template>
  <GameTimer
    :max-time="timeoutForTheme"
    v-if="selectableThemeOptions.length > 0"
    class="fixed left-0 right-0 top-0"
  />
  <GameTimer :max-time="timeoutPerQuestion" v-if="!lockGuess" class="fixed left-0 right-0 top-0" />
  <GameBonusPoints v-if="bonusPoints > 0" :points="bonusPoints" />
  <RoundResults v-if="showResults" />
  <main v-else class="mt-10 flex w-full items-center justify-center">
    <div
      class="flex max-w-[1048px] flex-col rounded-lg border border-gray-200 bg-white p-6 shadow dark:border-gray-700 dark:bg-gray-800"
    >
      <SelectTheme
        v-if="selectableThemeOptions.length > 0"
        :themes="selectableThemeOptions"
        @themeSelected="(value: string) => (themeSelection = value)"
      />
      <ViewThemes v-else-if="themeOptions.length > 0" :themes="themeOptions" />

      <div
        class="mt-2 flex flex-col rounded-lg border border-gray-200 bg-white p-2 shadow dark:border-gray-700 dark:bg-gray-800"
      >
        <ImageLoader v-if="imageUrl === '' && selectableThemeOptions.length === 0" />
        <img
          v-else-if="imageUrl !== ''"
          class="aspect-auto max-h-[720px] max-w-[1048px] object-contain"
          :src="imageUrl"
        />
      </div>
      <div
        v-if="!lockGuess"
        class="mt-2 flex items-center rounded-lg border border-gray-200 bg-white p-2 shadow dark:border-gray-700 dark:bg-gray-800"
      >
        <input
          class="flex-grow rounded border border-gray-300 px-2 py-1 text-black shadow-inner"
          type="text"
          v-model="guessInput"
          @keyup.enter="submitGuess(guessInput)"
        />
        <button
          class="ml-2 rounded border border-blue-700 bg-blue-500 px-4 py-2 hover:bg-blue-700 hover:disabled:bg-blue-500"
          :disabled="guessInput === ''"
          @click="submitGuess(guessInput)"
        >
          Submit
        </button>
        <button
          class="ml-2 rounded border border-green-700 bg-green-500 px-4 py-2 hover:bg-green-700"
          @mousedown="handleRecordButtonMouseDown"
          @mouseup="handleRecordButtonMouseUp"
        >
          🎤
        </button>
      </div>
    </div>
  </main>
</template>
