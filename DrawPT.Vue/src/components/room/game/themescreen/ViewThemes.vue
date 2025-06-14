<script setup lang="ts">
import { ref, onMounted } from 'vue'

const props = defineProps({
  themes: {
    type: Array<string>,
    required: true
  }
})

const animationsReadyToPlay = ref(false)

onMounted(() => {
  // Ensure the DOM has updated with initial paused state before trying to play
  requestAnimationFrame(() => {
    animationsReadyToPlay.value = true
  })
})
</script>

<template>
  <div class="flex min-h-screen flex-col items-center justify-center text-center text-xl font-bold">
    <h1 class="text-2xl font-bold">Selecting Theme...</h1>
    <div class="relative mb-16 w-[40rem]">
      <div v-for="(theme, index) in props.themes" :key="index">
        <div
          class="bg-surface-default mb-4 animate-blur-in cursor-default rounded-lg px-9 py-3"
          :style="{
            animationDelay: `${index * 200}ms`,
            animationPlayState: animationsReadyToPlay ? 'running' : 'paused',
            animationFillMode: 'backwards'
          }"
        >
          {{ theme }}
        </div>
      </div>
    </div>
  </div>
</template>
