// Google Sign-In API types
declare global {
  interface Window {
    google?: {
      accounts: {
        id: {
          initialize: (config: GoogleSignInConfig) => void
          renderButton: (element: Element | null, options: GoogleButtonConfig) => void
          prompt: () => void
          cancel: () => void
        }
      }
    }
    handleSignInWithGoogle?: (response: GoogleCredentialResponse) => void
  }
}

export interface GoogleSignInConfig {
  client_id: string
  callback: (response: GoogleCredentialResponse) => void
  auto_select?: boolean
  cancel_on_tap_outside?: boolean
}

export interface GoogleButtonConfig {
  type?: 'standard' | 'icon'
  shape?: 'rectangular' | 'pill' | 'circle' | 'square'
  theme?: 'outline' | 'filled_blue' | 'filled_black'
  text?: 'signin_with' | 'signup_with' | 'continue_with' | 'signin'
  size?: 'large' | 'medium' | 'small'
  logo_alignment?: 'left' | 'center'
  width?: string
  locale?: string
}

export interface GoogleCredentialResponse {
  credential: string
  select_by?: string
}

export {}
