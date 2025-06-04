<script setup lang="ts">
import type { PlayerAnswer } from '@/models/gameModels'
import { ref, computed } from 'vue'
const props = defineProps<{
  username?: string
  isYou?: boolean
  answer: PlayerAnswer
}>()

const showReason = ref(props.isYou)

const initials = computed(() => {
  const names = props.username?.split(' ')
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
  <div
    class="mt-4 flex w-[70vw] items-center justify-center rounded-lg border-gray-200 bg-white shadow dark:border-gray-700 dark:bg-gray-800"
    :class="[isYou ? 'border-4 border-green-400 dark:border-green-400' : 'border']"
  >
    <div class="flex w-full p-5">
      <div class="group relative w-max flex-col">
        <div
          class="flex h-10 w-10 cursor-default items-center justify-center rounded-full p-2 text-sm font-bold text-white"
          :style="{ backgroundColor: avatarColor }"
          aria-label="avatar"
        >
          {{ initials }}
        </div>
        <span
          class="pointer-events-none absolute -top-7 w-max translate-x-[-25%] cursor-default border border-gray-600 bg-gray-200 px-2 opacity-0 transition-opacity group-hover:opacity-100 dark:border-gray-200 dark:bg-gray-600"
        >
          {{ username }}
        </span>
      </div>
      <div class="cursor-pointer flex-col" @click="showReason = !showReason">
        <div
          v-if="answer.guess !== ''"
          class="ml-4 flex justify-center rounded-sm border border-white p-1 px-6"
        >
          {{ answer.guess }}
        </div>
        <div
          v-if="showReason"
          class="ml-4 mt-1 flex justify-center rounded-sm border border-white p-1 px-6"
        >
          {{ answer.reason }}
        </div>
      </div>
    </div>
    <div class="mr-5 flex items-end">
      <span>{{ answer.score }}</span>
      <span v-if="answer.bonusPoints > 0" class="ml-1 text-green-400">
        +{{ answer.bonusPoints }}
      </span>
      <span class="ml-1 text-xs">pts</span>
    </div>
  </div>
</template>
