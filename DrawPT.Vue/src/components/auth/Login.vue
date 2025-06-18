<template>
  <div class="flex min-h-screen items-center justify-center px-4 py-12 sm:px-6 lg:px-8">
    <div
      class="w-full max-w-md space-y-8 rounded-lg border border-gray-300 bg-white p-10 shadow-lg dark:border-gray-300/20 dark:bg-slate-500/20"
    >
      <div>
        <h2 class="text-default mt-6 text-center text-3xl font-extrabold">
          Sign in to your account
        </h2>
      </div>
      <form class="mt-8 space-y-6" @submit.prevent="handleLogin">
        <div class="-space-y-px rounded-md shadow-sm">
          <div>
            <label for="email" class="sr-only">Email address</label>
            <input
              id="email"
              v-model="email"
              name="email"
              type="email"
              required
              class="relative block w-full appearance-none rounded-none rounded-t-md border border-gray-300 px-3 py-2 text-gray-900 placeholder-gray-500 focus:z-10 focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 sm:text-sm"
              placeholder="Email address"
            />
          </div>
          <div>
            <label for="password" class="sr-only">Password</label>
            <input
              id="password"
              v-model="password"
              name="password"
              type="password"
              required
              class="relative block w-full appearance-none rounded-none rounded-b-md border border-gray-300 px-3 py-2 text-gray-900 placeholder-gray-500 focus:z-10 focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 sm:text-sm"
              placeholder="Password"
            />
          </div>
        </div>

        <div>
          <button
            type="submit"
            :disabled="loading"
            class="group relative flex w-full justify-center rounded-md border border-transparent bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
          >
            <span v-if="loading">Loading...</span>
            <span v-else>Sign in</span>
          </button>
        </div>
        <div v-if="error" class="text-center text-sm text-red-500">
          {{ error }}
        </div>

        <!-- Resend Confirmation Email Button -->
        <div v-if="showResendButton">
          <div class="text-center text-sm">
            <button
              type="button"
              @click="handleResendConfirmation"
              :disabled="loading"
              class="font-medium text-indigo-600 hover:text-indigo-500"
            >
              Resend confirmation email
            </button>
          </div>
          <div v-if="resendMessage" class="text-center text-sm text-green-500">
            {{ resendMessage }}
          </div>
        </div>

        <!-- Google Sign-In Button Container -->
        <div class="text-center">
          <div class="g_id_signin"></div>
        </div>

        <div class="text-center text-sm">
          <router-link to="/register" class="font-medium text-indigo-600 hover:text-indigo-500">
            Don't have an account? Sign up
          </router-link>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { supabase } from '@/lib/supabase'
import type { GoogleCredentialResponse } from '@/types/google'
import { usePlayerStore } from '@/stores/player'
import { storeToRefs } from 'pinia'

const router = useRouter()
const email = ref('')
const password = ref('')
const loading = ref(false)
const error = ref('')
const resendMessage = ref('')
const showResendButton = ref(false)

const playerStore = usePlayerStore()
const { player } = storeToRefs(playerStore)

const handleLogin = async () => {
  try {
    loading.value = true
    error.value = ''
    showResendButton.value = false // Reset on new login attempt

    const { data, error: signInError } = await supabase.auth.signInWithPassword({
      email: email.value,
      password: password.value
    })

    if (signInError) {
      if (signInError.message === 'Email not confirmed') {
        showResendButton.value = true
      }
      throw signInError
    }

    if (data.user) {
      await playerStore.init()
      if (!player.value.avatar) {
        router.push('/profile')
      } else {
        router.push('/')
      }
    }
  } catch (e: any) {
    error.value = e.message || 'An error occurred during sign in'
  } finally {
    loading.value = false
  }
}

const handleResendConfirmation = async () => {
  if (!email.value) {
    error.value = 'Please enter your email address to resend the confirmation.'
    return
  }
  try {
    loading.value = true
    error.value = ''
    resendMessage.value = ''

    const { error: resendError } = await supabase.auth.resend({
      type: 'signup',
      email: email.value
    })

    if (resendError) throw resendError

    resendMessage.value = 'Confirmation email resent. Please check your inbox.'
  } catch (e: any) {
    error.value = e.message || 'An error occurred while resending the confirmation email.'
  } finally {
    loading.value = false
  }
}

const handleSignInWithGoogle = async (response: GoogleCredentialResponse) => {
  try {
    loading.value = true
    error.value = ''

    const { data, error: signInError } = await supabase.auth.signInWithIdToken({
      provider: 'google',
      token: response.credential
    })

    if (signInError) throw signInError

    if (data.user) {
      await playerStore.init()
      if (!player.value.avatar) {
        router.push('/profile')
      } else {
        router.push('/')
      }
    }
  } catch (e: any) {
    error.value = e.message || 'An error occurred during Google sign in'
  } finally {
    loading.value = false
  }
}

const loadGoogleScript = (): Promise<boolean> => {
  return new Promise((resolve, reject) => {
    if (document.getElementById('google-signin-script')) {
      resolve(true)
      return
    }

    const script = document.createElement('script')
    script.id = 'google-signin-script'
    script.src = 'https://accounts.google.com/gsi/client'
    script.async = true
    script.onload = () => resolve(true)
    script.onerror = () => reject(new Error('Failed to load Google Sign-In script'))
    document.head.appendChild(script)
  })
}

const initializeGoogleSignIn = () => {
  if (window.google?.accounts) {
    // Make the callback function globally available
    window.handleSignInWithGoogle = handleSignInWithGoogle

    window.google.accounts.id.initialize({
      client_id: import.meta.env.VITE_GOOGLE_CLIENT_ID,
      callback: handleSignInWithGoogle,
      auto_select: true,
      cancel_on_tap_outside: false
    })

    const buttonContainer = document.querySelector('.g_id_signin')
    if (buttonContainer) {
      window.google.accounts.id.renderButton(buttonContainer, {
        type: 'standard',
        shape: 'pill',
        theme: 'outline',
        text: 'signin_with',
        size: 'large',
        logo_alignment: 'left'
      })
    }

    // Optional: Display the One Tap prompt
    window.google.accounts.id.prompt()
  }
}

onMounted(async () => {
  try {
    await loadGoogleScript()
    // Wait a bit for the script to fully initialize
    setTimeout(initializeGoogleSignIn, 100)
  } catch (error) {
    console.error('Failed to load Google Sign-In:', error)
  }
})

onUnmounted(() => {
  // Clean up the global callback
  if (window.handleSignInWithGoogle) {
    delete window.handleSignInWithGoogle
  }
})
</script>
