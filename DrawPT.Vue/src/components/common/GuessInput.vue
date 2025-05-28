<script setup lang="ts">
import { watch } from 'vue'
import { useSpeechRecognition } from '@/composables/useSpeechRecognition'
import StandardInput from '@/components/common/StandardInput.vue'

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
    <StandardInput
      class="flex-grow rounded border border-gray-300 px-2 py-1 text-black shadow-inner"
      placeholder="Type or hold 🎤 to speak"
      :modelValue="props.modelValue"
      @keyup.enter="localSubmitGuess"
      @input="handleInput"
    />
    <button
      class="btn ml-2"
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
    <button class="btn-primary ml-2" :disabled="modelValue === ''" @click="localSubmitGuess">
      Submit
    </button>
  </div>
</template>
