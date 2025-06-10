<script setup lang="ts">
import { computed, onBeforeMount, ref } from 'vue'
const props = defineProps({
  maxTime: {
    type: Number,
    required: true
  }
})

const maxTime = props.maxTime - 1000
const timer = ref(maxTime)

var percentage = computed(() => {
  return (timer.value / maxTime) * 100
})

onBeforeMount(() => {
  const interval = setInterval(() => {
    timer.value -= 1000
    if (timer.value <= 0) {
      clearInterval(interval)
    }
  }, 1000)
})
</script>

<template>
  <div>
    <div class="relative h-6 overflow-hidden bg-gray-200 dark:bg-gray-600/80">
      <div class="h-full w-full"></div>
      <div
        class="transition-10 absolute left-0 top-0 h-full w-full transition-all duration-1000 ease-linear"
        :class="percentage <= 20 ? 'bg-red-400' : 'bg-green-400'"
        :style="{ width: `calc(${percentage}vw)` }"
      ></div>
    </div>
    <div id="glow" class="w-vw relative">
      <div
        class="absolute left-1/2 top-0 h-[2px] w-3/4 -translate-x-1/2 bg-gradient-to-r from-transparent via-green-500 to-transparent blur-sm transition-all duration-1000 ease-linear"
        :class="percentage <= 20 ? 'via-red-400' : 'via-green-400'"
      />
      <div
        class="absolute left-1/2 top-0 h-px w-3/4 -translate-x-1/2 bg-gradient-to-r from-transparent via-green-500 to-transparent transition-all duration-1000 ease-linear"
        :class="percentage <= 20 ? 'via-red-400' : 'via-green-400'"
      />
      <div
        class="absolute left-1/2 top-0 h-[5px] w-1/4 -translate-x-1/2 bg-gradient-to-r from-transparent via-sky-300 to-transparent blur-sm transition-all duration-1000 ease-linear"
      />
      <div
        class="absolute left-1/2 top-0 h-px w-1/4 -translate-x-1/2 bg-gradient-to-r from-transparent via-sky-300 to-transparent transition-all duration-1000 ease-linear"
      />
    </div>
  </div>
</template>

<style scoped></style>
