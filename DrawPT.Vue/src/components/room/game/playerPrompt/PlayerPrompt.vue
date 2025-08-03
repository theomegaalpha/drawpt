<script setup lang="ts">
import GameTimer from '../GameTimer.vue'
import GuessInput from '@/components/common/GuessInput.vue'
import { computed, ref } from 'vue'
import { useGameStateStore } from '@/stores/gameState'
import ShinyButton from '@/components/common/ShinyButton.vue'

const timeoutPerQuestion = 30000
const emit = defineEmits<{
  (e: 'guessSubmitted', guess: string): void
}>()

const gameStateStore = useGameStateStore()

const lockGuess = computed(() => gameStateStore.isGuessLocked)

const guessInputFromComponent = ref<string>('')

const submitGuess = async (valueFromInput: string) => {
  if (valueFromInput === '' || lockGuess.value) {
    return
  }
  emit('guessSubmitted', valueFromInput)
  guessInputFromComponent.value = ''
}
</script>

<template>
  <div class="flex min-h-screen w-full items-center justify-center">
    <GameTimer
      :max-time="timeoutPerQuestion"
      v-if="!lockGuess"
      class="fixed bottom-0 left-0 right-0"
    />
    <div class="flex w-full max-w-5xl flex-col items-center px-4">
      <ShinyButton class="mb-4 cursor-default" :disabled="true">
        <h2 class="text-lg sm:text-2xl">Create your masterpiece!</h2>
      </ShinyButton>
      <GuessInput
        v-model="guessInputFromComponent"
        class="w-full sm:max-w-xl"
        placeholder="Type in your image prompt here"
        :submitAction="submitGuess"
        :isListeningAtStart="true"
      />
    </div>
  </div>
</template>
