<script setup lang="ts">
import GameTimer from '../GameTimer.vue'
import GuessInput from '@/components/common/GuessInput.vue'
import { ref } from 'vue'
import ShinyButton from '@/components/common/ShinyButton.vue'
import UnshinyButton from '@/components/common/UnshinyButton.vue'

const timeoutPerQuestion = 30000
const emit = defineEmits<{
  (e: 'promptSubmitted', prompt: string): void
}>()

const promptInputFromComponent = ref<string>('')

const submitPrompt = (valueFromInput: string) => {
  if (valueFromInput === '') {
    return
  }
  emit('promptSubmitted', valueFromInput)
  promptInputFromComponent.value = ''
}

const submitEmpty = () => {
  submitPrompt(' ')
}
</script>

<template>
  <div class="flex min-h-screen w-full items-center justify-center">
    <GameTimer :max-time="timeoutPerQuestion" class="fixed bottom-0 left-0 right-0" />
    <div class="flex w-full max-w-5xl flex-col items-center px-4">
      <ShinyButton class="mb-4 cursor-default" :disabled="true">
        <h2 class="text-lg sm:text-2xl">Create your masterpiece!</h2>
      </ShinyButton>

      <div
        class="relative z-10 mx-4 mb-4 flex w-fit cursor-pointer flex-col items-center space-y-2 rounded-lg bg-gray-500/10 p-4 px-6 backdrop-blur hover:bg-gray-500/20 dark:bg-white/10 dark:hover:bg-white/20"
      >
        Add as much detail as possible to create the best picture!
      </div>
      <GuessInput
        v-model="promptInputFromComponent"
        class="w-full sm:max-w-xl"
        placeholder="Create your image prompt"
        :submitAction="submitPrompt"
        :isListeningAtStart="true"
      />
      <UnshinyButton class="mt-2" @click="submitEmpty">Create one for me!</UnshinyButton>
    </div>
  </div>
</template>
