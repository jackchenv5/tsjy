<script setup>
import SieLogo from './SieLogo.vue'
import AppMenuItem from './AppMenuItem.vue'
import { useMenuStateStore } from '../stores/menuStateStore'
import { md, useResize } from '../composables/resize'
import { useAccessibleMenuStore } from '../stores/accessibleMenuStore'

const menuStateStore = useMenuStateStore()

useResize({
  onResize(width) {
    menuStateStore.setClosedBySize(width < md)
  },
})

const accessibleMenuStore = useAccessibleMenuStore()

const handleSelect = () => {
  if (menuStateStore.closedBySize) {
    menuStateStore.setClosedByUser(true)
  }
}

const handleMaskClick = () => {
  menuStateStore.setClosedByUser(true)
}
</script>

<template>
  <div
    class="app-menu-root"
    :class="{
      expand: menuStateStore.isExpanded,
      'small-size': menuStateStore.closedBySize,
    }"
  >
    <SieLogo></SieLogo>
    <div class="menu-wrapper">
      <ElMenu
        class="menu"
        :router="true"
        :default-active="$route.path"
        unique-opened
        @select="handleSelect"
      >
        <AppMenuItem :menu="accessibleMenuStore.accessibleMenu"></AppMenuItem>
      </ElMenu>
    </div>
  </div>
  <div class="mask" @click="handleMaskClick"></div>
</template>

<style scoped lang="scss">
.app-menu-root {
  background-color: var(--el-bg-color-page);
  width: 0;
  height: 100%;
  transition: all 0.3s;
  overflow: hidden;
  z-index: 2000;
  display: flex;
  flex-flow: column nowrap;

  &.expand {
    width: 20rem;
  }

  &.small-size {
    position: fixed;
  }

  &.expand {
    &.small-size {
      & + .mask {
        position: fixed;
        left: 0;
        top: 0;
        right: 0;
        bottom: 0;
        background-color: var(--el-overlay-color-lighter);
        z-index: 1000;
        transition: all 0.3s;
      }
    }
  }

  > .menu-wrapper {
    flex: 1 0;
    overflow: auto;

    > .menu {
      background-color: var(--el-bg-color-overlay);
    }
  }
}
</style>

<style lang="scss">
.app-menu-root {
  > .menu-wrapper {
    .el-menu {
      border-right: none;
    }
  }
}
</style>
