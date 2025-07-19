import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { watch } from 'vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('@/components/auth/Login.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/components/auth/Register.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/',
      name: 'home',
      component: () => import('@/views/HomeView.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('@/views/ProfileView.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/game-selection',
      name: 'game-selection',
      component: () => import('@/views/GameSelectionView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/room/:roomCode',
      name: 'room',
      component: () => import('@/views/RoomView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/game-not-found',
      name: 'game-not-found',
      component: () => import('@/views/GameNotFoundView.vue')
    },
    {
      path: '/about',
      name: 'about',
      component: () => import('../views/AboutView.vue')
    },
    {
      path: '/faq',
      name: 'faq',
      component: () => import('@/views/FAQView.vue')
    },
    {
      path: '/contact',
      name: 'contact',
      component: () => import('@/views/ContactView.vue')
    },
    {
      path: '/admin',
      meta: { requiresAuth: true, requiresRole: 'admin' },
      children: [
        {
          path: '',
          name: 'admin-home',
          meta: { requiresAuth: true, requiresRole: 'admin' },
          component: () => import('@/views/admin/IndexView.vue')
        },
        {
          path: 'dailies',
          name: 'admin-dailies',
          meta: { requiresAuth: true, requiresRole: 'admin' },
          component: () => import('@/views/admin/DailiesView.vue')
        }
      ]
    },
    {
      path: '/test',
      name: 'test',
      component: () => import('@/views/TestView.vue')
    },
    {
      path: '/:catchAll(.*)',
      name: 'not-found',
      component: () => import('@/views/GameNotFoundView.vue')
    }
  ]
})

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()

  // Wait for auth state to be initialized
  if (authStore.loading) {
    await new Promise((resolve) => {
      const unwatch = watch(
        () => authStore.loading,
        (loading: boolean) => {
          if (!loading) {
            unwatch()
            resolve(true)
          }
        }
      )
    })
  }

  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth)
  const requiredRole = to.matched.find((record: any) => record.meta.requiresRole)?.meta
    .requiresRole as string | undefined
  const isAuthenticated = !!authStore.user

  if ((requiresAuth && !isAuthenticated) || (requiredRole && requiredRole !== authStore.role)) {
    next({ name: 'login' })
  } else if (isAuthenticated && (to.name === 'login' || to.name === 'register')) {
    next({ name: 'home' })
  } else {
    next()
  }
})

export default router
