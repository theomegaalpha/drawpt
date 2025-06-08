<script setup lang="ts">
import ViewThemes from '../room/game/midround/ViewThemes.vue'
</script>

<template>
  <div class="flex min-h-screen items-center justify-center">
    <ViewThemes :themes="slugs" />
  </div>
</template>

<script lang="ts">
export default {
  name: 'ImageLoaderTest'
}
const slugs = ['typescript', 'javascript', 'dart', 'java', 'react']
</script>
