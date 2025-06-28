<script setup lang="ts">
import { ref, onBeforeUnmount, onMounted } from 'vue'
import GuessInput from '@/components/common/GuessInput.vue'
import signalRService from '@/services/signalRService'
import { registerAudioEvents, unregisterAudioEvents } from '@/services/audioEventHandlers'

const announcerMessage = ref<string>('')

/** Send text to server for TTS streaming */
async function submitAnnouncerMessage(value: string) {
  if (!value) return
  await signalRService.invoke('TestAnnouncer', value)
}

onMounted(async () => {
  try {
    if (!signalRService.isConnected) {
      await signalRService.startConnection('/gamehub')
    }
    registerAudioEvents()
  } catch (err) {
    console.error('SignalR connection failed in RoomWrapper:', err)
  }
})

onBeforeUnmount(() => {
  unregisterAudioEvents()
})
</script>

<template>
  <GuessInput v-model="announcerMessage" :submitAction="submitAnnouncerMessage" />
</template>
