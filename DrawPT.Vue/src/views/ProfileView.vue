<script setup lang="ts">
import EditProfile from '@/components/common/EditProfile.vue'
import router from '@/router'

// Navigate to home on save
const handleSaved = () => {
  router.push('/')
}
</script>

<template>
  <EditProfile @saved="handleSaved" />
</template>
