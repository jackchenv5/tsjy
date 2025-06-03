<script setup>
import MenuToggle from './MenuToggle.vue'
import DarkToggle from './DarkToggle.vue'
import { useTitleStore } from '../stores/titleStore'
import UserAvatar from './UserAvatar.vue'

const titleStore = useTitleStore()
</script>

<template>
  <div class="app-header-root">
    <MenuToggle></MenuToggle>
    <div class="app-name">{{ titleStore.title }}</div>
    <div class="right">
      <DarkToggle></DarkToggle>
      <UserAvatar></UserAvatar>
    </div>
  </div>
</template>

<style scoped lang="scss">
.app-header-root {
  width: 100%;
  height: 5rem;
  min-height: 5rem;
  display: flex;
  flex-flow: row nowrap;
  align-items: center;
  border-bottom: 1px solid var(--el-border-color);

  > .app-name {
    display: flex;
    flex-grow: 1;
    justify-content: center;
    font-size: 1.6rem;
    font-weight: bold;
  }

  > .right {
    display: flex;
    flex-wrap: nowrap;
    flex-direction: row;
    justify-content: flex-end;
    align-items: center;
    padding-right: 2rem;
    grid-gap: 2rem;
  }
}
</style>
