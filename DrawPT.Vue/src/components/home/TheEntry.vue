<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { RouterLink } from 'vue-router'
import WinStreak from '../common/WinStreak.vue'
import Leaderboard from './leaderboard/Leaderboard.vue'
import DailyQuestion from './DailyQuestion.vue'
import { useDailiesStore } from '@/stores/dailies'
import DailiesCarousel from '../common/DailiesCarousel.vue'

const { dailyQuestions, dailyAnswerHistory } = storeToRefs(useDailiesStore())

const scrollToPrompt = () => {
  const element = document.getElementById('prompt-of-the-day')
  if (element) {
    element.scrollIntoView({ behavior: 'smooth' })
  }
}
</script>

<template>
  <main class="container mx-auto px-6 py-8">
    <!-- Hero Section -->
    <div class="mb-10 animate-blur-in px-6 py-16 text-center">
      <h1 class="text-color-accent mb-4 text-4xl font-bold md:text-5xl">AI Draws, You Decipher</h1>
      <p class="text-color-default mx-auto max-w-3xl text-lg">
        AI-powered Pictionary where you decode abstract creations based on complex phrases.
      </p>
      <p class="text-color-default mx-auto mb-8 max-w-2xl text-lg font-semibold">
        Can you beat the algorithm?
      </p>
      <div>
        <button class="btn-default text-lg font-semibold shadow-md" @click="scrollToPrompt">
          Daily Challenge
        </button>
        <RouterLink
          class="btn-default ml-2 text-lg font-semibold shadow-md"
          :to="{ name: 'game-selection' }"
        >
          Play Game
        </RouterLink>
      </div>
    </div>
    <div class="mb-24 flex justify-center">
      <div class="aspect-video w-full max-w-3xl">
        <iframe
          class="h-full w-full"
          src="https://www.youtube.com/embed/tctpLLkZbWQ"
          title="YouTube video player"
          frameborder="0"
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
          allowfullscreen
        ></iframe>
      </div>
    </div>
    <div id="prompt-of-the-day" class="mb-10 grid grid-cols-1 gap-8 lg:grid-cols-2">
      <div class="bg-surface-default rounded-xl p-6 shadow-md">
        <DailyQuestion />
      </div>

      <div class="flex flex-col gap-8">
        <div
          class="bg-surface-default flex flex-grow flex-col overflow-hidden rounded-xl shadow-md"
        >
          <div class="p-4">
            <h2 class="text-xl font-medium">Leaderboard</h2>
          </div>
          <Leaderboard class="flex-grow" />
        </div>
        <div class="bg-surface-default rounded-xl p-6 shadow-md">
          <h2 class="mb-4 text-xl font-bold">Win Streak</h2>
          <WinStreak
            class="flex items-center justify-center"
            :dailyAnswerHistory="dailyAnswerHistory"
          />
        </div>
      </div>
    </div>
    <!-- Dailies Carousel -->
    <div class="mb-10 animate-blur-in">
      <h2 class="mb-4 text-center text-xl font-bold">Past Dailies</h2>
      <div class="flex space-x-4 overflow-x-auto py-4">
        <DailiesCarousel :dailies="dailyQuestions" />
      </div>
    </div>
  </main>
</template>
