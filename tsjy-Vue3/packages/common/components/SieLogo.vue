<script setup>
import { ref } from 'vue'
import { useDark } from '@vueuse/core'

const logoSrc = ref()
useDark({
  onChanged(isDark) {
    if (isDark) {
      logoSrc.value = new URL(
        '../assets/sie-logo-white-rgb.svg',
        import.meta.url,
      ).href
    } else {
      logoSrc.value = new URL(
        '../assets/sie-logo-petrol-rgb.svg',
        import.meta.url,
      ).href
    }
  },
})
</script>

<template>
  <div class="logo-root">
    <img :src="logoSrc" alt="logo" class="logo" />
  </div>
</template>

<style scoped lang="scss">
.logo-root {
  box-sizing: border-box;
  padding: 0 2rem;
  width: 20rem;
  height: 5rem;
  display: flex;
  cursor: pointer;

  > .logo {
    height: auto;
    width: auto;
    max-height: 100%;
    max-width: 100%;
  }
}
</style>
