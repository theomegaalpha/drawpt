import { ref } from 'vue'

export function useSpeechRecognition() {
  const transcribedText = ref('')
  const isListening = ref(false)

  // Check for browser compatibility
  const SpeechRecognition =
    (window as any).SpeechRecognition || (window as any).webkitSpeechRecognition
  if (!SpeechRecognition) {
    console.error('Speech recognition not supported in this browser.')
    return {
      transcribedText,
      isListening,
      toggleListening: () => console.error('Speech recognition not supported')
    }
  }

  const recognitionInstance = new SpeechRecognition()
  recognitionInstance.continuous = true
  recognitionInstance.interimResults = true
  recognitionInstance.lang = 'en-US'

  recognitionInstance.onresult = (event: SpeechRecognitionEvent) => {
    const t = Array.from(event.results)
      .map((result) => result[0])
      .map((result) => result.transcript)
      .join('')

    transcribedText.value = t.trim()
  }

  recognitionInstance.onerror = (event: SpeechRecognitionErrorEvent) => {
    console.error('Speech recognition error:', event.error)
    isListening.value = false
  }

  recognitionInstance.onend = () => {
    isListening.value = false
  }

  const toggleListening = () => {
    if (isListening.value) {
      recognitionInstance.stop()
      isListening.value = false
    } else {
      try {
        transcribedText.value = ''
        recognitionInstance.start()
        isListening.value = true
      } catch (error) {
        console.error('Error starting speech recognition:', error)
        isListening.value = false
      }
    }
  }

  try {
    recognitionInstance.start()
    isListening.value = true
  } catch (error) {
    console.error('Error starting initial speech recognition:', error)
    isListening.value = false
  }
  return {
    transcribedText,
    isListening,
    toggleListening
  }
}
