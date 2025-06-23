import { defineStore } from 'pinia'
import { ref } from 'vue'
import { supabase } from '@/lib/supabase'
import type { User } from '@supabase/supabase-js'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const isAuthenticated = ref(false)
  const role = ref<string>('')
  const loading = ref(true)

  // Initialize auth state
  supabase.auth.getSession().then(({ data: { session } }) => {
    user.value = session?.user ?? null
    loading.value = false
  })

  supabase.auth.getClaims().then(({ data }) => {
    role.value = data?.claims?.user_role || ''
  })

  // Listen for auth changes
  supabase.auth.onAuthStateChange((_event, session) => {
    user.value = session?.user ?? null
    isAuthenticated.value = !!user.value
  })

  const signOut = async () => {
    try {
      await supabase.auth.signOut()
      user.value = null
    } catch (error) {
      console.error('Error signing out:', error)
    }
  }

  return {
    user,
    role,
    loading,
    isAuthenticated,
    signOut
  }
})
