<template>
  <div class="bg-card overflow-hidden rounded-lg shadow-md">
    <div class="border-border flex border-b">
      <button
        v-for="tab in tabs"
        :key="tab.id"
        @click="activeTab = tab.id"
        class="flex-1 px-6 py-4 text-center text-sm font-medium transition-colors"
        :class="activeTab === tab.id ? 'bg-primary text-primary-foreground' : 'hover:bg-muted'"
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

    <div v-else-if="activeTabPlayers.length === 0" class="p-8 text-center">
      <p class="text-muted-foreground">No players found for this leaderboard.</p>
    </div>

    <div v-else class="divide-border divide-y">
      <div
        v-for="(player, index) in activeTabPlayers"
        :key="player.id"
        class="hover:bg-muted/50 flex items-center p-4 transition-colors"
      >
        <div class="text-muted-foreground mr-4 w-10 flex-shrink-0 text-center font-semibold">
          {{ index + 1 }}
        </div>
        <div class="bg-muted h-10 w-10 flex-shrink-0 overflow-hidden rounded-full">
          <img
            :src="randomImage()"
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
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useLeaderboardStore } from '@/stores/leaderboard'

const leaderboardStore = useLeaderboardStore()
const isLoading = computed(() => leaderboardStore.isLoading)
const randomImage = () => {
  const images = [
    '/images/profile-photos/animal-1.png',
    '/images/profile-photos/animal-2.png',
    '/images/profile-photos/animal-3.png',
    '/images/profile-photos/animal-4.png',
    '/images/profile-photos/animal-5.png',
    '/images/profile-photos/animal-6.png',
    '/images/profile-photos/anime-1.png',
    '/images/profile-photos/anime-2.png',
    '/images/profile-photos/anime-3.png',
    '/images/profile-photos/anime-4.png',
    '/images/profile-photos/anime-5.png',
    '/images/profile-photos/anime-6.png',
    '/images/profile-photos/anime-7.png',
    '/images/profile-photos/anime-8.png',
    '/images/profile-photos/anime-9.png',
    '/images/profile-photos/anime-10.png'
  ]
  let randomIndex = Math.floor(Math.random() * images.length)
  return images[randomIndex]
}

const tabs = [
  { id: 'daily', label: 'Daily Prompt' },
  { id: 'rooms', label: 'Game Rooms' }
]

const activeTab = ref('daily')

const activeTabPlayers = computed(() => {
  return activeTab.value === 'daily' ? leaderboardStore.dailies : leaderboardStore.playerResults
})

onMounted(async () => {
  // Fetch initial data for the active tab
  await fetchTabData()
})

const fetchTabData = async () => {
  if (activeTab.value === 'daily') {
    if (leaderboardStore.dailies.length === 0) {
      await leaderboardStore.fetchDailies()
    }
  } else {
    if (leaderboardStore.dailies.length === 0) {
      await leaderboardStore.fetchDailies()
    }
  }
}

// Watch for tab changes to load data if needed
watch(activeTab, async (newTab) => {
  await fetchTabData()
})

const handleImageError = (event: Event) => {
  // Replace broken image with a fallback
  const target = event.target as HTMLImageElement
  target.src = 'https://picsum.photos/id/237/200/200' // Fallback image
}
</script>
