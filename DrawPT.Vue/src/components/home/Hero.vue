<template>
  <div>
    <header>
      <nav
        :data-state="menuState && 'active'"
        class="group fixed z-20 w-full border-b border-dotted bg-white backdrop-blur md:relative dark:border-white/20 dark:bg-zinc-950/50 lg:dark:bg-transparent"
      >
        <div class="m-auto max-w-5xl px-6">
          <div class="flex flex-wrap items-center justify-between gap-6 py-3 lg:gap-0 lg:py-4">
            <div class="flex w-full justify-between lg:w-auto">
              <router-link to="/" aria-label="home" class="flex items-center space-x-2">
                <img src="@/assets/logo.png" alt="logo" class="mr-2 h-12 w-12" />
              </router-link>
            </div>

            <div class="lg:pr-4">
              <ul class="space-y-6 text-base lg:flex lg:gap-8 lg:space-y-0 lg:text-sm">
                <li v-for="(item, index) in menuItems" :key="index">
                  <router-link
                    :to="item.href"
                    class="text-muted-foreground hover:text-accent-foreground block duration-150"
                  >
                    <span>{{ item.name }}</span>
                  </router-link>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </nav>
    </header>

    <main>
      <section class="overflow-hidden bg-white dark:bg-transparent">
        <div class="relative mx-auto max-w-5xl px-6 py-28 lg:py-24">
          <div class="relative z-10 mx-auto max-w-2xl text-center">
            <h1 class="text-balance text-4xl font-semibold md:text-5xl lg:text-6xl">
              Modern Software testing reimagined
            </h1>
            <p class="mx-auto my-8 max-w-2xl text-xl">
              Officiis laudantium excepturi ducimus rerum dignissimos, and tempora nam vitae,
              excepturi ducimus iste provident dolores.
            </p>

            <Button asChild size="lg">
              <router-link to="#">
                <span class="btn-label">Start Building</span>
              </router-link>
            </Button>
          </div>
        </div>

        <div
          class="mx-auto -mt-16 max-w-7xl [mask-image:linear-gradient(to_bottom,black_50%,transparent_100%)]"
        >
          <div
            class="-mr-16 pl-16 [mask-image:linear-gradient(to_right,black_50%,transparent_100%)] [perspective:1200px] lg:-mr-56 lg:pl-56"
          >
            <div class="[transform:rotateX(20deg);]">
              <div class="relative skew-x-[.36rad] lg:h-[44rem]">
                <img
                  class="relative z-[2] rounded-[--radius] border dark:hidden"
                  src="https://tailark.com/_next/image?url=%2Fcard.png&w=3840&q=75"
                  alt="Tailark hero section"
                  width="2880"
                  height="2074"
                />
                <img
                  class="relative z-[2] hidden rounded-[--radius] border dark:block"
                  src="https://tailark.com/_next/image?url=%2Fdark-card.webp&w=3840&q=75"
                  alt="Tailark hero section"
                  width="2880"
                  height="2074"
                />
              </div>
            </div>
          </div>
        </div>
      </section>
      <section class="bg-background relative z-10 py-16">
        <div class="m-auto max-w-5xl px-6">
          <h2 class="text-center text-lg font-medium">Your favorite companies are our partners.</h2>
          <div
            class="mx-auto mt-20 flex max-w-4xl flex-wrap items-center justify-center gap-x-12 gap-y-8 sm:gap-x-16 sm:gap-y-12"
          >
            <img
              class="h-5 w-fit dark:invert"
              src="https://html.tailus.io/blocks/customers/nvidia.svg"
              alt="Nvidia Logo"
              height="20"
              width="auto"
            />
            <img
              class="h-4 w-fit dark:invert"
              src="https://html.tailus.io/blocks/customers/column.svg"
              alt="Column Logo"
              height="16"
              width="auto"
            />
            <img
              class="h-4 w-fit dark:invert"
              src="https://html.tailus.io/blocks/customers/github.svg"
              alt="GitHub Logo"
              height="16"
              width="auto"
            />
            <img
              class="h-5 w-fit dark:invert"
              src="https://html.tailus.io/blocks/customers/nike.svg"
              alt="Nike Logo"
              height="20"
              width="auto"
            />
            <img
              class="h-4 w-fit dark:invert"
              src="https://html.tailus.io/blocks/customers/laravel.svg"
              alt="Laravel Logo"
              height="16"
              width="auto"
            />
            <img
              class="h-7 w-fit dark:invert"
              src="https://html.tailus.io/blocks/customers/lilly.svg"
              alt="Lilly Logo"
              height="28"
              width="auto"
            />
            <img
              class="h-5 w-fit dark:invert"
              src="https://html.tailus.io/blocks/customers/lemonsqueezy.svg"
              alt="Lemon Squeezy Logo"
              height="20"
              width="auto"
            />
            <img
              class="h-6 w-fit dark:invert"
              src="https://html.tailus.io/blocks/customers/openai.svg"
              alt="OpenAI Logo"
              height="24"
              width="auto"
            />
            <img
              class="h-4 w-fit dark:invert"
              src="https://html.tailus.io/blocks/customers/tailwindcss.svg"
              alt="Tailwind CSS Logo"
              height="16"
              width="auto"
            />
            <img
              class="h-5 w-fit dark:invert"
              src="https://html.tailus.io/blocks/customers/vercel.svg"
              alt="Vercel Logo"
              height="20"
              width="auto"
            />
            <img
              class="h-5 w-fit dark:invert"
              src="https://html.tailus.io/blocks/customers/zapier.svg"
              alt="Zapier Logo"
              height="20"
              width="auto"
            />
          </div>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const menuItems = [
  { name: 'Features', href: '#' },
  { name: 'Solution', href: '#' },
  { name: 'Pricing', href: '#' },
  { name: 'About', href: '#' }
]

const menuState = ref(false)
</script>
