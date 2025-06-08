<template>
  <div class="relative w-full">
    <input
      :id="id"
      :type="type"
      :value="modelValue"
      @input="handleInput"
      :placeholder="placeholder"
      v-autocapitalize="autocapitalize"
      class="w-full rounded-full bg-zinc-900 px-5 py-3 pr-12 text-white placeholder-zinc-500 focus:outline-none"
      v-bind="$attrs"
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
}

withDefaults(defineProps<Props>(), {
  type: 'text',
  modelValue: '',
  autocapitalize: false
})

const emit = defineEmits(['update:modelValue'])

const handleInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.value)
}
</script>
