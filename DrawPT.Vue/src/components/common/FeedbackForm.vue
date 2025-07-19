<script setup lang="ts">
import { ref } from 'vue'
import { Loader2Icon } from 'lucide-vue-next'

// Feedback form state
const feedbackTypes = ['Bug report', 'Feature request', 'General comment']
const selectedType = ref<string>(feedbackTypes[0])
const message = ref<string>('')
const isSubmitting = ref<boolean>(false)
const isSuccess = ref<boolean>(false)

const submitFeedback = async () => {
  if (isSubmitting.value || message.value.trim() === '' || isSuccess.value) return
  isSubmitting.value = true
  try {
    console.log('Feedback submitted', { type: selectedType.value, message: message.value })

    message.value = ''
    isSuccess.value = true
    setTimeout(() => (isSuccess.value = false), 6000)
  } catch (error) {
    console.error('Failed to submit feedback:', error)
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <form @submit.prevent="submitFeedback" class="space-y-6">
    <div>
      <label for="feedback-type" class="block text-sm font-medium text-gray-700 dark:text-gray-300"
        >Feedback Type</label
      >
      <select
        id="feedback-type"
        v-model="selectedType"
        class="mt-1 block w-full rounded-md border-gray-300 p-1 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 dark:bg-zinc-900"
      >
        <option v-for="(type, idx) in feedbackTypes" :key="idx" :value="type" class="p-1">
          {{ type }}
        </option>
      </select>
    </div>
    <div>
      <label
        for="feedback-message"
        class="block text-sm font-medium text-gray-700 dark:text-gray-300"
        >Your Feedback</label
      >
      <textarea
        id="feedback-message"
        v-model="message"
        rows="4"
        class="mt-1 block w-full rounded-md border-gray-300 p-2 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 dark:bg-zinc-900"
        placeholder="Let us know your thoughts..."
      ></textarea>
    </div>
    <div>
      <button
        type="submit"
        class="btn-default flex w-full items-center justify-center space-x-2"
        :disabled="isSubmitting"
        :class="{ 'cursor-not-allowed opacity-50': isSubmitting }"
      >
        <Loader2Icon v-if="isSubmitting" class="h-5 w-5 animate-spin" />
        <span>
          {{
            isSuccess
              ? 'Successfully Sent Feedback'
              : isSubmitting
                ? 'Submitting...'
                : 'Submit Feedback'
          }}
        </span>
      </button>
    </div>
  </form>
</template>
