<template>
  <div class="my-2 flex cursor-pointer gap-[5px]" title="Click to copy values">
    <div
      v-for="(value, index) in closenessArray"
      :key="index"
      class="flex h-[25px] w-[25px] items-center justify-center rounded-[3px] border border-[#555] text-[0.8em] text-white transition-colors duration-300 ease-in-out"
      :style="{ backgroundColor: getColor(value) }"
      aria-live="polite"
      :aria-label="`Closeness value ${value} out of 9`"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps<{
  closenessArray: number[]
}>()

const getColor = (value: number): string => {
  if (value < 0 || value > 9) {
    return 'rgb(128, 128, 128)'
  }
  const greenIntensity = Math.round((value / 9) * 255)
  return `rgb(0, ${greenIntensity}, 0)`
}
</script>
