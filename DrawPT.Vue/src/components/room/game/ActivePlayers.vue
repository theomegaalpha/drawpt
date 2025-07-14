<script setup lang="ts">
import { useGameStateStore } from '@/stores/gameState'
import { storeToRefs } from 'pinia'
import { usePlayerStore } from '@/stores/player'

const { players } = defineProps<{
  players: Array<{ id: string; avatar?: string | null; username: string }>
}>()

const gameStateStore = useGameStateStore()
const { playerAnswers } = storeToRefs(gameStateStore)
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
        <div class="relative h-16 w-16 flex-shrink-0 md:h-20 md:w-20">
          <div
            v-if="playerAnswers.find((pa) => pa.playerId === player.id)?.bonusPoints"
            class="absolute bottom-full left-1/2 -mb-1 -translate-x-1/2"
          >
            <div class="animate-float-up-fade mb-1 text-lg font-bold text-green-500">
              +{{ playerAnswers.find((pa) => pa.playerId === player.id)?.bonusPoints }}
            </div>
          </div>
          <img
            :src="player.avatar || blankAvatar"
            :alt="player.username"
            class="md:border-6 h-full w-full rounded-full border-4 border-gray-400 filter dark:border-white"
            :class="{
              'opacity-50 grayscale': !playerAnswers.some((pa) => pa.playerId === player.id),
              'animate-bulging opacity-100 [animation-iteration-count:1]': playerAnswers.some(
                (pa) => pa.playerId === player.id
              )
            }"
          />
        </div>
      </li>
    </ul>
  </div>
</template>
