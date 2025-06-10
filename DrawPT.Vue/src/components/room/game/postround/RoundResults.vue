<script setup lang="ts">
import { ref, onMounted } from 'vue'
import PlayerResultCard from './PlayerResultCard.vue'
import { useRoomStore } from '@/stores/room'
import { usePlayerStore } from '@/stores/player'
import { useScoreboardStore } from '@/stores/scoreboard'

const { players } = useRoomStore()
const { player: you } = usePlayerStore()
const { lastRoundResults } = useScoreboardStore()

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
      class="absolute left-1/2 top-1/2 z-0 max-h-[70vh] max-w-[1048px] -translate-x-1/2 -translate-y-1/2 transition-opacity delay-100 duration-1000"
      :class="faded ? 'opacity-50' : 'opacity-100'"
      :src="lastRoundResults.question.imageUrl"
    />
    <div class="relative z-10 flex flex-col items-center justify-center">
      <div
        class="mt-2 flex flex-col rounded-lg border border-gray-200 bg-white p-2 shadow dark:border-gray-700 dark:bg-gray-800"
      >
        <h2>Original Prompt: {{ lastRoundResults.question.originalPrompt }}</h2>
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
