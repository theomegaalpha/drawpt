<template>
  <div ref="containerRef" :class="['relative h-full min-h-screen w-full', props.className]">
    <canvas
      ref="canvasRef"
      class="pointer-events-none absolute bottom-0 left-0 right-0 top-0 z-0"
    />
    <div class="absolute left-0 top-0 z-10 h-full w-full">
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'

interface Props {
  squareSize?: number
  gridGap?: number
  flickerChance?: number
  color?: string
  width?: number // Custom width for the canvas
  height?: number // Custom height for the canvas
  className?: string // Additional classes for the container div
  maxOpacity?: number
  darkModeColor?: string // Color for dark mode
}

const props = withDefaults(defineProps<Props>(), {
  squareSize: 5,
  gridGap: 7,
  flickerChance: 0.3,
  color: 'rgb(0, 0, 0)',
  darkModeColor: 'rgb(255, 255, 255)',
  className: '',
  maxOpacity: 0.3
})

const canvasRef = ref<HTMLCanvasElement | null>(null)
const containerRef = ref<HTMLDivElement | null>(null)
const isInView = ref(false)
const canvasSize = ref({ width: 0, height: 0 })
const isDarkMode = ref(false)

let animationFrameId: number = 0
let gridParams: ReturnType<typeof setupCanvasInternal> | undefined
let resizeObserver: ResizeObserver
let intersectionObserver: IntersectionObserver
// let lastTime = 0 // Replaced by lastLogicExecutionTime and lastScheduledRenderTime
let ctx: CanvasRenderingContext2D | null = null

// --- Optimizations Start ---
const TARGET_FPS = 15 // Target FPS for the animation
const RENDER_INTERVAL_MS = 1000 / TARGET_FPS
let lastLogicExecutionTime = 0 // Timestamp of the last logic update/draw
let lastScheduledRenderTime = 0 // Timestamp for scheduling renders

const colorCache = new Map<string, string>()
// --- Optimizations End ---

// Watch for dark mode changes
const checkDarkMode = () => {
  isDarkMode.value =
    window.matchMedia('(prefers-color-scheme: dark)').matches ||
    document.documentElement.classList.contains('dark')
}

const memoizedColor = computed(() => {
  const toRGBA = (colorVal: string) => {
    if (colorCache.has(colorVal)) {
      return colorCache.get(colorVal)!
    }
    if (typeof window === 'undefined') {
      // Return a fully transparent black as a fallback for SSR or no window
      return 'rgba(0, 0, 0, 0)'
    }
    const canvas = document.createElement('canvas')
    canvas.width = canvas.height = 1
    const tempCtx = canvas.getContext('2d')
    if (!tempCtx) {
      console.error('FlickeringGrid: Failed to get 2D context for color conversion')
      return 'rgba(0, 0, 0, 0)' // Fallback
    }
    tempCtx.fillStyle = colorVal
    tempCtx.fillRect(0, 0, 1, 1)
    const [r, g, b] = Array.from(tempCtx.getImageData(0, 0, 1, 1).data)
    // Keep the trailing comma for opacity to be appended later
    const result = `rgba(${r}, ${g}, ${b},`
    colorCache.set(colorVal, result)
    return result
  }
  return toRGBA(isDarkMode.value ? props.darkModeColor : props.color)
})

const setupCanvasInternal = (
  canvas: HTMLCanvasElement,
  currentWidth: number,
  currentHeight: number
) => {
  const dpr = window.devicePixelRatio || 1
  canvas.width = currentWidth * dpr
  canvas.height = currentHeight * dpr
  canvas.style.width = `${currentWidth}px`
  canvas.style.height = `${currentHeight}px`

  const effectiveSquareSize = props.squareSize
  const effectiveGridGap = props.gridGap

  const cols = Math.floor(currentWidth / (effectiveSquareSize + effectiveGridGap))
  const rows = Math.floor(currentHeight / (effectiveSquareSize + effectiveGridGap))

  const squares = new Float32Array(cols * rows)
  for (let i = 0; i < squares.length; i++) {
    squares[i] = Math.random() * props.maxOpacity
  }
  return { cols, rows, squares, dpr, width: currentWidth, height: currentHeight }
}

const updateSquares = (squares: Float32Array, deltaTime: number) => {
  for (let i = 0; i < squares.length; i++) {
    if (Math.random() < props.flickerChance * deltaTime) {
      squares[i] = Math.random() * props.maxOpacity
    }
  }
}

const drawGrid = (
  context: CanvasRenderingContext2D,
  params: NonNullable<typeof gridParams> // Ensure gridParams is not undefined here
) => {
  context.clearRect(0, 0, params.width * params.dpr, params.height * params.dpr)

  const effectiveSquareSize = props.squareSize
  const effectiveGridGap = props.gridGap

  for (let i = 0; i < params.cols; i++) {
    for (let j = 0; j < params.rows; j++) {
      const opacity = params.squares[i * params.rows + j]
      context.fillStyle = `${memoizedColor.value}${opacity})`
      context.fillRect(
        i * (effectiveSquareSize + effectiveGridGap) * params.dpr,
        j * (effectiveSquareSize + effectiveGridGap) * params.dpr,
        effectiveSquareSize * params.dpr,
        effectiveSquareSize * params.dpr
      )
    }
  }
}

