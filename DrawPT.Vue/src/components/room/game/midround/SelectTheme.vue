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
      class="border rounded-md border-gray-400 p-2 m-2 cursor-pointer hover:bg-gray-200"
    >
      {{ theme }}
    </div>
  </div>
</template>
