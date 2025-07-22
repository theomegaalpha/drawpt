<template>
  <div class="bg-card overflow-hidden rounded-lg shadow-md">
    <div class="border-border flex border-b">
      <button
        v-for="tab in tabs"
        :key="tab.id"
        @click="tab.enabled && (activeTab = tab.id)"
        :disabled="!tab.enabled"
        class="flex-1 px-6 py-4 text-center text-sm font-medium transition-colors"
        :class="[
          activeTab === tab.id
            ? 'bg-black/5 dark:bg-white/5'
            : tab.enabled
              ? 'hover:bg-muted'
              : 'text-muted-foreground cursor-not-allowed opacity-50'
        ]"
      >
        {{ tab.label }}
      </button>
    </div>

    <div v-if="isLoading" class="p-8 text-center">
      <div
        class="inline-block h-8 w-8 animate-spin rounded-full border-4 border-solid border-current border-r-transparent align-[-0.125em] motion-reduce:animate-[spin_1.5s_linear_infinite]"
      ></div>
      <p class="text-muted-foreground mt-4">Loading leaderboard data...</p>
    </div>

    <div v-else-if="activeTabResults.length === 0" class="p-8 text-center">
      <p class="text-muted-foreground">No players found for this leaderboard.</p>
    </div>

    <div v-else class="divide-border divide-y">
      <div
        v-for="(daily, index) in activeTabResults"
        :key="daily.playerId"
        class="hover:bg-muted/50 flex animate-[slide-in-left_0.2s_ease-in_forwards] items-center p-4 transition-colors"
      >
        <div class="text-muted-foreground mr-4 w-10 flex-shrink-0 text-center font-semibold">
          {{ index + 1 }}
        </div>
        <div class="bg-muted h-10 w-10 flex-shrink-0 overflow-hidden rounded-full">
          <img
            :src="daily.avatar || playerStore.blankAvatar"
            :alt="`${daily.username}'s avatar`"
            class="h-full w-full object-cover"
            @error="handleImageError"
          />
        </div>
        <div class="ml-4 flex-1">
          <h3 class="font-medium">{{ daily.username }}</h3>
        </div>
        <div class="text-lg font-bold">
          {{ daily.score }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useLeaderboardStore } from '@/stores/leaderboard'
import { usePlayerStore } from '@/stores/player'

const leaderboardStore = useLeaderboardStore()
const playerStore = usePlayerStore()
const isLoading = computed(() => leaderboardStore.isLoading)

const tabs = [
  { id: 'daily', label: 'Daily Prompt', enabled: true },
  { id: 'rooms', label: 'Game Rooms', enabled: false }
]

const activeTab = ref('daily')

const activeTabResults = computed(() => {
  return activeTab.value === 'daily' ? leaderboardStore.dailies : leaderboardStore.dailies
})

onMounted(async () => {
  // Fetch initial data for the active tab
  await fetchTabData()
})

const fetchTabData = async () => {
  if (activeTab.value === 'daily') {
    if (leaderboardStore.dailies.length === 0) {
      await leaderboardStore.fetchDailiesTop20()
    }
  } else {
    if (leaderboardStore.dailies.length === 0) {
      await leaderboardStore.fetchDailiesTop20()
    }
  }
}

// Watch for tab changes to load data if needed
watch(activeTab, async () => {
  await fetchTabData()
})

const handleImageError = (event: Event) => {
  // Replace broken image with a fallback
  const target = event.target as HTMLImageElement
  target.src = playerStore.blankAvatar
}
</script>
