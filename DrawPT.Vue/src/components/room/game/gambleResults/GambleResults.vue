<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { usePlayerStore } from '@/stores/player'
import { useGameStateStore } from '@/stores/gameState'
import PlayerResultCard from '@/components/common/PlayerResultCard.vue'

const { lastRoundResults, players } = useGameStateStore()
const { player: you } = usePlayerStore()

const isYou = (playerConnectionId: string) => playerConnectionId === you.connectionId

const winningAnswer = computed(() => {
  return lastRoundResults.answers
    .slice()
    .sort((a, b) => b.score + b.bonusPoints - (a.score + a.bonusPoints))[0]
})

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
      <PlayerResultCard
        :answer="winningAnswer"
        :username="players.find((p) => p.connectionId === winningAnswer.connectionId)?.username"
        :isYou="isYou(winningAnswer.connectionId)"
        :key="winningAnswer.id"
      />
    </div>
  </div>
</template>
