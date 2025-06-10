<template>
  <div class="w-full">
    <div class="border-border flex border-b">
      <button
        v-for="tab in tabs"
        :key="tab.id"
        @click="setActiveTab(tab.id)"
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

    <div v-else-if="activeTabContent.length === 0" class="p-8 text-center">
      <p class="text-muted-foreground">No players found for this leaderboard.</p>
    </div>

    <div v-else class="divide-border divide-y">
      <PlayerListItem
        v-for="(player, index) in activeTabContent"
        :key="player.id"
        :player="player"
        :rank="index + 1"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onBeforeMount, watch, ref } from 'vue'
import { useLeaderboardStore } from '@/stores/leaderboard'
import PlayerListItem from './PlayerListItem.vue'

const leaderboardStore = useLeaderboardStore()
const isLoading = computed(() => leaderboardStore.isLoading)
const activeTab = ref('daily')

const tabs = [
  { id: 'daily', label: 'Daily Prompt' },
  { id: 'rooms', label: 'Game Rooms' }
]

const activeTabContent = computed(() => {
  return activeTab.value === 'daily' ? leaderboardStore.dailies : leaderboardStore.playerResults
})

const setActiveTab = (tabId: string) => {
  activeTab.value = tabId
}

onBeforeMount(async () => {
  // Fetch initial data for the active tab
  await fetchTabData()
})

const fetchTabData = async () => {
  if (activeTab.value === 'daily') {
    if (leaderboardStore.dailies.length === 0) {
      await leaderboardStore.fetchDailies()
    }
  } else {
    if (leaderboardStore.playerResults.length === 0) {
      await leaderboardStore.fetchPlayerResults()
    }
  }
}

// Watch for tab changes to load data if needed
watch(activeTab, async () => {
  await fetchTabData()
})
</script>
