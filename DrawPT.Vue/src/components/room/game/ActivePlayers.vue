<script setup lang="ts">
import { computed, ref } from 'vue'
import { useGameStateStore } from '@/stores/gameState'
import { usePlayerStore } from '@/stores/player'

const { players } = useGameStateStore()
</script>

<template>
  <div>
    <h2 class="text-lg font-bold">Active Players</h2>
    <ul>
      <li v-for="player in players" :key="player.id">
        {{ player.username }}
      </li>
    </ul>
  </div>
</template>
