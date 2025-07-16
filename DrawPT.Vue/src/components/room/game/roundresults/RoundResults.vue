<script setup lang="ts">
import { ref, onMounted } from 'vue'
import PlayerResultCard from './PlayerResultCard.vue'
import { usePlayerStore } from '@/stores/player'
import { useGameStateStore } from '@/stores/gameState'

const { lastRoundResults, players } = useGameStateStore()
const { player: you } = usePlayerStore()

const isYou = (playerConnectionId: string) => playerConnectionId === you.connectionId

// Animation state for image opacity
const faded = ref(false)
onMounted(() => {
  setTimeout(() => {
    faded.value = true
  }, 100)
})
</script>

<template>
  <div class="fixed inset-4 overflow-hidden">
    <img
      v-if="lastRoundResults.question.imageUrl !== ''"
      class="absolute left-1/2 top-1/2 z-0 max-h-[70vh] max-w-[1048px] -translate-x-1/2 -translate-y-1/2 rounded-lg transition-opacity delay-100 duration-1000"
      :class="faded ? 'opacity-30' : 'opacity-100'"
      :src="lastRoundResults.question.imageUrl"
    />
    <div class="relative z-10 flex flex-col items-center justify-center py-8">
      <div class="mb-12 flex flex-col items-center max-sm:hidden">
        <h2
          class="rounded-lg border border-black/30 bg-gray-500/10 p-4 px-6 text-lg backdrop-blur dark:border-white/20 dark:bg-white/10"
        >
          {{ lastRoundResults.question.originalPrompt }}
        </h2>
        <div class="relative mb-12 w-[40rem]">
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
      </div>
      <PlayerResultCard
        v-for="(answer, index) in lastRoundResults.answers"
        :answer="answer"
        :username="players.find((p) => p.connectionId === answer.connectionId)?.username"
        :isYou="isYou(answer.connectionId)"
        :key="index"
      />
    </div>
  </div>
</template>
