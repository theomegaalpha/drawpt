<script setup lang="ts">
import { ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useDailiesStore } from '@/stores/dailies'
import GuessInput from '@/components/common/GuessInput.vue'
import api from '@/api/api'

const guess = ref('')
const dailiesStore = useDailiesStore()
const { dailyQuestion, dailyAnswer, imageLoaded } = storeToRefs(dailiesStore)

const defaultImageUrl = '/images/daily-image-error.png'

const handleImageError = (event: Event) => {
  dailiesStore.setImageLoaded(false)
  console.error('Error loading daily image:', event)
}

const submitGuess = () => {
  var myGuess = guess.value.trim()
  if (myGuess) {
    console.log('Submitted guess:', myGuess)
    dailiesStore.setIsLoading(true)
    api
      .submitAnswer(myGuess)
      .then((assessedAnswer) => {
        dailiesStore.setDailyAnswer(assessedAnswer)
      })
      .catch((error) => {
        console.error('API error:', error)
      })
      .finally(() => {
        guess.value = ''
        dailiesStore.setIsLoading(false)
      })
  }
}
</script>

<style scoped>
/* Add any component-specific styles here */
</style>

<template>
  <div class="flex items-baseline gap-x-2">
    <h2 class="text-xl font-bold">Guess Today's Prompt</h2>
    <span class="text-lg">Hint: {{ dailyQuestion.theme }}</span>
  </div>
  <div class="prose prose-indigo dark:prose-invert text-color-default mt-2">
    <div
      class="relative overflow-hidden rounded-lg border border-gray-200 bg-gray-50 dark:border-zinc-700 dark:bg-zinc-900"
    >
      <img
        :src="imageLoaded ? dailyQuestion.imageUrl : defaultImageUrl"
        :alt="dailyQuestion.theme"
        class="h-auto w-full object-contain"
        @error="handleImageError"
      />
      <div
        v-if="!imageLoaded"
        class="absolute inset-0 flex flex-col items-center justify-center bg-black bg-opacity-50 text-white dark:bg-opacity-70"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="48"
          height="48"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          class="mb-4 h-12 w-12 text-red-500"
        >
          <circle cx="12" cy="12" r="10" />
          <line x1="12" y1="8" x2="12" y2="12" />
          <line x1="12" y1="16" x2="12.01" y2="16" />
        </svg>
        <p class="text-center text-lg font-semibold">Error loading today's daily image.</p>
        <p class="text-center text-sm">Please try again later.</p>
      </div>
    </div>
    <GuessInput
      class="mt-4"
      v-model="guess"
      :disabled="!imageLoaded"
      :isLoading="dailiesStore.isLoading"
      :submitAction="submitGuess"
    />
  </div>
</template>
