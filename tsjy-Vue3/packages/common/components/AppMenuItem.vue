<script setup>
import { ElSubMenu, ElMenuItem } from 'element-plus'

defineProps(['menu'])
</script>

<template>
  <Component
    v-for="item in menu"
    :is="item['isSubMenu'] ? ElSubMenu : ElMenuItem"
    :key="item.id"
    :class="item['isSubMenu'] ? 'sub-menu' : 'menu-item'"
    :index="item.route ? item.route : item.label"
  >
    <template #title>
      {{ item.label }}
    </template>
    <template v-if="item['isSubMenu'] && item['children']">
      <AppMenuItem :menu="item['children']"></AppMenuItem>
    </template>
  </Component>
</template>

<style scoped lang="scss">
.sub-menu {
  background-color: var(--el-bg-color-page);
}

.menu-item {
  background-color: var(--el-bg-color-page);

  &.is-active {
    background-color: var(--el-bg-color);
  }

  &:hover:not(.is-active) {
    background-color: var(--el-menu-hover-bg-color);
  }
}
</style>
