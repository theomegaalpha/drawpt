<script setup lang="ts">
import { usePlayerStore } from '@/stores/player'
import { useGameStateStore } from '@/stores/gameState'
import PlayerCard from './PlayerCard.vue'

const gameStateStore = useGameStateStore()
const { players } = gameStateStore
const { player: you } = usePlayerStore()
</script>

<template>
  <div class="relative z-10 flex h-full flex-col items-center justify-center py-8">
    <transition-group tag="div" name="slide-up" class="mb-12 flex flex-col items-center" appear>
      <div
        v-for="(player, index) in players"
        :key="player.id"
        :style="{ transitionDelay: `${(players.length - 1 - index) * 1800 + 2000}ms` }"
      >
        <PlayerCard :is-you="player.connectionId === you.connectionId" :player="player" />
      </div>
    </transition-group>
  </div>
</template>

<style scoped>
.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.5s ease-out;
}
.slide-up-enter-from,
.slide-up-leave-to {
  opacity: 0;
  transform: translateY(30px);
}
</style>
