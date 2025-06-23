<template>
  <div class="relative mx-auto w-full max-w-lg overflow-hidden" aria-labelledby="DailiesCarousel">
    <ul
      class="flex transition-transform duration-500 ease-in-out"
      :style="{ transform: `translateX(-${current * 100}%)` }"
    >
      <li
        v-for="(daily, index) in dailies"
        :key="index"
        class="flex w-full flex-none items-center justify-center"
      >
        <img
          :src="daily.imageUrl"
          @click="goTo(index)"
          class="w-full transform rounded-lg object-cover transition-opacity duration-500"
          :class="{
            'scale-100 opacity-100': index === current,
            'scale-90 opacity-50': index === current - 1 || index === current + 1,
            'scale-75 opacity-30':
              index !== current && index !== current - 1 && index !== current + 1
          }"
        />
      </li>
    </ul>

    <!-- Navigation buttons -->
    <div class="flex">
      <button
        class="rounded-full bg-white bg-opacity-75 p-2 shadow hover:bg-opacity-100 focus:outline-none"
        title="Previous"
        @click="prev"
      >
        <ArrowLeft class="text-gray-700" />
      </button>
      <button
        class="rounded-full bg-white bg-opacity-75 p-2 shadow hover:bg-opacity-100 focus:outline-none"
        title="Next"
        @click="next"
      >
        <ArrowRight class="text-gray-700" />
      </button>
    </div>
    <div class="flex">{{ current + 1 }} / {{ dailies.length }}</div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ArrowLeft, ArrowRight } from 'lucide-vue-next'
import type { DailyQuestion } from '@/models/dailyModels'

const props = defineProps<{ dailies: DailyQuestion[] }>()

const current = ref(0)

function prev() {
  current.value = current.value === 0 ? props.dailies.length - 1 : current.value - 1
}
function next() {
  current.value = current.value === props.dailies.length - 1 ? 0 : current.value + 1
}
function goTo(index: number) {
  if (current.value !== index) current.value = index
}

onMounted(() => {})
</script>
