<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  size?: string
  username: string
  color: string
}>()

const initials = computed(() => {
  const names = props.username.split(' ')
  if (!names) return ''
  let initials = names[0].substring(0, 1).toUpperCase()

  if (names.length > 1) {
    initials += names[names.length - 1].substring(0, 1).toUpperCase()
  }

  return initials
})
</script>

<template>
  <div
    :class="`cursor-default rounded-full ${size ?? 'h-10 w-10'} bg-${color} flex items-center justify-center font-semibold text-white`"
  >
    <span>{{ initials }}</span>
  </div>
</template>
