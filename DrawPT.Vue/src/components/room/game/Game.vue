<script setup lang="ts">
import RoundResults from './postround/RoundResults.vue'
import GameBonusPoints from './midround/GameBonusPoints.vue'
import SelectTheme from './midround/SelectTheme.vue'
import ViewThemes from './midround/ViewThemes.vue'
import GameTimer from './midround/GameTimer.vue'
import ImageLoader from './midround/ImageLoader.vue'
import GuessInput from '@/components/common/GuessInput.vue'
import { onMounted, onUnmounted, ref, watchEffect } from 'vue'
import { useNotificationStore } from '@/stores/notifications'
import { useScoreboardStore } from '@/stores/scoreboard'
import service from '@/services/signalRService'

import type { PlayerAnswerBase, PlayerQuestion } from '@/models/gameModels'

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

function handleThemeSelected(newTheme: string) {
  themeSelection.value = newTheme
}

async function askForTheme(): Promise<string> {
  return new Promise((resolve, reject) => {
    if (themeTimeout.value) clearTimeout(themeTimeout.value) // Clear any existing timeout

    let stopEffect: (() => void) | null = null

    themeTimeout.value = setTimeout(() => {
      addGameNotification("Uh oh!  Time's up!", true)
      selectableThemeOptions.value = []
      if (stopEffect) stopEffect() // Stop the effect
      reject(new Error('Operation timed out'))
    }, timeoutForTheme)

    // themeSelection.value is reset to '' by service.on('askTheme',...) before this is called
    stopEffect = watchEffect(() => {
      const currentTheme = themeSelection.value
      if (currentTheme) {
        // Will trigger when themeSelection.value changes from '' to something
        console.log('Selected theme:', currentTheme)
        selectableThemeOptions.value = []
        clearTimeout(themeTimeout.value)
        if (stopEffect) stopEffect() // Stop the effect itself
        resolve(currentTheme)
      }
    })
  })
}

async function askQuestion(): Promise<PlayerAnswerBase> {
  guess.value = '' // Reset guess before starting to ensure watchEffect doesn't fire with stale data
  return new Promise((resolve, reject) => {
    if (questionTimeout.value) clearTimeout(questionTimeout.value) // Clear any existing timeout

    let stopEffect: (() => void) | null = null

    questionTimeout.value = setTimeout(() => {
      lockGuess.value = true
      addGameNotification("Uh oh!  Time's up!", true)
      if (stopEffect) stopEffect() // Stop the effect
      reject(new Error('Operation timed out'))
    }, timeoutPerQuestion)

    stopEffect = watchEffect(() => {
      const currentGuess = guess.value
      if (currentGuess) {
        // Will trigger when guess.value changes from '' to something
        const answer: PlayerAnswerBase = {
          guess: currentGuess,
          isGambling: false
        }
        console.log('Answer:', currentGuess) // Your console.log, now more reliable
        clearTimeout(questionTimeout.value)
        if (stopEffect) stopEffect() // Stop the effect itself
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

  service.on('askQuestion', async (question: PlayerQuestion): Promise<PlayerAnswerBase> => {
    console.log('askQuestion', question)
    showResults.value = false
    lockGuess.value = false
    imageUrl.value = question.imageUrl || ''
    setRound(question.roundNumber)
    const answer = await askQuestion()
    return answer
  })

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
  <main v-else class="flex min-h-screen w-full items-center justify-center">
    <div>
      <SelectTheme
        v-if="selectableThemeOptions.length > 0"
        :themes="selectableThemeOptions"
        @themeSelected="handleThemeSelected"
      />
      <ViewThemes v-else-if="themeOptions.length > 0" :themes="themeOptions" />

      <div>
        <ImageLoader v-if="imageUrl === '' && selectableThemeOptions.length === 0" />
        <img
          v-else-if="imageUrl !== ''"
          class="aspect-auto max-h-[720px] max-w-[1048px] object-contain"
          :src="imageUrl"
        />
      </div>
      <div v-if="!lockGuess">
        <GuessInput v-model="guessInput" :submitAction="submitGuess" />
      </div>
    </div>
  </main>
</template>
