<script setup>
import { useLocaleStore } from '#/common/stores/localeStore.js'
import { onMounted } from 'vue'
import { useSawing } from '@/composables/sawing.js'

const localeStore = useLocaleStore()

const { requestAllVariable } = useSawing()
onMounted(async () => {
  await requestAllVariable()
})
</script>

<template>
  <ElConfigProvider :locale="localeStore.locale">
    <RouterView></RouterView>
  </ElConfigProvider>
</template>

<style lang="scss">
html,
body {
  height: 100%;
  width: 100%;
  min-width: 375px;
  padding: 0;
  margin: 0;
  font-size: 10px;
  background-color: var(--el-bg-color);
  color: var(--el-text-color-primary);

  a {
    color: var(--el-color-primary);
    text-decoration: none;

    &:visited {
      color: var(--el-color-primary-light-3);
    }

    &:hover {
      text-decoration: underline;
    }
  }
}

#app {
  height: 100%;
  width: 100%;
  font-size: 1.4rem;
}
</style>
