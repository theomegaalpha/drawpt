<template>
  <div
    class="relative w-full rounded-full border border-gray-300 bg-zinc-100 py-3 pl-4 dark:border-gray-700/50 dark:bg-zinc-900"
  >
    <textarea
      rows="2"
      :id="id"
      :value="modelValue"
      @input="handleInput"
      :placeholder="placeholder"
      v-autocapitalize="autocapitalize"
      style="background-color: transparent !important"
      class="w-full resize-none overflow-auto bg-none pl-8 pr-2 placeholder-zinc-500 focus:outline-none dark:text-white"
      :class="{ 'cursor-not-allowed': disabled, 'cursor-wait': isLoading }"
      v-bind="$attrs"
      :disabled="disabled"
    ></textarea>
  </div>
</template>

<script setup lang="ts">
interface Props {
  modelValue: string | number | null
  id?: string
  placeholder?: string
  type?: string
  autocapitalize?: boolean
  disabled?: boolean
  isLoading?: boolean
}

withDefaults(defineProps<Props>(), {
  type: 'text',
  modelValue: '',
  autocapitalize: false,
  disabled: false
})

const emit = defineEmits(['update:modelValue'])

const handleInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.value)
}
</script>
