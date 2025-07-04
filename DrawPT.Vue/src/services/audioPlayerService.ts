// src/services/AudioPlayerService.ts
export class AudioPlayerService {
  private audioContext: AudioContext | null = null
  private node: AudioWorkletNode | null = null

  async init(sampleRate = 24000) {
    this.audioContext = new AudioContext({ sampleRate })
    // Ensure context is running (user gesture may be required)
    if (this.audioContext.state === 'suspended') {
      await this.audioContext.resume()
    }
    // Load worklet from public root
    const moduleUrl = `${location.origin}/audio-playback-worklet.js`
    await this.audioContext.audioWorklet.addModule(moduleUrl)
    this.node = new AudioWorkletNode(this.audioContext, 'audio-playback-worklet')
    this.node.connect(this.audioContext.destination)
  }

  playBuffer(pcm: Int16Array) {
    // Resume if suspended
    if (this.audioContext?.state === 'suspended') {
      this.audioContext.resume()
    }
    this.node?.port.postMessage(pcm)
  }

  stop() {
    this.node?.port.postMessage(null)
  }

  async destroy() {
    this.stop()
    await this.audioContext?.close()
    this.audioContext = this.node = null
  }
}
