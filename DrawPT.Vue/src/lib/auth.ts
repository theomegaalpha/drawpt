import { supabase } from '@/lib/supabase'

export const getAccessToken = async () => {
  const { data, error } = await supabase.auth.getSession()
  if (error) {
    throw error
  }
  return data.session?.access_token
}

// check if user is authenticated
export const isAuthenticated = async () => {
  const { data, error } = await supabase.auth.getSession()
  if (error) {
    console.error('Error checking authentication:', error)
    return false
  }
  return !!data.session
}
