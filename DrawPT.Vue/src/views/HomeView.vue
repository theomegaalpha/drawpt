<script setup lang="ts">
import TheEntry from '@/components/home/TheEntry.vue'
import Header from '@/components/header/Header.vue'
</script>

<template>
  <div class="min-h-screen">
    <Header />
    <TheEntry />
    <footer class="bg-indigo-900 py-6 text-white dark:bg-zinc-950">
      <div class="container mx-auto px-6">
        <div class="flex flex-col items-center justify-between md:flex-row">
          <div class="mb-4 md:mb-0">
            <h2 class="text-xl font-bold">DrawPT</h2>
            <p class="text-indigo-200 dark:text-indigo-400">
              The ultimate AI drawing guessing game
            </p>
          </div>
          <div class="flex gap-4">
            <a href="#" class="text-white hover:text-indigo-200 dark:hover:text-indigo-400"
              >About</a
            >
            <a href="#" class="text-white hover:text-indigo-200 dark:hover:text-indigo-400"
              >Contact</a
            >
            <a href="#" class="text-white hover:text-indigo-200 dark:hover:text-indigo-400"
              >Privacy</a
            >
            <a href="#" class="text-white hover:text-indigo-200 dark:hover:text-indigo-400"
              >Terms</a
            >
          </div>
        </div>
        <div
          class="mt-6 border-t border-indigo-800 pt-6 text-center text-indigo-200 dark:border-indigo-700 dark:text-indigo-400"
        >
          <p>© 2025 DrawPT. All rights reserved.</p>
        </div>
      </div>
    </footer>
  </div>
</template>
