<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useDailiesStore } from '@/stores/dailies'
import GuessInput from '@/components/common/GuessInput.vue'
import { EyeIcon, Loader2Icon, OctagonAlertIcon, Share2Icon } from 'lucide-vue-next'
import api from '@/api/api'
import ClosenessDisplay from '../common/ClosenessDisplay.vue'

const guess = ref('')
const shareText = ref('Share Results')
const dailiesStore = useDailiesStore()
const {
  dailyQuestion,
  dailyAnswer,
  dailyError,
  showAssessment,
  imageLoaded,
  isAssessing,
  isLoadingDaily
} = storeToRefs(dailiesStore)

const defaultImageUrl = computed(() => {
  if (isLoading.value) return '/images/daily-image-loading.png'
  return '/images/daily-image-error.png'
})

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

const getEmojiForValue = (value: number): string => {
  if (value >= 7) {
    return '🟩'
  } else if (value >= 4) {
    return '🟨'
  } else {
    return '⬛'
  }
}

const copyClosenessArrayToClipboard = async () => {
  if (!dailyAnswer.value.closenessArray || dailyAnswer.value.closenessArray.length === 0) {
    console.warn('Closeness array is empty, nothing to copy.')
    return
  }
  try {
    const emojiString = dailyAnswer.value.closenessArray.map(getEmojiForValue).join('')
    const formattedString = `My score today was: ${dailyAnswer.value.score}%\nHere is my spoiler free guess: ${emojiString}\nTry to beat my score at ${window.location.origin}`
    await navigator.clipboard.writeText(formattedString)
    console.debug('Closeness emoji copied to clipboard:', emojiString)

    shareText.value = 'Copied to Clipboard!'
    setTimeout(() => {
      shareText.value = 'Share Results'
    }, 3000)
  } catch (err) {
    console.error('Failed to copy closeness emoji to clipboard:', err)
    // Optional: Notify user of failure
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
        class="animate-fade-blur-in -my-1 aspect-[3/4] h-auto w-full object-contain"
        @error="handleImageError"
      />
      <!-- loading scree -->
      <div
        v-if="isLoading.value"
        class="absolute inset-0 flex flex-col items-center justify-center bg-black bg-opacity-80 text-white"
      >
        <div class="flex flex-col items-center justify-center">
          <Loader2Icon class="h-12 w-12 animate-spin" />
        </div>
      </div>
      <!-- error screen -->
      <div
        v-else-if="dailyError"
        class="absolute inset-0 flex flex-col items-center justify-center bg-black bg-opacity-80 text-white"
      >
        <div class="flex flex-col items-center justify-center">
          <OctagonAlertIcon class="mb-4 h-12 w-12 text-red-500" />
          <p class="text-center text-lg font-semibold">Error loading today's daily image.</p>
          <p class="text-center text-sm">Please try again later.</p>
        </div>
      </div>
      <!-- assessment stats -->
      <div
        v-else-if="showAssessment"
        class="absolute inset-0 flex flex-col items-center justify-center bg-black bg-opacity-90 text-white"
      >
        <div class="flex flex-col items-center justify-center">
          <p class="text-lg">Score: {{ dailyAnswer.score }}</p>
          <p class="text-lg font-semibold">Your guess:</p>
          <p class="text-sm">{{ dailyAnswer.guess }}</p>
          <ClosenessDisplay :closenessArray="dailyAnswer.closenessArray" />
          <p class="mx-6 mt-8 text-sm">{{ dailyAnswer.reason }}</p>
        </div>
      </div>
    </div>
    <GuessInput
      v-if="!dailiesStore.hasDailyAnswer"
      class="mt-4"
      v-model="guess"
      :isLoading="isLoading.value"
      :submitAction="submitGuess"
    />
    <div class="relative flex w-full pt-4" v-else>
      <button
        class="btn-default ml-2 flex h-12 w-full items-center justify-center rounded-full"
        @click="dailiesStore.setShowAssessment(!showAssessment)"
      >
        <Loader2Icon v-if="isLoading.value" class="mr-4 h-5 w-5 animate-spin" />
        <EyeIcon v-else class="mr-4 h-4 w-4" />
        {{ showAssessment ? 'Show Picture' : 'Show Stats' }}
      </button>
      <button
        class="btn-default ml-2 flex h-12 w-full items-center justify-center rounded-full"
        @click="copyClosenessArrayToClipboard"
      >
        <Loader2Icon v-if="isLoading.value" class="mr-4 h-5 w-5 animate-spin" />
        <Share2Icon v-else class="mr-4 h-4 w-4" />
        {{ shareText }}
      </button>
    </div>
  </div>
</template>
