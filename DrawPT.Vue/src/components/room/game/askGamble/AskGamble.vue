<script setup lang="ts">
import { computed, ref, toRefs, onMounted } from 'vue'
import { usePlayerStore } from '@/stores/player'
import { useGameStateStore } from '@/stores/gameState'
import type { GameGamble } from '@/models/gameModels'
import GambleOption from './GambleOption.vue'
import Avatar from '@/components/common/Avatar.vue'
import ShinyButton from '@/components/common/ShinyButton.vue'

const props = defineProps<{ gamble: GameGamble }>()
const emit = defineEmits<{ (e: 'gambleSubmitted', gamble: GameGamble): void }>()

const gameStateStore = useGameStateStore()
const { currentTheme, currentImageUrl, currentPrompt, players } = gameStateStore
const playerStore = usePlayerStore()
const { blankAvatar, player: you } = toRefs(playerStore)

const tempGamble = ref<GameGamble>({ ...props.gamble })

function selectPlayer(playerId: string) {
  tempGamble.value.playerId = playerId
}

function updateGamble(updatedGamble: GameGamble) {
  selectPlayer(updatedGamble.playerId)
  tempGamble.value.isHigh = updatedGamble.isHigh
}

function lockAnswer() {
  if (!tempGamble.value.playerId) return
  const result: GameGamble = {
    ...props.gamble,
    playerId: tempGamble.value.playerId,
    isHigh: tempGamble.value.isHigh
  }
  emit('gambleSubmitted', result)
}

const gamblePlayer = computed(() => players.find((p) => p.id === props.gamble.playerId))
const otherPlayers = computed(() => players.filter((p) => p.id !== you.value.id))
const title = computed(() =>
  otherPlayers.value.length > 1
    ? `Who will score the ${tempGamble.value.isHigh ? 'highest' : 'lowest'}?`
    : `How will ${otherPlayers.value[0].username} score?`
)

const faded = ref(false)
onMounted(() => {
  if (otherPlayers.value.length === 1) tempGamble.value.playerId = otherPlayers.value[0].id

  setTimeout(() => {
    faded.value = true
  }, 1000)
})
</script>

<template>
  <div class="relative flex h-screen flex-col items-center justify-center overflow-y-auto">
    <img
      v-if="currentImageUrl !== ''"
      class="absolute left-1/2 top-1/2 z-0 max-h-[70vh] max-w-[1048px] -translate-x-1/2 -translate-y-1/2 rounded-lg transition-opacity delay-100 duration-1000"
      :class="faded ? 'opacity-20' : 'opacity-100'"
      :src="currentImageUrl"
    />
    <Avatar
      v-if="gamblePlayer"
      :size="10"
      :username="gamblePlayer.username"
      :avatar="gamblePlayer.avatar || blankAvatar"
      class="mb-4"
    />
    <h1 class="text-xl font-bold">{{ title }}</h1>
    <div
      class="relative z-10 ml-4 flex w-full max-w-5xl cursor-pointer items-center rounded-lg bg-gray-500/10 p-4 px-6 backdrop-blur hover:bg-gray-500/20 dark:bg-white/10 dark:hover:bg-white/20"
    >
      <div class="font-semibold">[{{ currentTheme }}]</div>
      <div>{{ currentPrompt }}</div>
    </div>
    <transition-group tag="div" name="slide-up" class="flex flex-col items-center" appear>
      <div
        v-for="(player, index) in otherPlayers"
        :key="player.id"
        :style="{ transitionDelay: `${otherPlayers.length - index}s` }"
        class="cursor-pointer"
        @click="selectPlayer(player.id)"
      >
        <GambleOption
          :player="player"
          :isDuelMode="otherPlayers.length === 1"
          :gamble="tempGamble"
          @gambleSubmitted="updateGamble"
        />
      </div>
    </transition-group>
    <ShinyButton
      class="mt-4 transition-opacity duration-1000 ease-in-out"
      :class="faded ? 'opacity-100' : 'opacity-0'"
      :disabled="!tempGamble.playerId"
      @click="lockAnswer"
    >
      Lock Answer
    </ShinyButton>
  </div>
</template>

<style scoped>
.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.5s ease-out;
}
.slide-up-enter-from,
.slide-up-leave-to {
  opacity: 0;
  transform: translateY(30px);
}
</style>
