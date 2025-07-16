<script setup lang="ts">
import { ref, watch } from 'vue'
import { ArrowLeft, ArrowRight, EyeClosedIcon, EyeIcon } from 'lucide-vue-next'
import type { DailyQuestionEntity } from '@/models/dailyModels'

const props = defineProps<{ dailies: DailyQuestionEntity[] }>()

const currentIndex = ref(0)
const showPrompt = ref(false)

function getTransformByIndex(index: number) {
  if (index === currentIndex.value) return ''
  else if (index === currentIndex.value - 1) return `translateX(calc(+80%))`
  else if (index === currentIndex.value + 1) return `translateX(calc(-80%))`
  else if (index === currentIndex.value - 2) return `translateX(calc(+160%))`
  else if (index === currentIndex.value + 2) return `translateX(calc(-160%))`

  return `translateX(calc(10% - ${index * 70}%))`
}

function getImageStyle(index: number) {
  if (index === currentIndex.value) return 'scale-100 opacity-100'
  else if (index === currentIndex.value - 1 || index === currentIndex.value + 1)
    return 'w-4/5 scale-90 opacity-50'
  else if (index === currentIndex.value - 2 || index === currentIndex.value + 2)
    return 'w-4/5 scale-75 opacity-30'

  return 'hidden'
}

function prev() {
  currentIndex.value = currentIndex.value === 0 ? props.dailies.length - 1 : currentIndex.value - 1
}
function next() {
  currentIndex.value = currentIndex.value === props.dailies.length - 1 ? 0 : currentIndex.value + 1
}
function goTo(index: number) {
  if (currentIndex.value !== index) currentIndex.value = index
}

// Re-calculate the closest date index whenever dailies prop updates
watch(
  () => props.dailies,
  (dailies) => {
    if (dailies.length) {
      const today = new Date()
      let closest = 0
      let minDiff = Infinity
      dailies.forEach((daily, idx) => {
        const diff = Math.abs(new Date(daily.date).getTime() - today.getTime())
        if (diff < minDiff) {
          minDiff = diff
          closest = idx
        }
      })
      currentIndex.value = closest
    }
  },
  { immediate: true }
)
</script>

<template>
  <div class="relative mx-auto w-full max-w-lg overflow-hidden" aria-labelledby="DailiesCarousel">
    <ul
      class="mr-32 flex transition-transform duration-500 ease-in-out"
      :style="{ transform: `translateX(calc(30% - ${currentIndex * 100}%))` }"
    >
      <li
        v-for="(daily, index) in dailies"
        :key="index"
        class="flex w-full flex-none items-center justify-center"
      >
        <div
          id="img-container"
          class="relative mr-0 w-fit pr-0 transition-transform duration-500"
          :class="{
            'z-20': index === currentIndex,
            'z-10': index === currentIndex - 1 || index === currentIndex + 1,
            'z-0':
              index !== currentIndex && index !== currentIndex - 1 && index !== currentIndex + 1
          }"
          :style="{
            transform: getTransformByIndex(index)
          }"
        >
          <img
            :src="daily.imageUrl"
            @click="goTo(index)"
            class="transform rounded-lg object-cover transition-all duration-500"
            :class="getImageStyle(index)"
          />
          <span
            v-if="index === currentIndex && showPrompt"
            class="absolute left-1/2 top-1/2 max-h-full w-4/5 -translate-x-1/2 -translate-y-1/2 transform overflow-auto rounded bg-black bg-opacity-50 px-3 py-4 text-sm text-white drop-shadow-lg max-sm:mt-9"
          >
            {{ daily.originalPrompt }}
          </span>
          <div
            v-if="index === currentIndex"
            class="absolute left-1/2 top-1 inline-flex -translate-x-1/2 cursor-pointer items-center space-x-1 rounded bg-black bg-opacity-50 px-2 py-1 text-white hover:text-gray-200 sm:top-8"
            @click="showPrompt = !showPrompt"
            :title="showPrompt ? 'Hide Answer' : 'Show Answer'"
          >
            <EyeIcon v-if="!showPrompt" class="inline-block" />
            <EyeClosedIcon v-else class="inline-block" />
            <span class="whitespace-nowrap">{{ showPrompt ? 'Hide Answer' : 'Show Answer' }}</span>
          </div>
          <span
            v-if="index === currentIndex"
            class="absolute bottom-2 left-1/2 hidden -translate-x-1/2 transform rounded bg-black bg-opacity-50 px-2 py-1 text-sm sm:block"
          >
            {{ daily.date.split('T')[0] }}
          </span>
          <button
            v-if="index === currentIndex"
            class="absolute -left-2 top-1/2 -translate-y-1/2 transform rounded-full bg-white bg-opacity-75 p-2 shadow hover:bg-opacity-100 focus:outline-none"
            title="Previous"
            @click="prev"
          >
            <ArrowLeft class="text-gray-700" />
          </button>
          <button
            v-if="index === currentIndex"
            class="absolute -right-2 top-1/2 -translate-y-1/2 transform rounded-full bg-white bg-opacity-75 p-2 shadow hover:bg-opacity-100 focus:outline-none"
            title="Next"
            @click="next"
          >
            <ArrowRight class="text-gray-700" />
          </button>
        </div>
      </li>
    </ul>
  </div>
</template>
