<script setup lang="ts">
import type { DailyAnswer } from '@/models/dailyModels'
import { Check, FlameIcon } from 'lucide-vue-next'
import { computed } from 'vue'

interface Props {
  dailyAnswerHistory: DailyAnswer[]
}

const props = defineProps<Props>()

const daysOfWeek = ['S', 'M', 'T', 'W', 'T', 'F', 'S'] // Sun, Mon, Tue, Wed, Thu, Fri, Sat

// Helper to get the last 7 days (including today) as date strings (YYYY-MM-DD)
function getLast7Days(): string[] {
  const days: string[] = []
  const today = new Date()
  for (let i = 6; i >= 0; i--) {
    const d = new Date(today)
    d.setDate(today.getDate() - i)
    days.push(d.toISOString().slice(0, 10))
  }
  return days
}

// Compute a 7-day win streak array from dailyAnswerHistory
const winStreak = computed(() => {
  console.log(
    'Calculating win streak for last 7 days...',
    props.dailyAnswerHistory.length,
    'answers found.'
  )
  const last7Days = getLast7Days()
  return last7Days.map((date) => {
    // Mark as win if there is an answer for this date and score > 0
    return props.dailyAnswerHistory.some(
      (ans) => new Date(ans.date).toISOString().slice(0, 10) === date && ans.score > 0
    )
  })
})

const currentWinStreak = computed(() => {
  // Sort history by date ascending
  const sorted = [...props.dailyAnswerHistory].sort((a, b) => a.date.localeCompare(b.date))
  let streak = 0
  let day = new Date()
  for (;;) {
    const dayStr = day.toISOString().slice(0, 10)
    const win = sorted.find((ans) => ans.date.slice(0, 10) === dayStr && ans.score > 0)
    if (win) {
      streak++
      day.setDate(day.getDate() - 1)
    } else {
      break
    }
  }
  return streak
})

const getDayLabel = (index: number): string => {
  // Show the day of week for each of the last 7 days
  const last7Days = getLast7Days()
  const date = new Date(last7Days[index])
  return daysOfWeek[date.getDay()]
}
</script>

<template>
  <div class="flex flex-col items-center">
    <div class="relative mb-2 flex flex-col items-center" style="width: 3rem; height: 3rem">
      <FlameIcon
        :fill="currentWinStreak > 0 ? 'red' : 'gray'"
        class="absolute left-0 top-0 z-0 h-12 w-full text-gray-500"
        :class="{ 'text-red-500': currentWinStreak > 0 }"
      />
      <span
        v-if="currentWinStreak > 0"
        class="pointer-events-none absolute left-1/2 top-1/2 z-10 -translate-x-1/2 -translate-y-1/3 select-none text-2xl font-bold text-white drop-shadow-[0_0_2px_black]"
      >
        {{ currentWinStreak }}
      </span>
    </div>
    <div class="flex space-x-3">
      <div v-for="(isWin, index) in winStreak" :key="index" class="flex flex-col items-center">
        <div
          :class="[
            'flex h-8 w-8 items-center justify-center rounded-full border-2',
            isWin
              ? 'border-green-600 bg-green-500 dark:border-green-700 dark:bg-green-600'
              : 'border-gray-300 bg-gray-200 dark:border-gray-500 dark:bg-gray-600'
          ]"
        >
          <span v-if="isWin" class="text-sm font-extrabold text-white">
            <Check class="h-5 w-5" />
          </span>
        </div>
        <span class="mt-1 text-xs text-gray-500 dark:text-gray-400">
          {{ getDayLabel(index) }}
        </span>
      </div>
    </div>
  </div>
</template>
