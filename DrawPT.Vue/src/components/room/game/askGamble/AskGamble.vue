<script setup lang="ts">
import { computed, ref, toRefs } from 'vue'
import { usePlayerStore } from '@/stores/player'
import { useGameStateStore } from '@/stores/gameState'
import type { GameGamble } from '@/models/gameModels'
import GambleOption from './GambleOption.vue'
import Avatar from '@/components/common/Avatar.vue'
import ShinyButton from '@/components/common/ShinyButton.vue'

const props = defineProps<{ gamble: GameGamble }>()
const emit = defineEmits<{ (e: 'gambleSubmitted', gamble: GameGamble): void }>()

const gameStateStore = useGameStateStore()
const { players } = gameStateStore
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

const gamblePlayer = computed(() => players.find((p) => p.connectionId === props.gamble.playerId))

const otherPlayers = computed(() => players.filter((p) => p.connectionId !== you.value.id))

const title = computed(() =>
  otherPlayers.value.length > 1
    ? `Who will score the ${tempGamble.value.isHigh ? 'highest' : 'lowest'}?`
    : `How will ${otherPlayers.value[0].username} score?`
)
</script>

<template>
  <div class="relative flex h-screen flex-col items-center justify-center overflow-y-auto">
    <Avatar
      v-if="gamblePlayer"
      :size="10"
      :username="gamblePlayer.username"
      :avatar="gamblePlayer.avatar || blankAvatar"
      class="mb-4"
    />
    <h1 class="text-xl font-bold">{{ title }}</h1>
    <transition-group tag="div" name="slide-up" class="flex flex-col items-center" appear>
      <div
        v-for="(player, index) in otherPlayers"
        :key="player.id"
        :style="{ transitionDelay: `${otherPlayers.length - 1 - index}s` }"
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
    <ShinyButton class="mt-4" :disabled="!tempGamble.playerId" @click="lockAnswer">
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
