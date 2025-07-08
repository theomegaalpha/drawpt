// src/services/notificationEventHandlers.ts
import service from '@/services/signalRService'
import { useLeaderboardStore } from '@/stores/leaderboard'
import type { DailyAnswer } from '@/models/dailyModels'

// Register all NotificationHub events
export function registerNotificationHubEvents() {
  const leaderboard = useLeaderboardStore()

  // Handle new daily answers broadcast from the server
  service.on('newDailyAnswer', (answer: DailyAnswer) => {
    console.log('New daily answer received:', answer)

    leaderboard.dailies.unshift(answer)
    // Optionally trim to top 20
    if (leaderboard.dailies.length > 20) {
      leaderboard.dailies.splice(20)
    }
  })
}

// Unregister NotificationHub events
export function unregisterNotificationHubEvents() {
  service.off('NewDailyAnswer')
}
