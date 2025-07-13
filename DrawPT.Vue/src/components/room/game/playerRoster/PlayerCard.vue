<script setup lang="ts">
import { storeToRefs } from 'pinia'
import type { Player } from '@/models/player'
import { usePlayerStore } from '@/stores/player'
import Avatar from '@/components/common/Avatar.vue'

const playerStore = usePlayerStore()
const { blankAvatar } = storeToRefs(playerStore)

defineProps<{
  isYou?: boolean
  player: Player
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
      </div>
    </div>
  </div>
</template>
