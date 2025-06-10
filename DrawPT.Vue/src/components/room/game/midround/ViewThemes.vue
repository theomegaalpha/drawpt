<script setup lang="ts">
import { ref, onBeforeMount } from 'vue'

const props = defineProps({
  themes: {
    type: Array<string>,
    required: true
  }
})

const animationsReadyToPlay = ref(false)

onBeforeMount(() => {
  // Ensure the DOM has updated with initial paused state before trying to play
  requestAnimationFrame(() => {
    animationsReadyToPlay.value = true
  })
})
</script>

<template>
  <div class="text-center text-xl font-bold">
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
</template>
