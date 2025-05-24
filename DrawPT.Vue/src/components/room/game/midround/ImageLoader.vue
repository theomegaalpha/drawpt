<script setup lang="ts">
import { ref } from 'vue'
const num = ref(Array(16).fill(0))
</script>

<template>
  <div class="container">
    <div
      class="circle"
      v-for="(n, index) in num"
      :key="index"
      :data-index="index"
      :style="{ animationDuration: 22 / (index + 1) + 's' }"
    ></div>
  </div>
</template>

<style scoped lang="scss">
// setup variables
$color: #fff;
$size: 50vh;
$count: 16;
$step: 1vh;

.container {
  position: relative;
  display: block;
  width: $size;
  height: $size;
  transform-style: preserve-3d;
  transform: perspective(1000px) rotateY(45deg) rotateX(45deg);
}

@for $i from 1 through $count {
  .circle:nth-child(#{$i}) {
    position: absolute;
    background: transparent;
    border: 2px solid $color;
    border-radius: 50%;
    left: $step * ($i - 1);
    top: $step * ($i - 1);
    width: $size - $step * ($i - 1) * 2;
    height: $size - $step * ($i - 1) * 2;
    animation: spin infinite linear;
  }
  .circle:nth-child(2n) {
    border: 2px dashed rgba(255, 255, 255, 0.5);
  }
  .circle:last-child {
    display: none;
  }
}

@keyframes spin {
  0% {
    transform: rotateX(0deg);
  }
  100% {
    transform: rotateX(360deg);
  }
}
</style>
