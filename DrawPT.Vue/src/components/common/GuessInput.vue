<script setup lang="ts">
import { watch, computed, ref, onMounted } from 'vue'
import { useSpeechRecognition } from '@/composables/useSpeechRecognition'
import { MicIcon, MicOffIcon, SendIcon, Loader2Icon } from 'lucide-vue-next'
import GameTextInput from './GameTextInput.vue'

const props = defineProps<{
  modelValue: string
  submitAction: (value: string) => void
  placeholder?: string
  isListeningAtStart?: boolean
  isLoading?: boolean
  disabled?: boolean
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

// Toggle listening on button click
const handleToggleListening = () => {
  if (props.disabled) return
  toggleListening()
}

const pulse = ref(false)

watch(transcribedText, (newText) => {
  if (newText !== undefined && newText !== null && newText !== inputValue.value) {
    inputValue.value = newText
  }
  // trigger pulse glow on new speech
  pulse.value = true
  setTimeout(() => {
    pulse.value = false
  }, 300)
})

const localSubmitGuess = () => {
  props.submitAction(inputValue.value)
}

const onKeydown = (e: KeyboardEvent) => {
  if (e.key === 'Enter') {
    if (!e.shiftKey && !e.ctrlKey) {
      e.preventDefault()
      localSubmitGuess()
    }
  }
}

onMounted(() => {
  if (props.isListeningAtStart) {
    toggleListening()
  }
})
</script>

<template>
  <form @submit.prevent="localSubmitGuess" class="relative flex items-center">
    <div class="relative w-full">
      <GameTextInput
        :placeholder="props.placeholder || 'Guess the prompt'"
        v-model="inputValue"
        :disabled="props.disabled"
        :isLoading="props.isLoading"
        @keydown="onKeydown"
      />
      <button
        type="button"
        @click="handleToggleListening"
        :disabled="props.disabled || props.isLoading"
        class="absolute left-4 top-1/2 -translate-y-1/2 transform text-zinc-400 transition-colors"
        :class="{
          'cursor-not-allowed': props.disabled,
          'hover:text-white': !props.disabled,
          'cursor-wait': props.isLoading
        }"
        aria-label="Toggle microphone"
      >
        <Loader2Icon v-if="props.isLoading" class="h-5 w-5 animate-spin" />
        <MicIcon
          v-else-if="isListening"
          :class="['h-5 w-5 rounded-full text-green-500', { glow: isListening, pulse: pulse }]"
        />
        <MicOffIcon v-else class="h-5 w-5 text-red-500" />
      </button>
    </div>
    <button
      type="submit"
      class="btn-primary ml-2 flex h-12 w-12 items-center justify-center rounded-full"
      :class="{
        'cursor-not-allowed': disabled,
        'hover:text-white': !disabled,
        'cursor-wait': props.isLoading
      }"
      :disabled="inputValue === '' || props.isLoading"
    >
      <Loader2Icon v-if="props.isLoading" class="h-5 w-5 animate-spin" />
      <SendIcon v-else class="h-4 w-4" />
    </button>
  </form>
</template>

<style scoped>
@keyframes pulse {
  0% {
    box-shadow: 0 0 8px rgba(0, 255, 0, 0.5);
  }
  50% {
    box-shadow: 0 0 16px rgba(0, 255, 0, 0.7);
  }
  100% {
    box-shadow: 0 0 8px rgba(0, 255, 0, 0.5);
  }
}
.glow {
  box-shadow: 0 0 8px rgba(0, 255, 0, 0.5);
}
.pulse {
  animation: pulse 0.3s ease-out;
}
</style>
