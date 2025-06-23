<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useDailiesStore } from '@/stores/dailies'
import DailiesCarousel from '@/components/common/DailiesCarousel.vue'
import StandardInput from '@/components/common/StandardInput.vue'
import api from '@/api/api'
import type { DailyQuestionEntity } from '@/models/dailyModels'

const dailiesStore = useDailiesStore()
const { dailyQuestion } = storeToRefs(dailiesStore)
const futureQuestions = ref([] as DailyQuestionEntity[])

const selectedDate = ref(new Date().toISOString().substr(0, 10))

const generate = () => {
  api
    .createQuestion(new Date(selectedDate.value))
    .then((response) => {
      console.log('Daily question generated:', response)
      populateFutureQuestions()
    })
    .catch((error) => {
      console.error('Error generating daily question:', error)
    })
}

const populateFutureQuestions = () => {
  api
    .getFutureDailyQuestions()
    .then((response) => {
      futureQuestions.value = response
      console.log('Future daily questions:', futureQuestions.value)
    })
    .catch((error) => {
      console.error('Error fetching future daily questions:', error)
    })
}

onMounted(() => {
  populateFutureQuestions()
})
</script>

<template>
  <div class="h-fit p-6">
    <div class="mb-6 flex items-center space-x-2">
      <StandardInput type="date" v-model="selectedDate" class="max-w-xs" />
      <button @click="generate" class="btn-default">Generate</button>
    </div>
    <div class="flex space-x-4 overflow-x-auto py-4">
      <DailiesCarousel :dailies="[dailyQuestion]" />
    </div>
  </div>
</template>
