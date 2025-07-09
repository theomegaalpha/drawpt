<template>
  <button
    type="button"
    @click="handleLogin"
    :disabled="loading"
    class="group relative flex w-full justify-center rounded-full border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
  >
    <img :src="googleLogo" alt="Google logo" class="mr-2 h-5 w-5" />
    <span v-if="loading">Loading...</span>
    <span v-else>Continue with Google</span>
  </button>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { supabase } from '@/lib/supabase'
import googleLogo from '@/assets/icons/google.svg'

const loading = ref(false)
const error = ref('')
const redirectTo = import.meta.env.VITE_SUPABASE_REDIRECT_URL || window.location.origin

const handleLogin = async () => {
  try {
    loading.value = true
    error.value = ''
    const { data, error: oauthError } = await supabase.auth.signInWithOAuth({
      provider: 'google',
      options: { redirectTo }
    })
    if (oauthError) throw oauthError
    if (data?.url) window.location.href = data.url
  } catch (e: any) {
    error.value = e.message || 'An error occurred during Google sign in'
  } finally {
    loading.value = false
  }
}
</script>
