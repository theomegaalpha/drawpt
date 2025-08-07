<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { usePlayerStore } from '@/stores/player'
import { useGameStateStore } from '@/stores/gameState'
import confetti from 'canvas-confetti'

const { gambler, gamblePlayer, lastRoundResults, gambleResults, players } = useGameStateStore()
const { blankAvatar } = usePlayerStore()

// derive high/low for styling
const isHigh = computed(() => gambleResults.isHigh)
// Adjust wording when more than two players: 'the highest'/'the lowest'
const highLowText = computed(() => {
  if (players.length > 2) {
    return isHigh.value ? 'the highest' : 'the lowest'
  }
  return isHigh.value ? 'Above' : 'Below'
})
const highLowClass = computed(() => (isHigh.value ? 'text-green-500' : 'text-red-500'))
// Animation state for image opacity
const faded = ref(false)
const showGambler = ref(false)
const showPlayer = ref(false)
const showChoice = ref(false)
onMounted(() => {
  setTimeout(() => {
    faded.value = true
  }, 100)
  setTimeout(() => {
    showGambler.value = true
  }, 2000)
  setTimeout(() => {
    showPlayer.value = true
  }, 4000)
  setTimeout(() => {
    showChoice.value = true
  }, 6000)

  setTimeout(() => {
    confetti({
      spread: 60,
      origin: {
        x: Math.random(),
        y: Math.random() - 0.2
      }
    })
  }, 6500)
  setTimeout(() => {
    confetti({
      spread: 80,
      origin: {
        x: Math.random(),
        y: Math.random()
      }
    })
  }, 7000)
})
</script>

<template>
  <div class="relative flex h-screen flex-col items-center justify-center overflow-y-auto">
    <img
      v-if="lastRoundResults.question.imageUrl !== ''"
      class="absolute left-1/2 top-1/2 z-0 max-h-[70vh] max-w-[1048px] -translate-x-1/2 -translate-y-1/2 rounded-lg transition-opacity duration-1000"
      :class="faded ? 'opacity-10' : 'opacity-100'"
      :src="lastRoundResults.question.imageUrl"
    />
    <div class="relative z-10 flex flex-col items-center justify-center py-8 text-center">
      <h1 class="text-2xl font-bold">Gamble Results</h1>
      <div class="mt-8 space-y-2 text-lg font-medium">
        <div class="flex items-center space-x-2 rounded-lg bg-black/10 p-4 dark:bg-white/10">
          <img
            class="h-8 w-8 rounded-full transition-opacity duration-1000"
            :class="showGambler ? 'opacity-100' : 'opacity-0'"
            :src="gambler.avatar || blankAvatar"
            :alt="gambler.username"
          />
          <span
            class="transition-opacity duration-1000"
            :class="showGambler ? 'opacity-100' : 'opacity-0'"
            >{{ gambler.username }}</span
          >
        </div>
        <div>gambled that</div>
        <div class="flex items-center space-x-2 rounded-lg bg-black/10 p-4 dark:bg-white/10">
          <img
            class="h-8 w-8 rounded-full transition-opacity duration-1000"
            :class="showPlayer ? 'opacity-100' : 'opacity-0'"
            :src="gamblePlayer.avatar || blankAvatar"
            :alt="gamblePlayer.username"
          />
          <span
            class="transition-opacity duration-1000"
            :class="showPlayer ? 'opacity-100' : 'opacity-0'"
            >{{ gamblePlayer.username }}</span
          >
        </div>
        <div>would score</div>
        <div
          class="flex items-center justify-center space-x-2 rounded-lg bg-black/10 p-4 dark:bg-white/10"
        >
          <span
            class="transition-opacity duration-1000"
            :class="['font-bold', highLowClass, showChoice ? 'opacity-100' : 'opacity-0']"
            >{{ highLowText }}</span
          >
        </div>
        <div v-if="players.length <= 2">60 Points</div>
      </div>
    </div>
  </div>
</template>
