<template>
  <div
    class="radial-gradient relative rounded-full border border-black/25 px-6 py-2 dark:border-white/25"
  >
    <span class="linear-mask relative block h-full w-full font-light tracking-wide">
      <slot>Start now</slot>
    </span>
    <span class="linear-overlay absolute inset-0 block rounded-full p-px" />
  </div>
</template>
