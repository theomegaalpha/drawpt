<script setup lang="ts">
import EditProfile from '@/components/common/EditProfile.vue'
import router from '@/router'

// Navigate to home on save
const handleSaved = () => {
  router.push('/')
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center px-4 py-12 sm:px-6 lg:px-8">
    <EditProfile @saved="handleSaved" />
  </div>
</template>
