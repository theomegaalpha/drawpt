<template>
  <input
    :id="id"
    :type="type"
    :value="modelValue"
    @input="handleInput"
    :placeholder="placeholder"
    class="border-input ring-offset-background focus-visible:ring-ring flex h-10 w-full rounded-md border bg-white px-3 py-2 text-sm text-gray-900 placeholder-gray-400 file:border-0 file:bg-transparent file:text-sm file:font-medium focus:ring-2 focus:ring-indigo-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 dark:border-zinc-600 dark:bg-zinc-700 dark:text-gray-100 dark:placeholder-zinc-500 dark:ring-offset-zinc-950 dark:focus:ring-indigo-400"
    v-bind="$attrs"
  />
</template>

<script setup lang="ts">
interface Props {
  modelValue: string | number | null
  id?: string
  placeholder?: string
  type?: string
}

withDefaults(defineProps<Props>(), {
  type: 'text',
  modelValue: ''
})

const emit = defineEmits(['update:modelValue'])

const handleInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.value)
}
</script>
