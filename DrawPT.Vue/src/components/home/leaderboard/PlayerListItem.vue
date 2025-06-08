<template>
  <div class="hover:bg-muted/50 flex items-center p-4 transition-colors">
    <div class="text-muted-foreground mr-4 w-10 flex-shrink-0 text-center font-semibold">
      {{ rank }}
    </div>
    <div class="bg-muted h-10 w-10 flex-shrink-0 overflow-hidden rounded-full">
      <img
        :src="player.color"
        :alt="`${player.username}'s avatar`"
        class="h-full w-full object-cover"
        @error="handleImageError"
      />
    </div>
    <div class="ml-4 flex-1">
      <h3 class="font-medium">{{ player.username }}</h3>
    </div>
    <div class="text-lg font-bold">
      {{ player.score }}
    </div>
  </div>
</template>

<script setup lang="ts">
import type { PlayerResult } from '@/models/player'

const props = defineProps<{
  player: PlayerResult
  rank: number
}>()

const handleImageError = (event: Event) => {
  // Replace broken image with a fallback
  const target = event.target as HTMLImageElement
  target.src = 'https://picsum.photos/id/237/200/200' // Fallback image
}
</script>
