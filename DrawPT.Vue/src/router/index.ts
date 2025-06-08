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
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue')
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
  const isAuthenticated = !!authStore.user

  if (requiresAuth && !isAuthenticated) {
    next({ name: 'login' })
  } else if (isAuthenticated && (to.name === 'login' || to.name === 'register')) {
    next({ name: 'home' })
  } else {
    next()
  }
})

export default router
