import flattenColorPalette from 'tailwindcss/lib/util/flattenColorPalette.js'

/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  theme: {
    extend: {
      keyframes: {
        aurora: {
          from: {
            backgroundPosition: '50% 50%, 50% 50%'
          },
          to: {
            backgroundPosition: '350% 50%, 350% 50%'
          }
        },
        'timer-countdown': {
          '100%': { width: '100%' },
          '40%': { width: '40%', 'background-color': '#FF5F15' },
          '20%': { width: '20%', 'background-color': '#FF0F0F' },
          '0%': { width: '0', 'background-color': '#FF0F0F' }
        },
        'slide-in': {
          '0%': {
            transform: 'translateX(20%)',
            opacity: '0'
          },
          '100%': {
            transform: 'translateX(0)',
            opacity: '1'
          }
        },
        'blur-in-effect': {
          '0%': {
            opacity: '0.2',
            filter: 'blur(12px)',
            transform: 'translateY(12px)'
          },
          '100%': {
            opacity: '1',
            filter: 'blur(0px)',
            transform: 'translateY(0px)'
          }
        },
        'star-movement-bottom': {
          '0%': { transform: 'translate(0%, 0%)', opacity: '1' },
          '100%': { transform: 'translate(-100%, 0%)', opacity: '0' }
        },
        'star-movement-top': {
          '0%': { transform: 'translate(0%, 0%)', opacity: '1' },
          '100%': { transform: 'translate(100%, 0%)', opacity: '0' }
        }
      },
      animation: {
        aurora: 'aurora 60s linear infinite',
        'timer-countdown': 'timer-countdown linear',
        'slide-in': 'slide-in 0.2s ease-in forwards',
        'blur-in': 'blur-in-effect 0.5s ease-out forwards',
        'star-movement-bottom': 'star-movement-bottom linear infinite alternate',
        'star-movement-top': 'star-movement-top linear infinite alternate'
      }
    }
  },
  plugins: [addVariablesForColors]
}

function addVariablesForColors({ addBase, theme }) {
  const palette = theme('colors')
  const flatPalette =
    typeof flattenColorPalette === 'function'
      ? flattenColorPalette(palette)
      : flattenColorPalette.default(palette) // Try .default if flattenColorPalette itself is not a function

  let newVars = Object.fromEntries(
    Object.entries(flatPalette).map(([key, val]) => [`--${key}`, val])
  )

  addBase({
    ':root': newVars
  })
}
