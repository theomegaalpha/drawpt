<script setup lang="ts">
import { watch, computed } from 'vue'
import { useSpeechRecognition } from '@/composables/useSpeechRecognition'
import { MicIcon, SendIcon } from 'lucide-vue-next'
import StandardInput from './StandardInput.vue'

const props = defineProps<{
  modelValue: string
  submitAction: (value: string) => void
}>()

const emit = defineEmits(['update:modelValue'])

// Computed property to handle v-model logic for GuessInput
const inputValue = computed({
  get: () => props.modelValue,
  set: (value) => {
    emit('update:modelValue', value)
  }
})

const { transcribedText, isListening, toggleListening } = useSpeechRecognition()

const handleRecordButtonMouseDown = () => {
  if (!isListening.value) {
    toggleListening()
  }
}

const handleRecordButtonMouseUp = () => {
  if (isListening.value) {
    toggleListening()
  }
}

watch(transcribedText, (newText) => {
  if (newText !== undefined && newText !== null && newText !== inputValue.value) {
    inputValue.value = newText
  }
})

const localSubmitGuess = () => {
  props.submitAction(inputValue.value)
}
</script>

<template>
  <div class="relative flex w-full">
    <div class="relative w-full">
      <!-- Use inputValue for v-model on StandardInput. -->
      <StandardInput
        placeholder="Guess the prompt"
        v-model="inputValue"
        @keyup.enter="localSubmitGuess"
      />
      <button
        @mousedown="handleRecordButtonMouseDown"
        @mouseup="handleRecordButtonMouseUp"
        @mouseleave="handleRecordButtonMouseUp"
        class="absolute right-4 top-1/2 -translate-y-1/2 transform text-zinc-400 transition-colors hover:text-white"
        aria-label="Use microphone"
      >
        <MicIcon class="h-5 w-5" />
      </button>
    </div>
    <button
      class="btn-primary ml-2 rounded-full"
      :disabled="inputValue === ''"
      @click="localSubmitGuess"
    >
      <SendIcon class="h-4 w-4" />
    </button>
  </div>
</template>
