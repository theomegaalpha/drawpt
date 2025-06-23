<template>
  <div class="relative mx-auto h-[80vmin] w-[70vmin]" :aria-labelledby="`carousel-heading-${id}`">
    <ul
      class="absolute mx-[-4vmin] flex transition-transform duration-1000 ease-in-out"
      :style="{ transform: `translateX(-${current * (100 / dailies.length)}%)` }"
    >
      <li
        v-for="(slide, index) in dailies"
        :key="index"
        ref="el => (slideRefs[index] = el)"
        class="relative z-10 mx-[4vmin] flex h-[70vmin] w-[70vmin] flex-1 flex-col items-center justify-center text-center text-white transition-transform duration-500 ease-[cubic-bezier(0.4,0,0.2,1)]"
        :style="{
          transform: current !== index ? 'scale(0.98) rotateX(8deg)' : 'scale(1) rotateX(0deg)',
          transformOrigin: 'bottom'
        }"
        @click="goTo(index)"
        @mousemove="onMouseMove(index, $event)"
        @mouseleave="onMouseLeave(index)"
      >
        <div
          class="absolute left-0 top-0 h-full w-full overflow-hidden rounded-[1%] bg-[#1D1F2F] transition-all duration-150 ease-out"
          :style="{
            transform:
              current === index ? 'translate3d(calc(var(--x)/30), calc(var(--y)/30), 0)' : 'none'
          }"
        >
          <img
            :src="slide.imageUrl"
            :alt="slide.theme"
            class="duration-600 absolute inset-0 h-[120%] w-[120%] object-cover transition-opacity ease-in-out"
            :style="{ opacity: current === index ? 1 : 0.5 }"
            @load="onImageLoad"
            loading="eager"
            decoding="sync"
          />
          <div
            v-if="current === index"
            class="absolute inset-0 bg-black/30 transition-all duration-1000"
          />
        </div>
        <article
          class="relative p-[4vmin] transition-opacity duration-1000 ease-in-out"
          :class="{
            'visible opacity-100': current === index,
            'invisible opacity-0': current !== index
          }"
        >
          <h2 class="relative text-lg font-semibold md:text-2xl lg:text-4xl">
            {{ slide.theme }}
          </h2>
          <div class="flex justify-center">
            <button
              class="mx-auto mt-6 flex h-12 w-fit items-center justify-center rounded-2xl border-transparent bg-white px-4 py-2 text-xs text-black shadow-[0px_2px_3px_-1px_rgba(0,0,0,0.1),0px_1px_0px_0px_rgba(25,28,33,0.02),0px_0px_0px_1px_rgba(25,28,33,0.08)] transition duration-200 hover:shadow-lg sm:text-sm"
            >
              {{ slide.style }}
            </button>
          </div>
        </article>
      </li>
    </ul>

    <div class="absolute top-[calc(100%+1rem)] flex w-full justify-center">
      <button
        class="mx-2 flex h-10 w-10 items-center justify-center rounded-full border-transparent bg-neutral-200 transition duration-200 hover:-translate-y-0.5 focus:border-[#6D64F7] focus:outline-none active:translate-y-0.5 dark:bg-neutral-800"
        title="Go to previous slide"
        @click="prev"
      >
        <ArrowLeft class="text-neutral-600 dark:text-neutral-200" />
      </button>
      <button
        class="mx-2 flex h-10 w-10 items-center justify-center rounded-full border-transparent bg-neutral-200 transition duration-200 hover:-translate-y-0.5 focus:border-[#6D64F7] focus:outline-none active:translate-y-0.5 dark:bg-neutral-800"
        title="Go to next slide"
        @click="next"
      >
        <ArrowRight class="text-neutral-600 dark:text-neutral-200" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue'
import { ArrowLeft, ArrowRight } from 'lucide-vue-next'
import type { DailyQuestion } from '@/models/dailyModels'

const props = defineProps<{ dailies: DailyQuestion[] }>()

const current = ref(0)
const id = Math.random().toString(36).substr(2, 9)

const slideRefs = ref<Array<HTMLElement | null>>([])
const xRefs: number[] = []
const yRefs: number[] = []
let frameId: number

function prev() {
  current.value = current.value === 0 ? props.dailies.length - 1 : current.value - 1
}
function next() {
  current.value = current.value === props.dailies.length - 1 ? 0 : current.value + 1
}
function goTo(index: number) {
  if (current.value !== index) current.value = index
}

function onImageLoad(event: Event) {
  ;(event.target as HTMLImageElement).style.opacity = '1'
}

function animate() {
  slideRefs.value.forEach((el, idx) => {
    if (!el) return
    el.style.setProperty('--x', `${xRefs[idx]}px`)
    el.style.setProperty('--y', `${yRefs[idx]}px`)
  })
  frameId = requestAnimationFrame(animate)
}

function onMouseMove(index: number, event: MouseEvent) {
  const el = slideRefs.value[index]
  if (!el) return
  const r = el.getBoundingClientRect()
  xRefs[index] = event.clientX - (r.left + r.width / 2)
  yRefs[index] = event.clientY - (r.top + r.height / 2)
}
function onMouseLeave(index: number) {
  xRefs[index] = 0
  yRefs[index] = 0
}

onMounted(() => {
  props.dailies.forEach(() => {
    xRefs.push(0)
    yRefs.push(0)
  })
  animate()
})

onBeforeUnmount(() => {
  cancelAnimationFrame(frameId)
})
</script>
