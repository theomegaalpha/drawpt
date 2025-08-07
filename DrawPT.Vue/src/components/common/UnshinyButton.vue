<template>
  <button
    class="radial-gradient relative cursor-pointer rounded-full border border-black/15 px-6 py-2 disabled:cursor-not-allowed dark:border-white/15"
  >
    <span
      class="linear-mask relative inline-flex h-full w-full items-center justify-center font-light tracking-wide"
    >
      <slot />
    </span>
    <span class="linear-overlay absolute inset-0 block rounded-full p-px" />
  </button>
</template>
