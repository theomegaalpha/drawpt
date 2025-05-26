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
    let finalTranscript = ''

    for (let i = event.resultIndex; i < event.results.length; ++i) {
      const transcript = event.results[i][0].transcript
      if (event.results[i].isFinal) {
        if (event.results[i][0].confidence > 0.2) {
          finalTranscript += transcript
        }
      }
    }

    if (finalTranscript) {
      transcribedText.value = finalTranscript
    }
  }

  recognitionInstance.onerror = (event: SpeechRecognitionErrorEvent) => {
    console.error('Speech recognition error:', event.error)
    isListening.value = false
  }

  recognitionInstance.onend = () => {
    if (isListening.value) {
      // Potentially restart, or handle as per your app's logic
    } else {
      recognitionInstance.stop()
    }
    // isListening.value = false; // Ensure isListening is false if recognition truly stops
    // interimText.value = ''; // Clear interim when listening ends, handled by toggleListening
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

  return {
    transcribedText,
    isListening,
    toggleListening
  }
}
