<script setup lang="ts">
import { ref } from 'vue'
import { storeToRefs } from 'pinia'
import type { PlayerAnswer } from '@/models/gameModels'
import { usePlayerStore } from '@/stores/player'
import Avatar from '@/components/common/Avatar.vue'

const playerStore = usePlayerStore()
const { blankAvatar } = storeToRefs(playerStore)

const props = defineProps<{
  isYou?: boolean
  answer: PlayerAnswer
}>()

const showReason = ref(props.isYou)
</script>

<template>
  <div class="mt-4 flex w-[70vw] items-center">
    <div class="flex w-full">
      <div class="mr-4 w-full cursor-pointer flex-col" @click="showReason = !showReason">
        <div
          class="relative z-10 ml-4 flex w-full items-center rounded-lg bg-gray-500/10 p-4 px-6 backdrop-blur dark:bg-white/10"
        >
          <div class="flex items-center space-x-2">
            <div class="mr-2 flex cursor-default" aria-label="avatar">
              <Avatar
                :size="10"
                :username="answer.username"
                :avatar="answer.avatar || blankAvatar"
              />
            </div>
            <span class="text-sm font-medium">
              {{ answer.username }}
            </span>
          </div>
          <div class="ml-4 flex-grow p-1 px-6 text-left max-sm:hidden">
            {{ answer.guess }}
          </div>
          <div class="flex items-center max-sm:ml-4">
            <span>{{ answer.score }}</span>
            <span class="-mb-2 ml-1 text-xs">pts</span>
            <span
              v-if="answer.bonusPoints > 0"
              class="-mb-2 ml-1 animate-bounce font-semibold text-green-500 dark:text-green-400"
            >
              +{{ answer.bonusPoints }}
            </span>
          </div>
        </div>
        <div
          v-if="answer.reason"
          :class="[
            'ml-4 flex w-full rounded-lg bg-gray-500/10 p-4 px-6 backdrop-blur dark:bg-white/10',
            'relative z-0 transition-all duration-300 ease-in-out max-sm:hidden',
            showReason ? 'mt-1 opacity-100' : '-mt-10 opacity-40'
          ]"
        >
          {{ answer.reason }}
        </div>
      </div>
    </div>
  </div>
</template>
