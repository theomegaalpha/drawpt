<script setup lang="ts">
import { computed } from 'vue';

interface Props {
  // An array of 7 booleans, where true represents a win for that day.
  // Index 0 is the most recent day (today), index 6 is 7 days ago.
  dailyStatus: boolean[];
  // Optional: The current day index (0-6, Sunday-Saturday) to label days
  currentDayIndex?: number;
}

const props = withDefaults(defineProps<Props>(), {
  dailyStatus: () => Array(7).fill(false),
  currentDayIndex: undefined, // No labels by default
});

const daysOfWeek = ['S', 'M', 'T', 'W', 'T', 'F', 'S']; // Sun, Mon, Tue, Wed, Thu, Fri, Sat

// Ensure dailyStatus is always an array of 7
const normalizedDailyStatus = computed(() => {
  const status = props.dailyStatus || [];
  const filledStatus = Array(7).fill(false);
  for (let i = 0; i < Math.min(status.length, 7); i++) {
    filledStatus[i] = status[i];
  }
  return filledStatus.reverse(); // Reverse to show oldest day first (left to right)
});

const getDayLabel = (index: number): string => {
  if (props.currentDayIndex === undefined) return '';
  const dayIdx = (props.currentDayIndex - (6 - index) + 7) % 7;
  return daysOfWeek[dayIdx];
};

</script>

<template>
  <div class="flex flex-col items-center">
    <p class="mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">7-Day Win Streak</p>
    <div class="flex space-x-2">
      <div
        v-for="(isWin, index) in normalizedDailyStatus"
        :key="index"
        class="flex flex-col items-center"
      >
        <div
          :class="[
            'w-8 h-8 rounded-full flex items-center justify-center border-2',
            isWin ? 'bg-green-500 border-green-600 dark:bg-green-600 dark:border-green-700' : 'bg-gray-200 border-gray-300 dark:bg-gray-600 dark:border-gray-500',
          ]"
        >
          <span v-if="isWin" class="text-white font-bold text-sm">✓</span>
          <!-- Placeholder for future/missed days, could be empty or show a different icon -->
        </div>
        <span v-if="currentDayIndex !== undefined" class="mt-1 text-xs text-gray-500 dark:text-gray-400">
          {{ getDayLabel(index) }}
        </span>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Add any component-specific styles here if needed */
</style>