const animate = (timestamp: number) => {
  animationFrameId = requestAnimationFrame(animate) // Keep rAF scheduling

  if (!isInView.value || !gridParams || !ctx) {
    // If not in view, or not ready, skip update/draw.
    // IntersectionObserver handles cancelling rAF when not in view.
    return
  }

  // lastScheduledRenderTime and lastLogicExecutionTime are initialized by onMounted or intersectionObserver

  const elapsedSinceLastScheduled = timestamp - lastScheduledRenderTime

  if (elapsedSinceLastScheduled >= RENDER_INTERVAL_MS) {
    // Adjust lastScheduledRenderTime to maintain consistent interval
    lastScheduledRenderTime = timestamp - (elapsedSinceLastScheduled % RENDER_INTERVAL_MS)

    // Calculate deltaTime based on actual time passed since last logic execution
    const deltaTimeSeconds = (timestamp - lastLogicExecutionTime) / 1000
    lastLogicExecutionTime = timestamp // Update time of this execution

    updateSquares(gridParams.squares, deltaTimeSeconds)
    drawGrid(ctx, gridParams)
  }
}

const updateCanvasSizeAndSetup = () => {
  const canvas = canvasRef.value
  const container = containerRef.value
  if (!canvas || !container || !ctx) return // Added ctx check

  const newWidth = props.width !== undefined ? props.width : container.clientWidth
  const newHeight = props.height !== undefined ? props.height : container.clientHeight

  canvasSize.value = { width: newWidth, height: newHeight }
  gridParams = setupCanvasInternal(canvas, newWidth, newHeight)
  // If it was already animating, the loop will pick up the new params
  // If it wasn't (e.g. first setup or was out of view), ensure drawGrid is called once
  if (isInView.value && gridParams && ctx) {
    drawGrid(ctx, gridParams)
  }
}

onMounted(() => {
  const canvas = canvasRef.value
  const container = containerRef.value

  if (!canvas || !container) {
    console.error('FlickeringGrid: Canvas or container element not found.')
    return
  }

  // Initial dark mode check
  checkDarkMode()

  // Watch for dark mode changes
  const observer = new MutationObserver((mutations) => {
    mutations.forEach((mutation) => {
      if (mutation.attributeName === 'class') {
        checkDarkMode()
        // Redraw the grid with new colors
        if (gridParams && ctx) {
          drawGrid(ctx, gridParams)
        }
      }
    })
  })

  observer.observe(document.documentElement, {
    attributes: true,
    attributeFilter: ['class']
  })

  const localCtx = canvas.getContext('2d')
  if (!localCtx) {
    console.error('FlickeringGrid: Failed to get 2D rendering context.')
    return
  }
  ctx = localCtx

  // Force initial setup with container dimensions
  const initialWidth = container.clientWidth || window.innerWidth
  const initialHeight = container.clientHeight || window.innerHeight
  canvasSize.value = { width: initialWidth, height: initialHeight }
  gridParams = setupCanvasInternal(canvas, initialWidth, initialHeight)

  // Draw initial frame
  if (gridParams && ctx) {
    drawGrid(ctx, gridParams)
  }

  updateCanvasSizeAndSetup() // Initial setup

  resizeObserver = new ResizeObserver(() => {
    updateCanvasSizeAndSetup()
  })
  resizeObserver.observe(container)

  intersectionObserver = new IntersectionObserver(
    ([entry]) => {
      isInView.value = entry.isIntersecting
      if (isInView.value) {
        const now = performance.now()
        lastScheduledRenderTime = now // Initialize for throttled animation
        lastLogicExecutionTime = now // Initialize
        if (!animationFrameId) {
          // Start animation if not already running
          animationFrameId = requestAnimationFrame(animate)
        }
      } else {
        if (animationFrameId) {
          cancelAnimationFrame(animationFrameId)
          animationFrameId = 0
        }
        // Reset timers when not in view
        lastScheduledRenderTime = 0
        lastLogicExecutionTime = 0
      }
    },
    { threshold: 0 }
  )
  intersectionObserver.observe(canvas)

  // Watch for prop changes to re-initialize or update the grid
  watch(
    () => ({
      squareSize: props.squareSize,
      gridGap: props.gridGap,
      maxOpacity: props.maxOpacity,
      color: props.color,
      darkModeColor: props.darkModeColor,
      width: props.width,
      height: props.height,
      flickerChance: props.flickerChance
    }),
    () => {
      if (canvasRef.value && containerRef.value && ctx) {
        updateCanvasSizeAndSetup()
      }
    },
    { deep: true }
  )

  // Initial animation start if in view
  if (isInView.value) {
    const now = performance.now()
    lastScheduledRenderTime = now // Initialize for throttled animation
    lastLogicExecutionTime = now // Initialize
    // animationFrameId = requestAnimationFrame(animate) // Already handled by intersection observer logic if initially in view
    // Ensure animation starts if initially in view and observer hasn't fired yet or if logic needs explicit start
    if (
      !animationFrameId &&
      canvasRef.value &&
      intersectionObserver
        .takeRecords()
        .some((e) => e.isIntersecting || e.target === canvasRef.value)
    ) {
      isInView.value = true // Ensure isInView is true
      lastScheduledRenderTime = performance.now()
      lastLogicExecutionTime = performance.now()
      animationFrameId = requestAnimationFrame(animate)
    } else if (!animationFrameId && isInView.value) {
      // Fallback if observer logic is tricky for initial state
      lastScheduledRenderTime = performance.now()
      lastLogicExecutionTime = performance.now()
      animationFrameId = requestAnimationFrame(animate)
    }
  }
})

onUnmounted(() => {
  if (animationFrameId) {
    // Check before cancelling
    cancelAnimationFrame(animationFrameId)
    animationFrameId = 0
  }
  if (resizeObserver) resizeObserver.disconnect()
  if (intersectionObserver) intersectionObserver.disconnect()
  // Reset timers
  lastScheduledRenderTime = 0
  lastLogicExecutionTime = 0
})
</script>

<style scoped>
.relative {
  position: relative;
  display: block;
  width: 100%;
  height: 100%;
}

canvas {
  display: block;
  width: 100%;
  height: 100%;
}
</style>
