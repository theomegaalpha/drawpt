<template>
  <section class="mx-auto max-w-4xl px-4 py-12 lg:max-w-6xl lg:px-6">
    <div class="flex flex-col items-center gap-8 lg:flex-row">
      <div class="relative w-full lg:w-[calc(50%+4rem)]">
        <iframe
          class="aspect-video h-full w-full lg:-ml-24 lg:w-[calc(100%+4rem)]"
          src="https://www.youtube.com/embed/tctpLLkZbWQ"
          title="YouTube video player"
          frameborder="0"
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
          allowfullscreen
        ></iframe>
      </div>
      <div class="rounded-md bg-white/20 px-4 py-12 lg:w-1/2 dark:bg-black/20">
        <div class="mb-2 w-fit rounded-md bg-black/10 px-4 py-1 dark:bg-white/10">
          🎮 Gameplay Highlights
        </div>
        <h2 class="mb-4 text-2xl font-bold">What’s Here Now</h2>
        <p class="text-md mb-8">
          Right now, the game’s all about the guessing. The AI gets a secret prompt and tries its
          best to draw it. You and everyone else in the lobby see the same drawing and try to figure
          out what the original prompt was.
        </p>
        <h2 class="my-4 text-2xl font-bold">What’s Coming Next</h2>
        <p class="text-md mb-2">
          I’m working on a new mode where you get to write the prompts. Each round, one player will
          come up with a creative, weird, or hilarious idea. The AI will try to draw it, and the
          rest of the lobby will guess what it is.
        </p>
        <p class="text-md mb-2">
          It’s still in the works, but I wanted to share what’s coming because the project keeps
          growing (much more than I initially thought it would). I haven't figured out all the game
          mechanics yet, but I have some ideas that could make it even more fun.
        </p>
      </div>
    </div>
  </section>
</template>
