'''
<script setup lang="ts">
import { ref, watch } from 'vue'
import { useSpeechRecognition } from '@/composables/useSpeechRecognition'

const props = defineProps<{
  modelValue: string
  submitAction: (value: string) => void
}>()

const emit = defineEmits(['update:modelValue'])

const { transcribedText, interimText, isListening, toggleListening } = useSpeechRecognition()

// Store the text that was in the input before speech recognition started
const textBeforeSpeech = ref('')

const handleRecordButtonMouseDown = () => {
  if (!isListening.value) {
    textBeforeSpeech.value = props.modelValue // Capture current input
    // toggleListening will clear transcribedText and interimText in the composable
    toggleListening()
  }
}

const handleRecordButtonMouseUp = () => {
  if (isListening.value) {
    toggleListening()
  }
}

// Update the input with interim results, appended to the text that existed before speech
watch(interimText, (newInterim) => {
  if (isListening.value) {
    // Only update if listening and interim text is available
    emit('update:modelValue', textBeforeSpeech.value + newInterim)
  }
})

// Update the input with final results, appended to the text that existed before speech
watch(transcribedText, (newFinal) => {
  if (newFinal) {
    // Only update if final text is available
    emit('update:modelValue', textBeforeSpeech.value + newFinal)
  }
})

const handleInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  // If user types manually, this becomes the new base for any subsequent speech input,
  // or it's just a manual edit.
  if (!isListening.value) {
    textBeforeSpeech.value = '' // Reset if user types, indicating a new base or manual edit
  }
  emit('update:modelValue', target.value)
}

const localSubmitGuess = () => {
  props.submitAction(props.modelValue)
}
</script>

<template>
  <div
    class="mt-2 flex items-center rounded-lg border border-gray-200 bg-white p-2 shadow dark:border-gray-700 dark:bg-gray-800"
  >
    <input
      class="flex-grow rounded border border-gray-300 px-2 py-1 text-black shadow-inner"
      type="text"
      :value="modelValue"
      @input="handleInput"
      @keyup.enter="localSubmitGuess"
      placeholder="Type or hold 🎤 to speak"
    />
    <button
      class="ml-2 rounded border px-4 py-2"
      :class="{
        'border-green-700 bg-green-500 hover:bg-green-700': !isListening,
        'border-red-700 bg-red-500 text-white hover:bg-red-700': isListening
      }"
      @mousedown="handleRecordButtonMouseDown"
      @mouseup="handleRecordButtonMouseUp"
      @mouseleave="handleRecordButtonMouseUp"
    >
      {{ isListening ? 'Listening...' : '🎤' }}
    </button>
    <button
      class="ml-2 rounded border border-blue-700 bg-blue-500 px-4 py-2 hover:bg-blue-700 hover:disabled:bg-blue-500"
      :disabled="modelValue === ''"
      @click="localSubmitGuess"
    >
      Submit
    </button>
  </div>
</template>
'''
