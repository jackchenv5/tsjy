<script setup>
import { Fold, Expand } from '@element-plus/icons-vue'
import { useMenuStateStore } from '../stores/menuStateStore'

const menuStateStore = useMenuStateStore()

const handleToggle = () => {
  menuStateStore.setClosedByUser(menuStateStore.isExpanded)
}
</script>

<template>
  <div class="toggle-root" @click="handleToggle">
    <ElIcon class="icon">
      <Fold v-if="menuStateStore.isExpanded"></Fold>
      <Expand v-else></Expand>
    </ElIcon>
  </div>
</template>

<style scoped lang="scss">
.toggle-root {
  height: 100%;
  display: inline-flex;
  align-items: center;
  padding: 0 1.6rem;
  cursor: pointer;

  &:hover {
    background-color: var(--el-menu-hover-bg-color);
  }

  .icon {
    color: var(--el-color-primary);
    font-size: 2rem;
  }
}
</style>
