<script setup lang="ts">
import { defineEmits, ref, onBeforeMount } from 'vue'
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

const emit = defineEmits(['themeSelected'])
const handleClick = (theme: string) => {
  emit('themeSelected', theme)
}
</script>

<template>
  <div class="flex min-h-screen flex-col items-center justify-center text-center text-xl font-bold">
    <h1 class="text-2xl font-bold">Select a Theme</h1>
    <div class="relative mb-16 w-[40rem]">
      <div
        class="absolute inset-x-20 top-0 h-[2px] w-3/4 bg-gradient-to-r from-transparent via-indigo-500 to-transparent blur-sm"
      />
      <div
        class="absolute inset-x-20 top-0 h-px w-3/4 bg-gradient-to-r from-transparent via-indigo-500 to-transparent"
      />
      <div
        class="absolute inset-x-60 top-0 h-[5px] w-1/4 bg-gradient-to-r from-transparent via-sky-500 to-transparent blur-sm"
      />
      <div
        class="absolute inset-x-60 top-0 h-px w-1/4 bg-gradient-to-r from-transparent via-sky-500 to-transparent"
      />
    </div>
    <div v-for="(theme, index) in props.themes" :key="index" @click="handleClick(theme)">
      <div
        class="animate-blur-in cursor-default"
        :style="{
          animationDelay: `${index * 50}ms`,
          animationPlayState: animationsReadyToPlay ? 'running' : 'paused',
          animationFillMode: 'backwards'
        }"
      >
        <div
          class="bg-surface-default shimmer-glow mb-4 animate-blur-in cursor-pointer rounded-lg px-9 py-2"
          :style="{
            animationDelay: `${index * 50}ms`,
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

<style scoped>
.shimmer-glow {
  text-decoration: none;
  border: 1px solid rgb(146, 148, 248);
  position: relative;
  overflow: hidden;
}

.shimmer-glow:hover {
  box-shadow: 1px 1px 5px 2px rgba(146, 148, 248, 0.4);
}

.shimmer-glow:before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(120deg, transparent, rgba(146, 148, 248, 0.4), transparent);
  transition: all 150ms;
}

.shimmer-glow:hover:before {
  left: 100%;
}
</style>
