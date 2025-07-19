<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '@/api/api'
import type { Feedback } from '@/models/feedback'

const feedbacks = ref<Feedback[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
// Pagination state
const currentPage = ref(1)
const hasMore = ref(false)

async function loadFeedback(page = 1) {
  loading.value = true
  error.value = null
  api
    .getFeedback(page)
    .then((response) => {
      feedbacks.value = response
      hasMore.value = response.length === 50
    })
    .catch((err) => {
      console.error('Failed to load feedback:', err)
      error.value = 'Failed to load feedback. Please try again later.'
    })
    .finally(() => {
      loading.value = false
    })
}

onMounted(() => loadFeedback(currentPage.value))
// Pagination handlers
function prevPage() {
  if (currentPage.value > 1) {
    currentPage.value--
    loadFeedback(currentPage.value)
  }
}
function nextPage() {
  if (hasMore.value) {
    currentPage.value++
    loadFeedback(currentPage.value)
  }
}
</script>

<template>
  <div class="h-full p-6">
    <h1 class="mb-8 text-3xl font-bold">Feedback Management</h1>
    <div v-if="loading" class="py-6 text-center">Loading feedback...</div>
    <div v-else>
      <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-800">
          <tr>
            <th
              class="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500 dark:text-gray-400"
            >
              Type
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500 dark:text-gray-400"
            >
              Message
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500 dark:text-gray-400"
            >
              Created At
            </th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 bg-white dark:divide-gray-700 dark:bg-gray-900">
          <tr v-for="fb in feedbacks" :key="fb.id" class="hover:bg-gray-50 dark:hover:bg-gray-800">
            <td class="whitespace-nowrap px-6 py-4">{{ fb.type }}</td>
            <td class="break-words px-6 py-4">{{ fb.message }}</td>
            <td class="whitespace-nowrap px-6 py-4">
              {{ new Date(fb.createdAt).toLocaleString() }}
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="error" class="mt-4 text-red-500">{{ error }}</p>
      <!-- Pagination Controls -->
      <div class="mt-4 flex items-center justify-between">
        <button
          @click="prevPage"
          :disabled="currentPage === 1"
          class="btn-default px-4 py-2"
          :class="{ 'cursor-not-allowed opacity-50': currentPage === 1 }"
        >
          Previous
        </button>
        <span>Page {{ currentPage }}</span>
        <button
          @click="nextPage"
          :disabled="!hasMore"
          class="btn-default px-4 py-2"
          :class="{ 'cursor-not-allowed opacity-50': !hasMore }"
        >
          Next
        </button>
      </div>
    </div>
  </div>
</template>
