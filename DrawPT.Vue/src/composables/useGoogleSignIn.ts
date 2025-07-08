import { onMounted, onUnmounted } from 'vue'
import type { GoogleCredentialResponse } from '@/types/google'

interface UseGoogleSignInOptions {
  /** CSS selector for the container to render the button */
  containerSelector?: string
  /** Automatically select Google account prompt */
  autoSelect?: boolean
  /** Cancel prompt on tap outside */
  cancelOnTapOutside?: boolean
}

/**
 * Composable to initialize Google Sign-In.
 * @param callback The function to call when Google sign-in is successful.
 * @param options Optional settings like container selector.
 */
export function useGoogleSignIn(
  callback: (response: GoogleCredentialResponse) => void,
  options: UseGoogleSignInOptions = {}
) {
  const containerSelector = options.containerSelector || '.g_id_signin'

  const loadScript = (): Promise<boolean> => {
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

  const initialize = () => {
    if (window.google?.accounts) {
      // Expose callback globally
      window.handleSignInWithGoogle = callback

      window.google.accounts.id.initialize({
        client_id: import.meta.env.VITE_GOOGLE_CLIENT_ID,
        callback,
        auto_select: options.autoSelect ?? false,
        cancel_on_tap_outside: options.cancelOnTapOutside ?? false
      })

      const container = document.querySelector(containerSelector)
      if (container) {
        window.google.accounts.id.renderButton(container, {
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
      await loadScript()
      // slight delay for script to be ready
      setTimeout(initialize, 100)
    } catch (err) {
      console.error('Failed to load Google Sign-In:', err)
    }
  })

  onUnmounted(() => {
    if (window.handleSignInWithGoogle) {
      delete window.handleSignInWithGoogle
    }
  })
}
