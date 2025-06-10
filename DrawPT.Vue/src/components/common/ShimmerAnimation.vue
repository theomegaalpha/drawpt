<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  class?: string // To accept external classes, Vue uses `class` not `className`
  showRadialGradient?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  showRadialGradient: true,
  class: '' // Default for the external class prop
})

const containerClasses = computed(() => {
  let classes = 'fixed inset-0 bg-zinc-50 dark:bg-black transition-bg' // Changed to fixed, full viewport, removed flex and z-index
  if (props.class) {
    classes += ` ${props.class}`
  }
  return classes
})

const gradientClasses = computed(() => {
  let baseClasses = `
    [--white-gradient:repeating-linear-gradient(100deg,var(--white)_0%,var(--white)_7%,var(--transparent)_10%,var(--transparent)_12%,var(--white)_16%)]
    [--dark-gradient:repeating-linear-gradient(100deg,var(--black)_0%,var(--black)_7%,var(--transparent)_10%,var(--transparent)_12%,var(--black)_16%)]
    [--aurora:repeating-linear-gradient(100deg,var(--blue-500)_10%,var(--indigo-300)_15%,var(--blue-300)_20%,var(--violet-200)_25%,var(--blue-400)_30%)]
    [background-image:var(--white-gradient),var(--aurora)]
    dark:[background-image:var(--dark-gradient),var(--aurora)]
    [background-size:300%,_200%]
    [background-position:50%_50%,50%_50%]
    filter blur-[10px] invert dark:invert-0
    after:content-[""] after:absolute after:inset-0 after:[background-image:var(--white-gradient),var(--aurora)]
    after:dark:[background-image:var(--dark-gradient),var,--aurora)]
    after:[background-size:200%,_100%]
    after:animate-aurora after:[background-attachment:fixed] after:mix-blend-difference
    pointer-events-none
    absolute -inset-[10px] opacity-40 dark:opacity-10 will-change-transform` // Ensure no z-index here, parent div handles layering
  if (props.showRadialGradient) {
    baseClasses += ` [mask-image:radial-gradient(ellipse_at_100%_0%,black_10%,var(--transparent)_70%)]`
  }
  return baseClasses.trim()
})
</script>

<template>
  <div :class="containerClasses" v-bind="$attrs">
    <div class="absolute inset-0 -z-10 overflow-hidden">
      <!-- Shimmer effect container, behind slot -->
      <div :class="gradientClasses"></div>
    </div>
    <div class="relative z-0 h-full w-full overflow-auto">
      <slot></slot>
    </div>
    <!-- Content will be rendered here, on top of the shimmer -->
  </div>
</template>

<style scoped>
/* Component-specific styles can go here if needed */
</style>
