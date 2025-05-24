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
  <div class="fixed top-4 right-4 bottom-4 left-4 border-grey-50">
    <div className="flex flex-col items-center">
      <div
        class="flex flex-col mt-2 p-2 bg-white border border-gray-200 rounded-lg shadow dark:bg-gray-800 dark:border-gray-700"
      >
        <h2>Original Prompt: {{ lastRoundResults.question.originalPrompt }}</h2>
      </div>
      <PlayerResultCard
        v-for="(answer, index) in lastRoundResults.answers"
        :answer="answer"
        :username="players.find((p) => p.connectionId === answer.playerConnectionId)?.username"
        :isYou="isYou(answer.playerConnectionId)"
        :key="index"
      />
    </div>
  </div>
</template>
