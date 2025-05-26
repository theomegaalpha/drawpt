import { useAuthStore } from '@/stores/auth'

export const registerGuard = (router: any) => {
  router.beforeEach(async (to: any, from: any, next: any) => {
    const authStore = useAuthStore()
    const requiresAuth = to.matched.some((record: any) => record.meta.requiresAuth)
    const isAuthenticated = !!authStore.user

    if (requiresAuth && !isAuthenticated) {
      next('/login')
    } else {
      next()
    }
  })
}
