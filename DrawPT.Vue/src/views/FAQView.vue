<script setup lang="ts">
// No script needed for static content
import AppHeader from '@/components/common/AppHeader.vue'
import AppFooter from '@/components/common/AppFooter.vue'
import { ref } from 'vue'
import { ChevronDownIcon } from 'lucide-vue-next'

// Stubbed FAQ items
type FaqItem = { question: string; answer: string }
const faqs: FaqItem[] = [
  {
    question: 'Are the models trained on my answers?',
    answer:
      'Nope. DrawPT uses pre-trained models that do not learn from individual player inputs. Your privacy is respected.'
  },
  {
    question: 'Is DrawPT really free?',
    answer: 'Yes! DrawPT will remain free as long as I can afford server costs with my day job.'
  },
  {
    question: 'How is scoring determined?',
    answer:
      "Scoring is done completely by the AI model; which explains why the same answer can yield different scores. It's subjective like a human judge would be."
  },
  {
    question: 'What models are used?',
    answer:
      'A combination of gpt 4.1 mini, gpt 4.1, gpt 4o mini tts and Freepik for the art generation.'
  }
]
// Track which FAQ is open
const openIndex = ref<number | null>(null)
const toggle = (idx: number) => {
  openIndex.value = openIndex.value === idx ? null : idx
}
</script>

<template>
  <div class="flex min-h-screen flex-col">
    <AppHeader />
    <main class="flex-grow">
      <section class="mx-auto max-w-4xl px-4 py-12">
        <h1 class="mb-4 text-center text-4xl font-bold">FAQ</h1>
        <div class="space-y-4">
          <div v-for="(item, index) in faqs" :key="index" class="border-b pb-2">
            <button
              @click="toggle(index)"
              class="flex w-full items-center justify-between py-2 text-left font-medium hover:text-blue-600"
            >
              <span>{{ item.question }}</span>
              <ChevronDownIcon
                class="h-5 w-5 transition-transform"
                :class="{ 'rotate-180': openIndex === index }"
              />
            </button>
            <transition
              enter-active-class="transition-all duration-400 ease-out"
              enter-from-class="max-h-0 opacity-0"
              enter-to-class="max-h-40 opacity-100"
            >
              <div
                v-if="openIndex === index"
                class="mt-2 overflow-hidden text-gray-700 dark:text-gray-300"
              >
                {{ item.answer }}
              </div>
            </transition>
          </div>
        </div>
      </section>
    </main>
    <AppFooter />
  </div>
</template>
