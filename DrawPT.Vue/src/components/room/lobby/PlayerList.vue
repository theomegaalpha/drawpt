<script setup lang="ts">
import Avatar from '@/components/common/Avatar.vue'
import { usePlayerStore } from '@/stores/player'
import { useRoomStore } from '@/stores/room'
import service from '@/services/signalRService'
import { ref } from 'vue'

const { player } = usePlayerStore()
const { room } = useRoomStore()
</script>

<template>
  <div class="cursor-default px-4 md:px-8 xl:px-12">
    <h2 class="mb-5 text-center text-2xl font-bold">Players</h2>
    <div
      class="animate-slide-in-left mt-2 flex flex-row items-center rounded-lg border border-zinc-200 bg-white p-2 shadow dark:border-zinc-900 dark:bg-black"
      v-for="p in room.players"
      :key="p.id"
    >
      <Avatar :username="p.username" :color="p.color" />
      <p class="ml-2 xl:ml-4">{{ p.username }}</p>
    </div>
    <div
      class="mt-2 flex flex-row items-center rounded-lg border border-zinc-200 bg-zinc-100 p-2 shadow dark:border-zinc-900 dark:bg-zinc-900"
      v-for="p in 8 - room.players.length"
      :key="p"
    >
      <div
        class="flex h-10 w-10 cursor-default items-center justify-center rounded-full bg-slate-200 font-semibold text-white dark:bg-slate-400"
      ></div>
      <p class="ml-2 xl:ml-4">Open player slot</p>
    </div>
  </div>
</template>

<style scoped>
/* Add any specific styles for PlayerList here */
</style>
