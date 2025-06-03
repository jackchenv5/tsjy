<script setup>
defineProps(['motor', 'active'])
const emits = defineEmits(['click'])
</script>

<template>
  <div class="container" :class="{ active: active }" @click="emits('click')">
    <ElText truncated>
      {{ motor.name }}
    </ElText>
  </div>
</template>

<style scoped lang="scss">
.container {
  border: 1px solid var(--el-border-color);
  border-radius: var(--el-border-radius-base);
  padding: 0.5rem 1rem;
  height: 4rem;
  min-height: 4rem;
  width: 16rem;
  display: flex;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;

  &:hover {
    border-color: var(--el-color-primary);
  }

  &.active {
    background-color: var(--el-color-primary-light-9);
  }
}
</style>
