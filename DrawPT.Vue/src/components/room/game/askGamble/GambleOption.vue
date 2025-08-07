<script setup lang="ts">
import { storeToRefs } from 'pinia'
import type { Player } from '@/models/player'
import type { GameGamble } from '@/models/gameModels'
import { usePlayerStore } from '@/stores/player'
import Avatar from '@/components/common/Avatar.vue'

const playerStore = usePlayerStore()
const { blankAvatar } = storeToRefs(playerStore)

const props = defineProps<{ isDuelMode: boolean; player: Player; gamble: GameGamble }>()
const emit = defineEmits<{ (e: 'gambleSubmitted', gamble: GameGamble): void }>()

function onSelect(isHigh: boolean) {
  emit('gambleSubmitted', {
    ...props.gamble,
    playerId: props.player.id,
    isHigh
  })
}
</script>

<template>
  <div class="mt-4 flex w-96 max-w-[70vw] items-center">
    <div class="flex w-full">
      <div class="mr-4 w-full cursor-pointer flex-col">
        <div
          v-if="!isDuelMode"
          class="relative z-10 ml-4 flex w-full cursor-pointer items-center rounded-lg bg-gray-500/10 p-4 px-6 backdrop-blur hover:bg-gray-500/20 dark:bg-white/10 dark:hover:bg-white/20"
          :class="{
            'bg-gray-500/15 outline outline-2 outline-black/50 dark:bg-white/15 dark:outline-white/50':
              gamble.playerId === player.id
          }"
          @click="onSelect(true)"
        >
          <div class="flex items-center space-x-2">
            <div class="mr-2 flex cursor-default" aria-label="avatar">
              <Avatar
                :size="10"
                :username="player.username"
                :avatar="player.avatar || blankAvatar"
              />
            </div>
            <span class="ml-12 text-sm font-medium">
              {{ player.username }}
            </span>
          </div>
        </div>
        <div class="space-y-2" v-else>
          <div
            @click="onSelect(true)"
            class="relative z-10 ml-4 flex w-full cursor-pointer items-center rounded-lg bg-gray-500/10 p-4 px-6 backdrop-blur hover:bg-gray-500/20 dark:bg-white/10 dark:hover:bg-white/20"
            :class="{
              'bg-gray-500/15 outline outline-2 outline-black/50 dark:bg-white/15 dark:outline-white/50':
                gamble.isHigh
            }"
          >
            <div class="flex items-center space-x-2">
              <span class="mr-2 text-green-500">Above</span> 60 pts
            </div>
          </div>
          <div
            @click="onSelect(false)"
            class="relative z-10 ml-4 flex w-full cursor-pointer items-center rounded-lg bg-gray-500/10 p-4 px-6 backdrop-blur hover:bg-gray-500/15 dark:bg-white/10 dark:hover:bg-white/15"
            :class="{
              'bg-gray-500/15 outline outline-2 outline-black/50 dark:bg-white/15 dark:outline-white/50':
                !gamble.isHigh
            }"
          >
            <div class="flex items-center space-x-2">
              <span class="mr-2 text-red-500">Below</span> 60 pts
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
