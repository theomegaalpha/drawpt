<script setup lang="ts">
import type { DailyAnswer } from '@/models/dailyModels'
import { Check, FlameIcon } from 'lucide-vue-next'
import { computed } from 'vue'

interface Props {
  dailyAnswerHistory: DailyAnswer[]
}

const props = defineProps<Props>()

// Labels for days Sunday (0) through Saturday (6)
const daysOfWeek = ['S', 'M', 'T', 'W', 'T', 'F', 'S']

// Helper to format a Date as local YYYY-MM-DD
function formatDateLocal(d: Date): string {
  const y = d.getFullYear()
  const m = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${y}-${m}-${day}`
}

// Helper to get dates for current week Sunday through Saturday
function getCurrentWeekDates(): string[] {
  const today = new Date()
  const sunday = new Date(today)
  sunday.setDate(today.getDate() - today.getDay())
  return Array.from({ length: 7 }).map((_, i) => {
    const d = new Date(sunday)
    d.setDate(sunday.getDate() + i)
    return formatDateLocal(d)
  })
}

// Compute a weekly win array for current calendar week (Sun–Sat)
const winStreak = computed(() => {
  const weekDates = getCurrentWeekDates()
  return weekDates.map((date) =>
    props.dailyAnswerHistory.some((ans) => {
      const ansDate = new Date(ans.date)
      return formatDateLocal(ansDate) === date && ans.score > 0
    })
  )
})

// Compute consecutive wins ending today, normalized to local dates
const currentWinStreak = computed(() => {
  let streak = 0
  let day = new Date()
  for (;;) {
    const dayStr = formatDateLocal(day)
    const win = props.dailyAnswerHistory.some((ans) => {
      const ansDate = new Date(ans.date)
      return formatDateLocal(ansDate) === dayStr && ans.score > 0
    })
    if (win) {
      streak++
      day.setDate(day.getDate() - 1)
    } else {
      break
    }
  }
  return streak
})

// Label for each column based on fixed week day order
const getDayLabel = (index: number): string => daysOfWeek[index]
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
