<script setup lang="ts">
import Test from '@/components/test/Test.vue'
import GameTimer from '@/components/room/game/midround/GameTimer.vue'
</script>

<template>
  <GameTimer :max-time="30000" class="fixed left-0 right-0 top-0" />
  <Test />
</template>

<style>
@media (min-width: 1024px) {
  .about {
    min-height: 100vh;
    display: flex;
    align-items: center;
  }
}
</style>
