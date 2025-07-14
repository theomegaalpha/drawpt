import flattenColorPalette from 'tailwindcss/lib/util/flattenColorPalette.js'

/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  theme: {
    extend: {
      keyframes: {
        'float-up-fade': {
          '0%': { opacity: '1', transform: 'translateY(0)' },
          '100%': { opacity: '0', transform: 'translateY(-1rem)' }
        },
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
        'slide-in-left': {
          '0%': {
            transform: 'translateX(-50%)',
            opacity: '0'
          },
          '100%': {
            transform: 'translateX(0)',
            opacity: '1'
          }
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
        'fade-blur-in-effect': {
          '0%': {
            opacity: '0',
            filter: 'blur(12px)'
          },
          '50%': {
            opacity: '0',
            filter: 'blur(12px)'
          },
          '100%': {
            opacity: '1',
            filter: 'blur(0px)'
          }
        },
        'fade-blur-in-fast-effect': {
          '0%': {
            opacity: '0',
            filter: 'blur(12px)'
          },
          '100%': {
            opacity: '1',
            filter: 'blur(0px)'
          }
        },
        bulging: {
          '0%': { transform: 'scale(1)' },
          '50%': { transform: 'scale(1.1)' },
          '100%': { transform: 'scale(1)' }
        }
      },
      animation: {
        aurora: 'aurora 60s linear infinite',
        'timer-countdown': 'timer-countdown linear',
        'slide-in-left': 'slide-in-left 0.3s ease-in forwards',
        'slide-in': 'slide-in 0.2s ease-in forwards',
        'blur-in': 'blur-in-effect 0.5s ease-out forwards',
        'fade-blur-in-slow': 'fade-blur-in-effect 2s ease-out forwards',
        'fade-blur-in-fast': 'fade-blur-in-fast-effect 175ms ease-out forwards',
        bulging: 'bulging 1.5s ease-in-out infinite',
        'float-up-fade': 'float-up-fade 3s ease-out forwards'
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
