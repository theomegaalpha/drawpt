import * as signalR from '@microsoft/signalr'
import { getAccessToken } from '@/lib/auth'

export class SignalRService {
  private connection: signalR.HubConnection | null = null
  public isConnected: boolean = false

  constructor() {
    this.getConnectionStatus = this.getConnectionStatus.bind(this)
    this.stopConnection = this.stopConnection.bind(this)
    this.invoke = this.invoke.bind(this)
    this.on = this.on.bind(this)
    this.off = this.off.bind(this)
  }

  public async startConnection(hubUrl: string): Promise<void> {
    const accessToken = (await getAccessToken()) || ''
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, { accessTokenFactory: () => accessToken })
      .withAutomaticReconnect()
      .build()

    try {
      await this.connection.start()
      console.log('SignalR Connected.')
      this.isConnected = true
    } catch (err) {
      console.log('SignalR Connection Failed: ', err)
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
}

export default new SignalRService()
