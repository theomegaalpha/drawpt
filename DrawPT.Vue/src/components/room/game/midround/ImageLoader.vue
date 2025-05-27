<script setup lang="ts">
import { ref } from 'vue'
const num = ref(Array(16).fill(0))
</script>

<template>
  <div
    class="preserve-3d perspective-1000 rotate-y-45 rotate-x-45 relative block h-[50vh] w-[50vh] transform"
  >
    <div
      v-for="(n, index) in num"
      :key="index"
      :data-index="index"
      :class="[
        'animate-spin-x absolute rounded-full border-2 border-indigo-900 bg-transparent dark:border-white',
        {
          'border-indigo-850/50 border-dashed dark:border-white/50': (index + 1) % 2 === 0,
          hidden: index === num.length - 1
        }
      ]"
      :style="{
        animationDuration: 22 / (index + 1) + 's',
        left: `${index}vh`,
        top: `${index}vh`,
        width: `${50 - index * 2}vh`,
        height: `${50 - index * 2}vh`
      }"
    ></div>
  </div>
</template>

<style scoped>
@keyframes spin-x {
  0% {
    transform: rotateX(0deg);
  }
  100% {
    transform: rotateX(360deg);
  }
}

.animate-spin-x {
  animation: spin-x infinite linear;
}

.preserve-3d {
  transform-style: preserve-3d;
}

.perspective-1000 {
  perspective: 1000px;
}

.rotate-y-45 {
  transform: perspective(1000px) rotateY(45deg) rotateX(45deg);
}
</style>
