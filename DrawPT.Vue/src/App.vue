<script setup lang="ts">
import { RouterView } from 'vue-router'
import { motion } from 'motion-v'
import ShimmerAnimation from '@/components/common/ShimmerAnimation.vue'
</script>

<template>
  <ShimmerAnimation>
    <RouterView />
    <motion.div
      :initial="{ opacity: 0.0, y: 40 }"
      :whileInView="{ opacity: 0.5, y: 0 }"
      :transition="{
        delay: 0.3,
        duration: 0.8,
        ease: 'easeInOut'
      }"
      class="relative flex flex-col items-center justify-center gap-4 px-4"
    >
    </motion.div>
  </ShimmerAnimation>
</template>

<style></style>
