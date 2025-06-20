<script setup lang="ts">
import { storeToRefs } from 'pinia'
import type { PlayerResult } from '@/models/player'
import { usePlayerStore } from '@/stores/player'

const playerStore = usePlayerStore()
const { blankAvatar } = storeToRefs(playerStore)

const props = defineProps<{
  isYou?: boolean
  playerResult: PlayerResult
}>()
</script>

<template>
  <div class="mt-4 flex w-96 max-w-[70vw] items-center">
    <div class="flex w-full">
      <div class="mr-4 w-full cursor-pointer flex-col">
        <div
          class="relative z-10 ml-4 flex w-full items-center rounded-lg bg-gray-500/10 p-4 px-6 backdrop-blur dark:bg-white/10"
        >
          <div class="flex items-center space-x-2">
            <div
              class="flex h-10 w-10 cursor-default items-center justify-center rounded-full p-2 text-sm font-bold text-white"
              aria-label="avatar"
            >
              <img
                :src="props.playerResult.avatar ?? blankAvatar"
                :alt="props.playerResult.username"
                class="h-10 w-10 rounded-full object-cover"
              />
            </div>
            <span class="ml-12 text-sm font-medium">
              {{ playerResult.username }}
            </span>
          </div>
          <div class="ml-auto flex items-center font-semibold text-green-600 dark:text-green-500">
            <span>{{ playerResult.score }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
