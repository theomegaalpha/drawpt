<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
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

onMounted(() => {
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
    <div class="h-3 relative overflow-hidden bg-gray-200">
      <div class="w-full h-full bg-gray-200"></div>
      <div
        class="h-full w-full transition-all duration-1000 ease-linear left-0 absolute top-0 transition-10"
        :class="percentage <= 20 ? 'bg-red-400' : 'bg-green-400'"
        :style="{ width: percentage + '%' }"
      ></div>
    </div>
  </div>
</template>
