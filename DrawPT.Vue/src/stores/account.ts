import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { User } from '@supabase/supabase-js'

export const useAccountStore = defineStore('account', () => {
  const user = ref<User | null>(null)

  const setUser = (newUser: User | null) => {
    user.value = newUser
  }

  return {
    user,
    setUser
  }
})
