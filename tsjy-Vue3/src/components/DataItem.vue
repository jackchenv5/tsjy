<script setup>
import { computed } from 'vue'

const props = defineProps(['label', 'content', 'markColor', 'bgColor', 'color'])

const contentStyle = computed(() => {
  return {
    color: props['color'],
    borderColor: props['markColor'],
    backgroundColor: props['bgColor'],
  }
})
</script>

<template>
  <div class="data-item">
    <div class="label">
      {{ label }}
    </div>
    <div class="content" :style="contentStyle">
      <ElTooltip :content="content?.toString() ? content : '-'">
        <ElText truncated>
          {{ content?.toString() ? content : '-' }}
        </ElText>
      </ElTooltip>
    </div>
  </div>
</template>

<style scoped lang="scss">
.data-item {
  display: flex;
  flex-flow: column;
  grid-gap: 0.2rem;

  > .label {
    font-size: var(--el-font-size-extra-medial);
  }

  > .content {
    border-radius: var(--el-border-radius-base);
    display: flex;
    justify-content: center;
    padding: 0.5rem 1rem;
    border-left: 5px solid;
    background-color: var(--primary-1);
    border-color: var(--primary-6);
  }
}
</style>
