<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useDailiesStore } from '@/stores/dailies'
import GuessInput from '@/components/common/GuessInput.vue'
import { Loader2Icon, OctagonAlertIcon } from 'lucide-vue-next'
import api from '@/api/api'

const guess = ref('')
const dailiesStore = useDailiesStore()
const { dailyQuestion, dailyAnswer, imageLoaded, isAssessing, isLoadingDaily } =
  storeToRefs(dailiesStore)

const defaultImageUrl = '/images/daily-image-error.png'

const handleImageError = (event: Event) => {
  console.error('Error loading daily image:', event)
}

const isLoading = computed(() => isAssessing || isLoadingDaily)

const submitGuess = () => {
  var myGuess = guess.value.trim()
  if (myGuess) {
    console.log('Submitted guess:', myGuess)
    dailiesStore.setIsLoading(true)
    dailiesStore.setIsAssessing(true)
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
        dailiesStore.setIsAssessing(false)
      })
  }
}

onMounted(() => {
  dailiesStore.initStore()
})
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
        v-if="!imageLoaded || isLoading.value || dailyAnswer"
        class="absolute inset-0 flex flex-col items-center justify-center bg-black bg-opacity-50 text-white dark:bg-opacity-70"
      >
        <!-- assessment stats -->
        <div
          v-if="dailyAnswer && dailyAnswer.guess"
          class="flex flex-col items-center justify-center"
        >
          <p class="text-lg font-semibold">Your guess:</p>
          <p class="text-sm">{{ dailyAnswer.guess }}</p>
        </div>
        <!-- loading scree -->
        <div v-if="isLoading.value" class="flex flex-col items-center justify-center">
          <Loader2Icon class="h-12 w-12 animate-spin" />
        </div>
        <!-- error screen -->
        <div class="flex flex-col items-center justify-center" v-else-if="!dailyAnswer">
          <OctagonAlertIcon class="mb-4 h-12 w-12 text-red-500" />
          <p class="text-center text-lg font-semibold">Error loading today's daily image.</p>
          <p class="text-center text-sm">Please try again later.</p>
        </div>
      </div>
    </div>
    <GuessInput
      class="mt-4"
      v-model="guess"
      :disabled="!imageLoaded"
      :isLoading="isLoading.value"
      :submitAction="submitGuess"
    />
  </div>
</template>
