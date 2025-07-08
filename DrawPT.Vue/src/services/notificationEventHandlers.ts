// src/services/notificationEventHandlers.ts
import service from '@/services/signalRService'
import { useLeaderboardStore } from '@/stores/leaderboard'
import type { DailyAnswer } from '@/models/dailyModels'

// Register all NotificationHub events
export function registerNotificationHubEvents() {
  const leaderboard = useLeaderboardStore()

  // Handle new daily answers broadcast from the server
  service.on('newDailyAnswer', (answer: DailyAnswer) => {
    const list = leaderboard.dailies
    // Determine if answer qualifies for top 20
    const lowestScore = list.length < 20 ? -Infinity : list[list.length - 1].score
    if (list.length < 20 || answer.score > lowestScore) {
      leaderboard.addDailyAnswer(answer)
    }
  })
}

// Unregister NotificationHub events
export function unregisterNotificationHubEvents() {
  service.off('NewDailyAnswer')
}
