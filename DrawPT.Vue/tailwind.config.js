/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
  theme: {
    extend: {
      keyframes: {
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
        }
      },
      animation: {
        'timer-countdown': 'timer-countdown linear',
        'slide-in': 'slide-in 0.2s ease-in forwards'
      }
    }
  },
  plugins: []
}
