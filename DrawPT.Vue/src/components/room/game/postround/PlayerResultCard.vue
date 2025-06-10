<script setup lang="ts">
import type { PlayerAnswer } from '@/models/gameModels'
import { ref, computed } from 'vue'
const props = defineProps<{
  isYou?: boolean
  answer: PlayerAnswer
}>()

const showReason = ref(props.isYou)

const initials = computed(() => {
  const names = props.answer.username?.split(' ')
  if (!names) return ''
  let initials = names[0].substring(0, 1).toUpperCase()

  if (names.length > 1) {
    initials += names[names.length - 1].substring(0, 1).toUpperCase()
  }

  return initials
})
const avatarColor = computed(() => {
  return '#' + Math.floor(Math.random() * 16777215).toString(16)
})
</script>

<template>
  <div class="mt-4 flex w-[70vw] items-center">
    <div class="flex w-full">
      <div class="mr-4 w-full cursor-pointer flex-col" @click="showReason = !showReason">
        <div
          class="relative z-10 ml-4 flex w-full items-center rounded-lg bg-gray-500/10 p-4 px-6 backdrop-blur dark:bg-white/10"
        >
          <div class="flex items-center space-x-2">
            <div
              class="flex h-10 w-10 cursor-default items-center justify-center rounded-full p-2 text-sm font-bold text-white"
              :style="{ backgroundColor: avatarColor }"
              aria-label="avatar"
            >
              {{ initials }}
            </div>
            <span class="text-sm font-medium">
              {{ answer.username }}
            </span>
          </div>
          <div class="ml-4 flex-grow p-1 px-6 text-left">
            {{ answer.guess }}
          </div>
          <div class="flex items-center">
            <span>{{ answer.score }}</span>
            <span class="-mb-2 ml-1 text-xs">pts</span>
            <span
              v-if="answer.bonusPoints > 0"
              class="ml-1 font-semibold text-green-500 dark:text-green-400"
            >
              +{{ answer.bonusPoints }}
            </span>
          </div>
        </div>
        <div
          v-if="answer.reason"
          :class="[
            'ml-4 flex w-full rounded-lg bg-gray-500/10 p-4 px-6 backdrop-blur dark:bg-white/10',
            'relative z-0 transition-all duration-300 ease-in-out',
            showReason ? 'mt-1 opacity-100' : '-mt-10 opacity-40'
          ]"
        >
          {{ answer.reason }}
        </div>
      </div>
    </div>
  </div>
</template>
