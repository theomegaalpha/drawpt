import { supabase } from '@/lib/supabase'

/**
 * Composable to initiate Facebook OAuth login via Supabase.
 * @returns loginWithFacebook function to trigger login.
 */
export function useFacebookLogin() {
  /**
   * Starts the OAuth flow for Facebook.
   */
  const loginWithFacebook = async () => {
    const { data, error } = await supabase.auth.signInWithOAuth({ provider: 'facebook' })
    if (error) {
      throw error
    }
    // Redirect to Facebook OAuth consent
    if (data.url) {
      window.location.href = data.url
    }
  }

  return { loginWithFacebook }
}
