<script setup lang="ts"></script>

<template>
  <div class="h-full p-6">
    <h1 class="mb-8 text-3xl font-bold">Admin Dashboard</h1>
    <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
      <router-link
        to="/admin/dailies"
        class="block rounded-lg bg-white p-6 shadow transition hover:shadow-lg"
      >
        <h2 class="mb-2 text-xl font-semibold">Dailies</h2>
        <p class="text-gray-600">Manage daily tasks</p>
      </router-link>
    </div>
  </div>
</template>
