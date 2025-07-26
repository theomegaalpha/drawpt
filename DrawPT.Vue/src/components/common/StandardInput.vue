<template>
  <div class="relative w-full">
    <input
      :id="id"
      :type="type"
      :value="modelValue"
      @input="handleInput"
      :placeholder="placeholder"
      v-autocapitalize="autocapitalize"
      class="w-full rounded-full border border-gray-300 bg-zinc-100 py-3 pl-12 placeholder-zinc-500 focus:outline-none dark:border-gray-700/50 dark:bg-zinc-900 dark:text-white"
      :class="{ 'cursor-not-allowed': disabled, 'cursor-wait': isLoading }"
      v-bind="$attrs"
      :disabled="disabled"
    />
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
