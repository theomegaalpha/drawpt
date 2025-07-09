<template>
  <div class="flex min-h-screen items-center justify-center px-4 py-12 sm:px-6 lg:px-8">
    <div
      class="w-full max-w-md space-y-8 rounded-lg border border-gray-300 bg-white p-10 shadow-lg dark:border-gray-300/20 dark:bg-slate-500/20"
    >
      <div>
        <h2 class="text-default mt-6 text-center text-3xl font-extrabold">Create your account</h2>
      </div>
      <form class="mt-8 space-y-6" @submit.prevent="handleRegister">
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
              class="relative block w-full appearance-none rounded-none border border-gray-300 px-3 py-2 text-gray-900 placeholder-gray-500 focus:z-10 focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 sm:text-sm"
              placeholder="Password"
            />
          </div>
          <div>
            <label for="confirmPassword" class="sr-only">Confirm Password</label>
            <input
              id="confirmPassword"
              v-model="confirmPassword"
              name="confirmPassword"
              type="password"
              required
              class="relative block w-full appearance-none rounded-none rounded-b-md border border-gray-300 px-3 py-2 text-gray-900 placeholder-gray-500 focus:z-10 focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 sm:text-sm"
              placeholder="Confirm Password"
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
            <span v-else>Sign up with email</span>
          </button>
          <div class="my-4 text-center">
            <GoogleLoginButton />
          </div>
        </div>

        <div v-if="error" class="text-center text-sm text-red-500">
          {{ error }}
        </div>

        <div class="text-center text-sm">
          <router-link to="/login" class="font-medium text-indigo-600 hover:text-indigo-500">
            Already have an account? Sign in
          </router-link>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { supabase } from '@/lib/supabase'
import GoogleLoginButton from '@/components/common/GoogleLoginButton.vue'

// Define a multi-word component name
defineOptions({ name: 'AuthRegister' })

const router = useRouter()
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const error = ref('')

const handleRegister = async () => {
  try {
    if (password.value !== confirmPassword.value) {
      error.value = 'Passwords do not match'
      return
    }

    loading.value = true
    error.value = ''

    const { data, error: signUpError } = await supabase.auth.signUp({
      email: email.value,
      password: password.value
    })

    if (signUpError) throw signUpError

    if (data.user) {
      router.push('/login')
    }
  } catch (e: any) {
    error.value = e.message || 'An error occurred during sign up'
  } finally {
    loading.value = false
  }
}
</script>
