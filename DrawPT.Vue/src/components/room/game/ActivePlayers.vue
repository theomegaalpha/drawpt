<script setup lang="ts">
import { useGameStateStore } from '@/stores/gameState'
import { usePlayerStore } from '@/stores/player'

const { players } = defineProps<{
  players: Array<{ id: string; avatar?: string | null; username: string }>
}>()

const { playerAnswers } = useGameStateStore()
const { blankAvatar } = usePlayerStore()
</script>

<template>
  <div>
    <!-- Use a fixed height (e.g., 70vh to match the main image) so each row has space -->
    <ul class="-mx-4 grid h-full max-h-[70vh] w-fit grid-rows-4 justify-items-center md:mx-1">
      <li
        v-for="player in players"
        :key="player.id"
        class="relative -mx-4 flex h-full flex-none items-center md:mx-1"
      >
        <!-- Bonus points float animation -->
        <div
          v-if="playerAnswers.find((pa) => pa.id === player.id)?.bonusPoints"
          class="animate-float-up-fade absolute left-1/2 top-4 -mx-4 -translate-x-1/2 transform text-lg font-bold text-green-500"
        >
          +{{ playerAnswers.find((pa) => pa.id === player.id)?.bonusPoints }}
        </div>
        <img
          :src="player.avatar || blankAvatar"
          :alt="player.username"
          class="md:border-6 inline-block h-16 w-16 flex-shrink-0 rounded-full border-4 border-gray-400 filter md:h-20 md:w-20 dark:border-white"
          :class="{
            grayscale: !playerAnswers.some((pa) => pa.id === player.id),
            'animate-bulging [animation-iteration-count:1]': playerAnswers.some(
              (pa) => pa.id === player.id
            )
          }"
        />
      </li>
    </ul>
  </div>
</template>
