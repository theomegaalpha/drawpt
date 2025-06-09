<template>
  <component
    :is="props.as"
    :class="[
      'group relative inline-block overflow-hidden rounded-[20px] py-[1px]', // Added 'group'
      props.class // Use the prop for external classes
    ]"
  >
    <!-- Star animations -->
    <div
      class="animate-star-movement-bottom absolute bottom-[-11px] right-[-250%] z-0 h-[50%] w-[300%] rounded-full opacity-20 dark:opacity-70"
      :style="{
        background: `radial-gradient(circle, ${effectiveStarColor}, transparent 10%)`,
        animationDuration: props.speed
      }"
    ></div>
    <!-- Corrected self-closing tag -->
    <div
      class="animate-star-movement-top absolute left-[-250%] top-[-10px] z-0 h-[50%] w-[300%] rounded-full opacity-20 dark:opacity-70"
      :style="{
        background: `radial-gradient(circle, ${effectiveStarColor}, transparent 10%)`,
        animationDuration: props.speed
      }"
    ></div>
    <!-- Corrected self-closing tag -->

    <!-- Content wrapper -->
    <div
      class="z-1 from-background/90 to-muted/90 text-foreground dark:from-background dark:to-muted relative overflow-hidden rounded-[20px] border border-black/20 bg-gradient-to-b px-6 py-4 text-center text-base dark:border-white/10"
    >
      <!-- Shimmer effect (now a child of content wrapper) -->
      <div
        class="absolute inset-0 z-[1] -translate-x-full transform bg-[linear-gradient(48deg,var(--tw-gradient-stops))] from-transparent via-slate-500/5 to-transparent transition-transform ease-in-out group-hover:translate-x-full dark:via-white/10"
      ></div>
      <!-- Slot content wrapper -->
      <div class="relative z-[2]">
        <slot />
      </div>
    </div>
  </component>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue' // Added imports

const props = defineProps({
  as: {
    type: String,
    default: 'button'
  },
  color: {
    // This is the user-facing prop for star color
    type: String,
    default: 'white' // Default is white
  },
  speed: {
    type: String,
    default: '6s'
  },
  class: {
    type: String,
    default: ''
  }
})

const isDarkMode = ref(false)

const checkDarkMode = () => {
  isDarkMode.value =
    window.matchMedia('(prefers-color-scheme: dark)').matches ||
    document.documentElement.classList.contains('dark')
}

onMounted(() => {
  checkDarkMode() // Initial check
  window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', checkDarkMode)
})

const effectiveStarColor = computed(() => {
  if (props.color === 'white') {
    // Check if it's the default color
    return isDarkMode.value ? 'white' : 'black' // 'black' for light theme, 'white' for dark
  }
  return props.color // Use user-provided color otherwise
})
</script>
