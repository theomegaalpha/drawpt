<script setup lang="ts">
// No script needed for static content
import PurposeSection from '@/components/about/PurposeSection.vue'
import GameplaySection from '@/components/about/GameplaySection.vue'
import AboutMeSection from '@/components/about/AboutMeSection.vue'
</script>

<template>
  <section class="mx-auto max-w-4xl px-4 py-12">
    <h1 class="mb-4 text-center text-4xl font-bold">About DrawPT</h1>
  </section>
  <section class="mx-auto max-w-4xl px-4 py-12">
    <div class="flex flex-col items-center gap-8 lg:flex-row">
      <div class="">
        <div class="mx-auto mb-2 w-fit rounded-md bg-black/10 px-4 py-1 dark:bg-white/10">
          🤖 A New Kind of Referee
        </div>
        <p class="text-md mb-2 text-center">
          Before AI, scoring in games like this was rigid. You were either right or wrong; no
          in-between.
        </p>
        <p class="text-md mb-2 text-center">
          But with an AI acting as our in-game referee, things shift. Scoring becomes more fluid,
          more human in a way. You might not guess the exact prompt, but your answer could be close
          enough. You can be partially right.
        </p>
        <p class="text-md mb-2 text-center">
          This completely changes the game. I hope this gets your creative juices flowing.
        </p>
        <p class="text-md mb-2 text-center">
          What game couldn't exist before? What game can exist now with AI?
        </p>
      </div>
    </div>
  </section>
  <PurposeSection />
  <GameplaySection />
  <AboutMeSection />
</template>

<style scoped></style>

