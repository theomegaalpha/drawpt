<script setup lang="ts">
import ThemeButton from './ThemeButton.vue'
import { defineEmits, ref, onMounted } from 'vue'
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

const emit = defineEmits(['themeSelected'])
const handleClick = (theme: string) => {
  emit('themeSelected', theme)
}
</script>

<template>
  <div class="text-center text-xl font-bold">
    <h1 class="-mt-8 text-2xl font-bold">Select a Theme</h1>
    <div className="w-[40rem] mb-16 relative">
      <div
        className="absolute inset-x-20 top-0 bg-gradient-to-r from-transparent via-indigo-500 to-transparent h-[2px] w-3/4 blur-sm"
      />
      <div
        className="absolute inset-x-20 top-0 bg-gradient-to-r from-transparent via-indigo-500 to-transparent h-px w-3/4"
      />
      <div
        className="absolute inset-x-60 top-0 bg-gradient-to-r from-transparent via-sky-500 to-transparent h-[5px] w-1/4 blur-sm"
      />
      <div
        className="absolute inset-x-60 top-0 bg-gradient-to-r from-transparent via-sky-500 to-transparent h-px w-1/4"
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
        <ThemeButton>{{ theme }}</ThemeButton>
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
  transition: all 650ms;
}

.shimmer-glow:hover:before {
  left: 100%;
}
</style>
