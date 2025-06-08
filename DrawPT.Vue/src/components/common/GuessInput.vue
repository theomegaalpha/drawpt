<script setup lang="ts">
import { watch } from 'vue'
import { useSpeechRecognition } from '@/composables/useSpeechRecognition'
import { MicIcon, SendIcon } from 'lucide-vue-next'

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
  <div class="relative flex w-full">
    <div class="relative w-full">
      <input
        type="text"
        placeholder="Guess the prompt"
        :value="modelValue"
        @input="handleInput"
        class="w-full rounded-full bg-zinc-900 px-5 py-3 pr-12 text-white placeholder-zinc-500 focus:outline-none"
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
      :disabled="modelValue === ''"
      @click="localSubmitGuess"
    >
      <SendIcon class="h-4 w-4" />
    </button>
  </div>
</template>
