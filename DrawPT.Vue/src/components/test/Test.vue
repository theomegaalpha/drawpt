<script setup lang="ts">
import SelectTheme from '../room/game/themescreen/SelectTheme.vue'
</script>

<template>
  <div class="flex min-h-screen items-center justify-center">
    <SelectTheme :themes="slugs" />
  </div>
</template>

<script lang="ts">
export default {
  name: 'ImageLoaderTest'
}
const slugs = ['typescript', 'javascript', 'dart', 'java', 'react']
</script>
