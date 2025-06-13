<script setup lang="ts">
import { onMounted } from 'vue'
import { useScoreboardStore } from '@/stores/scoreboard'
import { usePlayerStore } from '@/stores/player'
import { useVolumeStore } from '@/stores/volumeStore'
import PlayerResultCard from './PlayerResultCard.vue'
import confetti from 'canvas-confetti'

const scoreboardStore = useScoreboardStore()
const { setSfxUrl, stopMusic } = useVolumeStore()
const { gameResults } = scoreboardStore
const { player: you } = usePlayerStore()

var duration = gameResults.playerResults.length * 2000
var end = Date.now() + duration

function frame() {
  // launch a few confetti from the left edge
  confetti({
    particleCount: 7,
    angle: 60,
    spread: 55,
    origin: { x: 0 }
  })
  // and launch a few from the right edge
  confetti({
    particleCount: 7,
    angle: 120,
    spread: 55,
    origin: { x: 1 }
  })

  // keep going until the time is up
  if (Date.now() < end) {
    requestAnimationFrame(frame)
  }
}

onMounted(() => {
  setSfxUrl('/sounds/victory.mp3')
  stopMusic()
})

onMounted(() => {
  // start the animation
  frame()
})
</script>

<template>
  <div class="relative z-10 flex h-full flex-col items-center justify-center py-8">
    <transition-group tag="div" name="slide-up" class="mb-12 flex flex-col items-center" appear>
      <!-- It's better to use a unique id from the result if available -->
      <div
        v-for="(result, index) in gameResults.playerResults.sort((a, b) => b.score - a.score)"
        :key="result.id"
        :style="{ transitionDelay: `${(gameResults.playerResults.length - 1 - index) * 1800}ms` }"
      >
        <PlayerResultCard
          :is-you="result.connectionId === you.connectionId"
          :player-result="result"
        />
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
