<script setup lang="ts">
import { computed, ref, onMounted, onBeforeUnmount } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { MenuIcon, XIcon } from 'lucide-vue-next' // Added MenuIcon and XIcon
import { storeToRefs } from 'pinia'

import { usePlayerStore } from '@/stores/player'
import {
  registerNotificationHubEvents,
  unregisterNotificationHubEvents
} from '@/services/notificationEventHandlers'
import service from '@/services/signalRService'

const playerStore = usePlayerStore()
const { player, blankAvatar } = storeToRefs(playerStore)

const router = useRouter()
const authStore = useAuthStore()
const isAuthenticated = computed(() => {
  return !!authStore.user
})

const _menuItems = [
  { name: 'About', href: '/about', role: '' },
  { name: 'Play Game', href: '/game-selection', role: '' },
  { name: 'Admin', href: '/admin', role: 'admin' }
]
const menuItems = computed(() => {
  return _menuItems.filter((item) => !item.role || item.role === authStore.role)
})
const isMobileMenuOpen = ref(false) // State for mobile menu

const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}

const isProfileMenuOpen = ref(false)
const profileMenuRef = ref<HTMLElement | null>(null)

const toggleProfileMenu = () => {
  isProfileMenuOpen.value = !isProfileMenuOpen.value
}

const handleLogout = async () => {
  await authStore.signOut()
  router.push('/')
}

const handleClickOutside = (event: MouseEvent) => {
  if (profileMenuRef.value && !profileMenuRef.value.contains(event.target as Node)) {
    isProfileMenuOpen.value = false
  }
}

// Register SignalR notifications
onMounted(async () => {
  try {
    if (!service.isConnected) {
      await service.startConnection('/notificationhub')
    }
    registerNotificationHubEvents()
  } catch (err) {
    console.error('NotificationHub connection failed:', err)
  }
  document.addEventListener('click', handleClickOutside)
})
onBeforeUnmount(() => {
  unregisterNotificationHubEvents()
  document.removeEventListener('click', handleClickOutside)
  if (service.isConnected) {
    service.stopConnection()
  }
})
</script>

<template>
  <header>
    <nav
      class="group fixed z-20 w-full border border-b border-slate-500/10 bg-white/30 backdrop-blur md:relative dark:border-white/10 dark:bg-zinc-950/50 lg:dark:bg-transparent"
    >
      <div class="m-auto max-w-7xl px-6">
        <div class="flex flex-wrap items-center justify-between gap-6 py-3 lg:gap-0 lg:py-4">
          <div class="flex w-full items-center justify-between lg:w-auto">
            <router-link to="/" aria-label="home" class="flex items-center space-x-2">
              <img src="@/assets/logo.png" alt="logo" class="mr-2 h-12 w-12" />
              <h1 class="text-2xl font-bold text-indigo-600 dark:text-indigo-300">DrawPT</h1>
            </router-link>
            <div class="lg:hidden">
              <button
                @click="toggleMobileMenu"
                aria-label="Toggle menu"
                class="rounded-md p-2 text-gray-700 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-indigo-500 dark:text-gray-300 dark:hover:bg-zinc-800"
              >
                <MenuIcon v-if="!isMobileMenuOpen" class="h-6 w-6" />
                <XIcon v-else class="h-6 w-6" />
              </button>
            </div>
          </div>

          <div
            :class="[
              'w-full lg:flex lg:w-auto lg:items-center lg:pr-4',
              isMobileMenuOpen ? 'block' : 'hidden'
            ]"
          >
            <ul
              class="flex flex-col space-y-2 py-4 text-base lg:flex-row lg:items-center lg:gap-2 lg:space-y-0 lg:py-0 lg:text-sm"
            >
              <li
                v-for="(item, index) in menuItems"
                :key="index"
                class="rounded-md p-2 hover:bg-black/5 dark:hover:bg-white/5"
              >
                <router-link
                  v-if="!item.role || item.role === authStore.role"
                  :to="item.href"
                  class="text-muted-foreground hover:text-accent-foreground block duration-150"
                >
                  <span>{{ item.name }}</span>
                </router-link>
              </li>
              <li v-if="isAuthenticated">
                <router-link
                  to="/profile"
                  class="text-muted-foreground hover:text-accent-foreground block rounded-md p-2 duration-150 hover:bg-black/5 lg:hidden dark:hover:bg-white/5"
                >
                  <span>Edit Profile</span>
                </router-link>
              </li>
              <li v-if="isAuthenticated">
                <div
                  ref="profileMenuRef"
                  class="text-muted-foreground hover:text-accent-foreground relative hidden cursor-pointer rounded-md p-2 duration-150 hover:bg-black/5 lg:block dark:hover:bg-white/5"
                >
                  <img
                    :src="player.avatar || blankAvatar"
                    alt="avatar"
                    class="hidden h-8 w-8 rounded-full lg:block"
                    @click="toggleProfileMenu"
                  />
                  <div
                    v-if="isProfileMenuOpen"
                    class="absolute right-0 z-50 mt-2 flex w-48 flex-col items-end rounded-md bg-white shadow-lg ring-1 ring-black ring-opacity-5 dark:bg-black"
                  >
                    <router-link
                      to="/profile"
                      class="text-default bg-surface-default w-full rounded-t-md px-4 py-2 text-right text-sm hover:bg-black/5 dark:hover:bg-white/5"
                    >
                      Edit Profile
                    </router-link>
                    <button
                      @click="handleLogout"
                      class="text-default bg-surface-default w-full rounded-b-md px-4 py-2 text-right text-sm hover:bg-black/5 dark:hover:bg-white/5"
                    >
                      Log out
                    </button>
                  </div>
                </div>
                <div
                  @click="handleLogout"
                  class="text-muted-foreground hover:text-accent-foreground block cursor-pointer rounded-md p-2 duration-150 hover:bg-black/5 lg:hidden dark:hover:bg-white/5"
                >
                  <span>Log out</span>
                </div>
              </li>
              <li v-if="!isAuthenticated">
                <router-link
                  to="/login"
                  class="text-muted-foreground hover:text-accent-foreground block rounded-md p-2 duration-150 hover:bg-black/5 dark:hover:bg-white/5"
                >
                  <span>Login</span>
                </router-link>
              </li>
              <li v-if="!isAuthenticated">
                <router-link
                  to="/register"
                  class="text-muted-foreground hover:text-accent-foreground block rounded-md p-2 duration-150 hover:bg-black/5 dark:hover:bg-white/5"
                >
                  <span>Sign up for free!</span>
                </router-link>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </nav>
  </header>
</template>
