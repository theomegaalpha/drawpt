<script setup lang="ts">
import { motion } from 'motion-v'
const { isPrimary, disabled } = defineProps({
  isPrimary: { type: Boolean, default: false },
  disabled: { type: Boolean, default: false }
})
</script>

<template>
  <motion.button
    :initial="{ '--x': '100%', scale: 1 }"
    :animate="{ '--x': '-100%' }"
    :whileTap="{ scale: 0.97 }"
    :transition="{
      repeat: Infinity,
      repeatType: 'loop',
      repeatDelay: 1,
      type: 'spring',
      stiffness: 20,
      damping: 15,
      mass: 2,
      scale: {
        type: 'spring',
        stiffness: 10,
        damping: 5,
        mass: 0.1
      }
    }"
    :disabled="disabled"
    :class="[
      'radial-gradient',
      { 'shiny-btn-primary': isPrimary },
      'relative rounded-full px-6 py-2'
    ]"
  >
    <span
      class="linear-mask relative inline-flex h-full w-full items-center justify-center font-light tracking-wide"
    >
      <slot />
    </span>
    <span class="linear-overlay absolute inset-0 block rounded-full p-px" />
  </motion.button>
</template>
