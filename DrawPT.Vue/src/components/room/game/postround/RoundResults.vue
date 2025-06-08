<script setup lang="ts">
import PlayerResultCard from './PlayerResultCard.vue'
import { useRoomStore } from '@/stores/room'
import { usePlayerStore } from '@/stores/player'
import { useScoreboardStore } from '@/stores/scoreboard'

const { players } = useRoomStore()
const { player: you } = usePlayerStore()
const { lastRoundResults } = useScoreboardStore()

const isYou = (playerConnectionId: string) => playerConnectionId === you.connectionId
</script>

<template>
  <div class="border-grey-50 fixed bottom-4 left-4 right-4 top-4">
    <div className="flex flex-col items-center justify-center">
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
