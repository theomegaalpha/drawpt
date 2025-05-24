<script setup lang="ts">
import { useNotificationStore } from '@/stores/notifications'
const notificationStore = useNotificationStore()
const { gameNotifications } = notificationStore
</script>

<template>
  <div class="fixed bottom-4 right-4">
    <transition-group
      enter-from-class="translate-x-[150%] opacity-0"
      leave-to-class="translate-x-[150%] opacity-0"
      enter-active-class="transition duration-300"
      leave-active-class="transition duration-300"
      tag="div"
    >
      <div
        class="flex flex-col items-end"
        v-for="(notification, index) in gameNotifications"
        :key="index"
      >
        <div
          id="notification"
          class="mt-2 w-fit rounded-lg border border-zinc-400 bg-white p-2 px-5 transition-all duration-300 dark:border-zinc-500 dark:bg-zinc-800"
          :class="{
            'border-red-600 bg-red-200 pr-8 font-semibold text-red-800 motion-safe:animate-[pulse_1s_ease-in-out]':
              notification.isAlert
          }"
        >
          <span v-if="notification.isAlert" class="absolute right-3 top-4 flex h-3 w-3">
            <span
              class="absolute inline-flex h-full w-full animate-ping rounded-full bg-red-400 opacity-75"
            ></span>
            <span class="relative inline-flex h-3 w-3 rounded-full bg-red-500"></span>
          </span>
          {{ notification.message }}
        </div>
      </div>
    </transition-group>
  </div>
</template>
