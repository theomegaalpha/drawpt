<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { getFeedback as fetchFeedback, updateFeedback } from '@/api/miscApi'
import type { Feedback } from '@/models/feedback'

const feedbacks = ref<Feedback[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
const currentPage = ref(1)
const hasMore = ref(false)

const showResolved = ref<boolean>(false)

async function loadFeedback(page = 1) {
  loading.value = true
  error.value = null
  try {
    const response = await fetchFeedback(page, showResolved.value)
    feedbacks.value = response
    hasMore.value = response.length === 50
  } catch (err) {
    console.error('Failed to load feedback:', err)
    error.value = 'Failed to load feedback. Please try again later.'
  } finally {
    loading.value = false
  }
}

onMounted(() => loadFeedback(currentPage.value))
watch(showResolved, () => {
  currentPage.value = 1
  loadFeedback(1)
})

function toggleResolved(fb: Feedback) {
  fb.isResolved = !fb.isResolved
  updateFeedback({ ...fb, isResolved: fb.isResolved }).catch((err) => {
    console.error('Failed to resolve feedback:', err)
    fb.isResolved = !fb.isResolved
  })
}

function formatLocalDate(utcString: string): string {
  const d = new Date(utcString.endsWith('Z') ? utcString : utcString + 'Z')
  return d.toLocaleString()
}

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
      <div class="mb-4 flex items-center">
        <input
          id="showResolved"
          type="checkbox"
          v-model="showResolved"
          class="h-4 w-4 rounded text-indigo-600 focus:ring-indigo-500"
        />
        <label for="showResolved" class="ml-2 text-sm text-gray-700 dark:text-gray-300">
          Show Resolved
        </label>
      </div>
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
            <th
              class="px-6 py-3 text-center text-xs font-medium uppercase text-gray-500 dark:text-gray-400"
            >
              Resolved
            </th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 bg-white dark:divide-gray-700 dark:bg-gray-900">
          <tr v-for="fb in feedbacks" :key="fb.id" class="hover:bg-gray-50 dark:hover:bg-gray-800">
            <td class="whitespace-nowrap px-6 py-4">{{ fb.type }}</td>
            <td class="break-words px-6 py-4">{{ fb.message }}</td>
            <td class="whitespace-nowrap px-6 py-4">
              {{ formatLocalDate(fb.createdAt) }}
            </td>
            <td class="px-6 py-4 text-center">
              <input
                type="checkbox"
                :checked="fb.isResolved"
                @change="toggleResolved(fb)"
                :disabled="fb.isResolved"
                class="h-4 w-4 rounded text-indigo-600 focus:ring-indigo-500"
              />
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
