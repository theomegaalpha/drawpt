import { supabase } from '@/lib/supabase'

export const getAccessToken = async () => {
  const { data, error } = await supabase.auth.getSession()
  if (error) {
    throw error
  }
  return data.session?.access_token
}
