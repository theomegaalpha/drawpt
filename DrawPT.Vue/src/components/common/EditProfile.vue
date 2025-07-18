<script setup lang="ts">
import { ref, computed, defineProps } from 'vue'
import { storeToRefs } from 'pinia'
import { usePlayerStore } from '@/stores/player'
import StandardInput from '@/components/common/StandardInput.vue'
import { Loader2Icon } from 'lucide-vue-next'
import api from '@/api/api'
const {
  header = 'Set Username',
  buttonText = 'Save Changes',
  isLoadingOverride = false
} = defineProps<{
  header?: string
  buttonText?: string
  isLoadingOverride: boolean
}>()
const emit = defineEmits(['saved'])

const playerStore = usePlayerStore()
const { player, blankAvatar, avatarOptions } = storeToRefs(playerStore)

const defaultAvatar = computed(() => player.value.avatar || blankAvatar.value)
const selectedAvatar = ref('')
const usernameError = ref('')
const birthday = ref('')
const showAvatarOptions = ref(false)
const isLoading = ref(false)

const updateProfile = async () => {
  if (isLoading.value) return
  try {
    isLoading.value = true
    player.value.avatar = selectedAvatar.value || defaultAvatar.value
    await api.updatePlayer(player.value)
    playerStore.updatePlayer(player.value)
    emit('saved')
  } catch (error) {
    console.error('Failed to update profile:', error)
  } finally {
    isLoading.value = false
  }
}

const toggleShowAvatarOptions = () => {
  showAvatarOptions.value = !showAvatarOptions.value
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center px-4 py-12 sm:px-6 lg:px-8">
    <div
      class="w-full max-w-md space-y-8 rounded-lg border border-gray-300 bg-white p-10 shadow-lg dark:border-gray-300/20 dark:bg-slate-500/20"
    >
      <h1 class="mb-6 text-center text-2xl font-bold">{{ header }}</h1>

      <!-- Avatar Selection -->
      <div class="mb-6 flex flex-col items-center">
        <div
          class="border-primary mb-4 h-24 w-24 cursor-pointer overflow-hidden rounded-full border-2"
          @click="toggleShowAvatarOptions"
        >
          <img
            :src="selectedAvatar || defaultAvatar"
            alt="Profile picture"
            class="h-full w-full object-cover"
          />
        </div>
        <div
          v-if="!showAvatarOptions"
          class="cursor-pointer rounded border px-2 py-1"
          @click="toggleShowAvatarOptions"
        >
          Select Avatar
        </div>

        <h2 class="mb-2 text-sm font-medium" v-if="showAvatarOptions">Select Avatar</h2>
        <div class="grid grid-cols-3 gap-2" v-if="showAvatarOptions">
          <button
            v-for="(avatar, index) in avatarOptions"
            :key="index"
            class="h-16 w-16 overflow-hidden rounded-full border-2"
            :class="avatar === selectedAvatar ? 'border-primary' : 'border-transparent'"
            @click="selectedAvatar = avatar"
          >
            <img :src="avatar" alt="Avatar option" class="h-full w-full object-cover" />
          </button>
        </div>
      </div>

      <!-- Username Field -->
      <div class="mb-4">
        <label for="username" class="mb-1 block text-sm font-medium">Username</label>
        <StandardInput
          id="username"
          v-model="player.username"
          type="text"
          placeholder="Enter your username"
          :isLoading="isLoading || isLoadingOverride"
        />
        <p v-if="usernameError" class="mt-1 text-sm text-red-500">{{ usernameError }}</p>
      </div>

      <!-- Birthday Field -->
      <div class="mb-6 hidden">
        <label for="birthday" class="mb-1 block text-sm font-medium">Birthday</label>
        <StandardInput
          id="birthday"
          v-model="birthday"
          type="date"
          :isLoading="isLoading || isLoadingOverride"
        />
      </div>

      <!-- Submit Button -->
      <button
        @click="updateProfile"
        class="btn-default flex w-full items-center justify-center space-x-2"
        :class="{ 'cursor-wait': isLoading || isLoadingOverride }"
      >
        <Loader2Icon v-if="isLoading || isLoadingOverride" class="h-5 w-5 animate-spin" />
        <span>{{ buttonText }}</span>
      </button>
    </div>
  </div>
</template>
