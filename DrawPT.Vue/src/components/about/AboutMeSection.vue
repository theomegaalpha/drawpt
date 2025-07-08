<template>
  <section class="mx-auto max-w-4xl px-4 py-12">
    <div class="flex flex-col items-center gap-8 lg:flex-row">
      <div class="lg:w-1/2">
        <div class="mb-2 w-fit rounded-md bg-black/10 px-4 py-1 dark:bg-white/10">👥 The Team</div>
        <h2 class="mb-4 text-2xl font-bold">Just Me Here 😁</h2>
        <p class="text-md mb-2">
          This project has been (and continues to be) my escape from the routine of a 9-to-5
          programming job. It’s become a creative outlet where I can tinker with everything from the
          small nuances of game mechanics to editing footage for a trailer.
        </p>
        <p class="text-md mb-2">
          Along the way, I’ve learned a lot: not just about coding, but about the challenges that
          come with seeing a project all the way through. It’s taught me patience, persistence, and
          how to push through when the fun parts fade and the hard parts show up.
        </p>
        <p class="text-md mb-2">So, I’m glad you're here playing it. It seriously means a lot.</p>
      </div>
      <div class="lg:w-1/2">
        <img :src="aboutMeGif" alt="About Me" class="w-full rounded-lg" />
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import aboutMeGif from './aboutme.gif'
</script>
