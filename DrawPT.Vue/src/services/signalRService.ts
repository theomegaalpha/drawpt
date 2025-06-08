import * as signalR from '@microsoft/signalr'
import { getAccessToken } from '@/lib/auth'

// Define a callback type for connection status changes
type ConnectionStatusCallback = (isConnected: boolean) => void

export class SignalRService {
  private connection: signalR.HubConnection | null = null
  public isConnected: boolean = false
  private statusChangeCallbacks: ConnectionStatusCallback[] = []

  constructor() {
    this.getConnectionStatus = this.getConnectionStatus.bind(this)
    this.stopConnection = this.stopConnection.bind(this)
    this.invoke = this.invoke.bind(this)
    this.on = this.on.bind(this)
    this.off = this.off.bind(this)
    this.onConnectionStatusChanged = this.onConnectionStatusChanged.bind(this)
  }

  private notifyStatusChange() {
    this.statusChangeCallbacks.forEach((callback) => callback(this.isConnected))
  }

  public async startConnection(hubUrl: string): Promise<void> {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, { accessTokenFactory: async () => (await getAccessToken()) || '' })
      .withAutomaticReconnect()
      .build()

    // Optional: Handle reconnected and reconnected events to update isConnected
    this.connection.onreconnected(() => {
      console.log('SignalR Reconnected.')
      this.isConnected = true
      this.notifyStatusChange()
    })
    this.connection.onreconnecting((error) => {
      console.log(`SignalR Reconnecting due to: ${error}`)
      this.isConnected = false // Or a specific 'reconnecting' state
      this.notifyStatusChange()
    })
    this.connection.onclose((error) => {
      console.log(`SignalR Connection closed due to: ${error}`)
      this.isConnected = false
      this.notifyStatusChange()
    })

    try {
      await this.connection.start()
      console.log('SignalR Connected.')
      this.isConnected = true
      this.notifyStatusChange()
    } catch (err) {
      console.log('SignalR Connection Failed: ', err)
      this.isConnected = false
      this.notifyStatusChange()
    }
  }

  public async getConnectionStatus(): Promise<signalR.HubConnectionState> {
    if (this.connection) {
      return this.connection.state
    }
    return signalR.HubConnectionState.Disconnected
  }

  public async stopConnection(): Promise<void> {
    if (this.connection) {
      try {
        await this.connection.stop()
        console.log('SignalR Disconnected.')
        this.isConnected = false
        this.notifyStatusChange()
      } catch (err) {
        console.log('SignalR Disconnection Failed: ', err)
      }
    }
  }

  public on(method: string, newMethod: (...args: any[]) => void): void {
    if (this.connection) {
      this.connection.on(method, newMethod)
    }
  }

  public off(method: string): void {
    if (this.connection) {
      this.connection.off(method)
    }
  }

  public async invoke(method: string, ...args: any[]): Promise<any> {
    if (this.connection) {
      try {
        return await this.connection.invoke(method, ...args)
      } catch (err) {
        console.error(err)
      }
    }
  }

  // Method to subscribe to connection status changes
  public onConnectionStatusChanged(callback: ConnectionStatusCallback): () => void {
    this.statusChangeCallbacks.push(callback)
    // Return an unsubscribe function
    return () => {
      this.statusChangeCallbacks = this.statusChangeCallbacks.filter((cb) => cb !== callback)
    }
  }
}

export default new SignalRService()
