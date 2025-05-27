<script setup lang="ts">
import { defineEmits } from 'vue'
const props = defineProps({
  themes: {
    type: Array<string>,
    required: true
  }
})

const emit = defineEmits(['themeSelected'])
const handleClick = (theme: string) => {
  emit('themeSelected', theme)
}
</script>

<template>
  <div class="font-bold">
    <h1 class="text-2xl font-bold">Select a theme:</h1>
    <div
      v-for="(theme, index) in props.themes"
      :key="index"
      @click="handleClick(theme)"
      class="shimmer-glow m-2 cursor-pointer rounded-md border border-gray-400 p-2"
    >
      {{ theme }}
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
