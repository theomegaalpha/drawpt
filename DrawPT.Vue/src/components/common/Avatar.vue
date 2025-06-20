<script setup lang="ts">
defineProps<{
  size?: number
  username: string
  avatar: string
}>()
</script>

<template>
  <div
    :class="`cursor-default rounded-full ${size ? 'h-' + size + ' w-' + size : 'h-10 w-10'} border-2 border-gray-500 dark:border-white`"
  >
    <img :src="avatar" :alt="username" class="h-full w-full rounded-full object-cover" />
  </div>
</template>
