<script setup>
import { useShiftStore } from '../stores/shiftStore'
import { onBeforeUnmount, onMounted } from 'vue'
import { dayjs } from 'element-plus'

const shiftStore = useShiftStore()
onMounted(() => {
  shiftStore.startInterval()
})
onBeforeUnmount(() => {
  shiftStore.stopInterval()
})
</script>

<template>
  <div
    class="current-shift-root"
    v-if="shiftStore.currentShift && shiftStore.currentShift.isEnabled"
  >
    <div class="name">
      {{ shiftStore.currentShift.name }}
    </div>
    <div class="time">
      {{
        `${dayjs.unix(shiftStore.currentShift.startTime).format('HH:mm')}` +
        ' - ' +
        `${dayjs.unix(shiftStore.currentShift.endTime).format('HH:mm')}`
      }}
    </div>
    <div class="span-the-day" v-if="shiftStore.currentShift.spanTheDay">+1</div>
  </div>
</template>

<style scoped lang="scss">
.current-shift-root {
  display: flex;
  flex-flow: row;
  align-items: center;
  font-size: 1.2rem;
  grid-gap: 0.5rem;

  > * {
    background-color: var(--el-color-primary-light-3);
    border-radius: var(--el-border-radius-base);
    padding: 0.1rem 0.5rem;
  }

  > .span-the-day {
    background-color: var(--el-color-danger-light-3);
  }
}
</style>
