<script setup lang="ts">
import { RouterView } from 'vue-router'
// import { motion } from 'motion-v' // motion is currently unused
import ShimmerAnimation from '@/components/common/ShimmerAnimation.vue'
</script>

<template>
  <ShimmerAnimation>
    <RouterView />
  </ShimmerAnimation>
  <!-- Any other global UI elements that should always be on top of everything -->
</template>

<style></style>
