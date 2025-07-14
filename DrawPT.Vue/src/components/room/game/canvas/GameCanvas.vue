<script setup lang="ts">
import GameBonusPoints from '../canvas/GameBonusPoints.vue'
import GameTimer from '../GameTimer.vue'
import GuessInput from '@/components/common/GuessInput.vue'
import { computed, ref } from 'vue'
import { useGameStateStore } from '@/stores/gameState'
import ShinyButton from '@/components/common/ShinyButton.vue'
import ActivePlayers from '../ActivePlayers.vue'

const timeoutPerQuestion = 30000
const emit = defineEmits<{
  (e: 'guessSubmitted', guess: string): void
}>()

const gameStateStore = useGameStateStore()

const imageUrl = computed(() => gameStateStore.currentImageUrl)
const lockGuess = computed(() => gameStateStore.isGuessLocked)
const bonusPoints = computed(() => gameStateStore.currentBonusPoints)

const guessInputFromComponent = ref<string>('')

const submitGuess = async (valueFromInput: string) => {
  if (valueFromInput === '' || lockGuess.value) {
    return
  }
  emit('guessSubmitted', valueFromInput)
  guessInputFromComponent.value = ''
}
</script>

<template>
  <div class="flex min-h-screen w-full items-center justify-center">
    <GameTimer
      :max-time="timeoutPerQuestion"
      v-if="!lockGuess"
      class="fixed bottom-0 left-0 right-0"
    />
    <GameBonusPoints v-if="bonusPoints > 0" :points="bonusPoints" />
    <div class="flex w-fit max-w-5xl flex-col items-center px-4">
      <ShinyButton class="-mb-12 cursor-default sm:mb-4" :disabled="true">
        <h2 class="sm:text-2xl">Theme: {{ gameStateStore.currentTheme }}</h2>
      </ShinyButton>
      <div class="mb-2 flex items-start justify-center">
        <div class="relative inline-block">
          <!-- Left players inline -->
          <ActivePlayers
            class="absolute right-full top-1/2 h-full -translate-y-1/2 transform"
            :players="gameStateStore.players"
            :is-guess-locked="lockGuess"
          />
          <img
            v-if="imageUrl !== '' && !gameStateStore.shouldShowResults"
            class="max-h-[70vh] rounded-lg object-contain"
            :src="imageUrl"
          />
          <!-- Right players overlay -->
          <ActivePlayers
            class="absolute left-full top-1/2 h-full -translate-y-1/2 transform"
            :players="gameStateStore.players"
            :is-guess-locked="lockGuess"
          />
        </div>
      </div>
      <div v-if="!lockGuess" class="w-full max-w-2xl">
        <GuessInput v-model="guessInputFromComponent" :submitAction="submitGuess" />
      </div>
    </div>
  </div>
</template>
