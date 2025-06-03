<script setup>
import { useResize } from '#/common/composables/resize'
import StatusCompareChart from './StatusCompareChart.vue'
import ShiftCompareChart from './ShiftCompareChart.vue'
import StatusHistoryChart from './StatusHistoryChart.vue'
import { computed, inject, ref, watch } from 'vue'
import { getAllStatusApi } from '../apis/facilityStatus'

// region 组件
const { breakPoint } = useResize()
const model = defineModel()
const facilityId = inject('currentFacility')
const startTime = inject('startTime')
const endTime = inject('endTime')
// endregion

// region 加载数据
const loadingStatusCompare = ref(false)
const loadingShiftCompare = ref(false)
const loadingStatusHistory = ref(false)
watch(model, async (value) => {
  if (value) {
    await getAllStatus()
  }
})
const loading = computed(() => {
  return (
    loadingStatusCompare.value ||
    loadingShiftCompare.value ||
    loadingStatusHistory.value
  )
})
watch(loading, (value) => {
  model.value = value
})
const statusData = ref()
const getAllStatus = async () => {
  try {
    const { data } = await getAllStatusApi({
      facilityId: facilityId.value['id'],
      startTime: startTime.value,
      endTime: endTime.value,
    })
    statusData.value = data
    loadingStatusCompare.value = true
    loadingShiftCompare.value = true
    loadingStatusHistory.value = true
  } catch (e) {
    console.error(e)
  }
}
// endregion
</script>

<template>
  <div class="status-charts-root" :class="breakPoint">
    <div class="top-container" :class="breakPoint">
      <StatusCompareChart
        v-model="loadingStatusCompare"
        :status-data="statusData"
      >
      </StatusCompareChart>
      <ShiftCompareChart
        v-model="loadingShiftCompare"
        :status-data="statusData"
      >
      </ShiftCompareChart>
    </div>
    <div class="bottom-container">
      <StatusHistoryChart
        v-model="loadingStatusHistory"
        :status-data="statusData"
      >
      </StatusHistoryChart>
    </div>
  </div>
</template>

<style scoped lang="scss">
.status-charts-root {
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;
  flex: 1;
  width: 0;
  min-height: 44rem;

  &.lg,
  &.md,
  &.sm,
  &.xs {
    width: auto;
  }

  &.sm,
  &.xs {
    min-height: 66rem;
  }

  > .top-container {
    flex: 1;
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;

    &.xs {
      flex-flow: column;
      flex: 2;
    }
  }

  > .bottom-container {
    flex: 1;
    display: flex;
    flex-flow: column;
  }
}
</style>
