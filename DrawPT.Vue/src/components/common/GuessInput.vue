'''
<script setup lang="ts">
import { watch } from 'vue'
import { useSpeechRecognition } from '@/composables/useSpeechRecognition'

const props = defineProps<{
  modelValue: string
  submitAction: (value: string) => void
}>()

const emit = defineEmits(['update:modelValue'])

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
  console.log('Transcribed text:', newText)
  if (newText && newText !== '') {
    emit('update:modelValue', newText)
  }
})

const handleInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.value)
}

const localSubmitGuess = () => {
  props.submitAction(props.modelValue)
}
</script>

<template>
  <div class="mt-2 flex items-center rounded-lg shadow">
    <input
      class="flex-grow rounded border border-gray-300 px-2 py-1 text-black shadow-inner"
      type="text"
      :value="modelValue"
      @input="handleInput"
      @keyup.enter="localSubmitGuess"
      placeholder="Type or hold 🎤 to speak"
    />
    <button
      class="ml-2 rounded border px-4 py-1"
      :class="{
        'border-green-700 bg-green-500 hover:bg-green-700': !isListening,
        'border-red-700 bg-red-500 text-white hover:bg-red-700': isListening
      }"
      @mousedown="handleRecordButtonMouseDown"
      @mouseup="handleRecordButtonMouseUp"
      @mouseleave="handleRecordButtonMouseUp"
    >
      {{ isListening ? '...' : '🎤' }}
    </button>
    <button
      class="ml-2 rounded border border-blue-700 bg-blue-500 px-4 py-1 hover:bg-blue-700 hover:disabled:bg-blue-500"
      :disabled="modelValue === ''"
      @click="localSubmitGuess"
    >
      Submit
    </button>
  </div>
</template>
'''
