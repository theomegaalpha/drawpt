// src/services/AudioPlayerService.ts
export class AudioPlayerService {
  private audioContext: AudioContext | null = null
  private node: AudioWorkletNode | null = null
  private gainNode: GainNode | null = null
  // buffer list for incoming Opus chunks
  private chunks: Uint8Array[] = []

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
    this.gainNode = this.audioContext.createGain()
    this.node.connect(this.gainNode)
    this.gainNode.connect(this.audioContext.destination)
  }

  /**
   * Append a base64-encoded Opus chunk to the buffer list
   */
  appendOpusChunk(base64: string) {
    const binary = atob(base64)
    const u8 = Uint8Array.from(binary, (c) => c.charCodeAt(0))
    this.chunks.push(u8)
  }

  /**
   * Decode accumulated Opus data and play via Web Audio API
   */
  async playOpusStream() {
    console.debug('[AudioPlayer] playOpusStream called, chunks count:', this.chunks.length)
    if (!this.audioContext) return
    // concatenate all chunks
    const total = this.chunks.reduce((sum, c) => sum + c.length, 0)
    const merged = new Uint8Array(total)
    let offset = 0
    for (const c of this.chunks) {
      merged.set(c, offset)
      offset += c.length
    }
    // clear buffer list
    this.chunks = []
    console.debug('[AudioPlayer] merged buffer length:', merged.byteLength)
    // decode and play with error logging
    try {
      const audioBuffer = await this.audioContext.decodeAudioData(merged.buffer)
      console.debug('[AudioPlayer] decodeAudioData success, duration:', audioBuffer.duration)
      const src = this.audioContext.createBufferSource()
      src.buffer = audioBuffer
      // route through gainNode if available
      if (this.gainNode) {
        src.connect(this.gainNode)
      } else {
        src.connect(this.audioContext.destination)
      }
      src.start()
      console.debug('[AudioPlayer] playback started')
    } catch (err) {
      console.error('decodeAudioData failed:', err)
    }
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

  /** Set the playback volume (0.0 to 1.0) */
  setVolume(volume: number) {
    if (this.gainNode) {
      this.gainNode.gain.value = volume
    }
  }
}
