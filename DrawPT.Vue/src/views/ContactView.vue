<script setup lang="ts">
// No script needed for static content
import AppHeader from '@/components/common/AppHeader.vue'
import AppFooter from '@/components/common/AppFooter.vue'
import FeedbackForm from '@/components/common/FeedbackForm.vue'

// Stubbed contact methods
type ContactItem = { label: string; info: string; url: string }
const contacts: ContactItem[] = [
  { label: 'Email', info: 'david@drawpt.ai', url: 'mailto:david@drawpt.ai' },
  { label: 'Instagram', info: '@drawpt.ai', url: 'https://instagram.com/drawpt.ai' },
  { label: 'Feedback Form', info: 'Fill out the feedback form below.', url: '' }
]
</script>

<template>
  <div class="flex min-h-screen flex-col">
    <AppHeader />
    <main class="flex-grow">
      <section class="mx-auto max-w-4xl px-4 py-12">
        <h1 class="mb-4 text-center text-4xl font-bold">Contact Us</h1>
        <div class="space-y-4">
          <div v-for="(item, index) in contacts" :key="index" class="flex items-start">
            <span class="w-32 font-medium">{{ item.label }}:</span>
            <span class="text-blue-600" v-if="item.url">
              <a :href="item.url" target="_blank" rel="noopener noreferrer">{{ item.info }}</a>
            </span>
            <span v-else>{{ item.info }}</span>
          </div>
        </div>
        <div class="mt-12">
          <h2 class="mb-4 text-2xl font-semibold">Send Us Feedback</h2>
          <FeedbackForm />
        </div>
      </section>
    </main>
    <AppFooter />
  </div>
</template>
