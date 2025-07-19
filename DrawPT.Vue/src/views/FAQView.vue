<script setup lang="ts">
// No script needed for static content
import AppHeader from '@/components/common/AppHeader.vue'
import AppFooter from '@/components/common/AppFooter.vue'
import { ref } from 'vue'

// Stubbed FAQ items
type FaqItem = { question: string; answer: string }
const faqs: FaqItem[] = [
  {
    question: 'What is DrawPT?',
    answer:
      'DrawPT is a real-time drawing and guessing game powered by AI-driven scoring and feedback.'
  },
  {
    question: 'How do I join a game room?',
    answer: 'You can join by entering a room code on the home page or by clicking an invite link.'
  },
  {
    question: 'How is scoring determined?',
    answer:
      'Our AI model awards partial credit for close matches, making scoring more flexible and fun.'
  },
  {
    question: 'Can I play on mobile devices?',
    answer: 'Yes! DrawPT is fully responsive and works on both desktop and mobile browsers.'
  }
]
// Track which FAQ is open
const openIndex = ref<number | null>(null)
const toggle = (idx: number) => {
  openIndex.value = openIndex.value === idx ? null : idx
}
</script>

<template>
  <div class="h-full">
    <AppHeader />
    <section class="mx-auto max-w-4xl px-4 py-12">
      <h1 class="mb-4 text-center text-4xl font-bold">FAQ</h1>
      <div class="space-y-4">
        <div v-for="(item, index) in faqs" :key="index" class="border-b pb-2">
          <button
            @click="toggle(index)"
            class="flex w-full items-center justify-between py-2 text-left font-medium hover:text-blue-600"
          >
            <span>{{ item.question }}</span>
            <svg
              :class="{ 'rotate-180 transform': openIndex === index }"
              class="h-5 w-5 transition-transform"
              viewBox="0 0 20 20"
              fill="currentColor"
            >
              <path
                fill-rule="evenodd"
                d="M5.23 7.21a.75.75 0 011.06.02L10 10.586l3.71-3.355a.75.75 0 111.02 1.09l-4 3.614a.75.75 0 01-1.02 0l-4-3.614a.75.75 0 01.02-1.06z"
                clip-rule="evenodd"
              />
            </svg>
          </button>
          <div v-if="openIndex === index" class="mt-2 text-gray-700 dark:text-gray-300">
            {{ item.answer }}
          </div>
        </div>
      </div>
    </section>
  </div>
  <AppFooter />
</template>
