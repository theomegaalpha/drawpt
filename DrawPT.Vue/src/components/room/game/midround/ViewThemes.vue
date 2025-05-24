<script setup lang="ts">
const props = defineProps({
  themes: {
    type: Array<string>,
    required: true
  }
})
</script>

<template>
  <div class="font-bold">
    <div v-for="(theme, index) in props.themes" :key="index">
      {{ theme }}
    </div>
  </div>
</template>
