<script setup lang="ts">
import { defineProps, defineEmits } from 'vue'

const {
  header,
  backgroundImage,
  disabled = false
} = defineProps({
  header: { type: String, required: true },
  backgroundImage: { type: String, required: true },
  disabled: { type: Boolean, default: false }
})

const emit = defineEmits(['click'])

const onCardClick = () => {
  if (!disabled) {
    emit('click')
  }
}
</script>

<template>
  <div
    class="group relative col-span-1 aspect-[2/3] transform overflow-hidden rounded-xl shadow-md transition duration-300 ease-in-out"
    :class="[
      disabled ? 'cursor-not-allowed' : 'cursor-default',
      !disabled ? 'hover:scale-105 hover:shadow-lg' : ''
    ]"
    :role="disabled ? undefined : 'button'"
    @click="onCardClick"
  >
    <div
      class="absolute inset-0 bg-cover bg-center blur-sm grayscale filter transition duration-300 ease-in-out"
      :class="!disabled ? 'group-hover:blur-none group-hover:grayscale-0' : ''"
      :style="{ backgroundImage: `url(${backgroundImage})` }"
    ></div>
    <div class="relative z-10 flex h-full flex-col justify-center p-8 text-center">
      <h2 class="mb-4 rounded-full bg-black/50 py-1 text-xl font-bold text-white">
        {{ header }}
      </h2>
      <slot />
    </div>
  </div>
</template>
