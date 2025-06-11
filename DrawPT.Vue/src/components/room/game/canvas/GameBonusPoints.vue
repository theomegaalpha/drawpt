<script setup lang="ts">
const props = defineProps({
  points: {
    type: Number,
    required: true
  }
})
</script>
<template>
  <div class="fixed bottom-4 z-50 text-green-600 font-bold">
    <span>+ {{ props.points }}</span>
  </div>
</template>
