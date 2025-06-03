<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import FacilityList from '#/facility/components/FacilityList.vue'
import { dayjs, useLocale } from 'element-plus'
import { computed, nextTick, ref, provide } from 'vue'
import { useResize } from '#/common/composables/resize'
import ChartsComponent from '../components/StatusCharts.vue'
import RecordComponent from '../components/StatusRecord.vue'

// region 组件
const { t } = useLocale()
const { breakPoint } = useResize()

const loading = computed(() => {
  return loadingStatusRecord.value || loadingChartData.value
})
// endregion

// region 时间筛选
let startDate = dayjs()
  .subtract(6, 'day')
  .hour(0)
  .minute(0)
  .second(0)
  .millisecond(0)
const date = ref([
  new Date(startDate),
  new Date(new Date().setHours(23, 59, 59, 999)),
])
const defaultTime = [
  new Date(2000, 1, 1, 0, 0, 0),
  new Date(2000, 1, 1, 23, 59, 59),
]
const dateShortcuts = [
  {
    text: t?.('el.facilityStatus.stoppedData.last7Days'),
    value: () => {
      const end = new Date()
      const start = new Date()
      start.setTime(start.getTime() - 6 * 24 * 60 * 60 * 1000)
      return [start, end]
    },
  },
  {
    text: t?.('el.facilityStatus.stoppedData.last14Days'),
    value: () => {
      const end = new Date()
      const start = new Date()
      start.setTime(start.getTime() - 13 * 24 * 60 * 60 * 1000)
      return [start, end]
    },
  },
  {
    text: t?.('el.facilityStatus.stoppedData.last30Days'),
    value: () => {
      const end = new Date()
      const start = new Date()
      start.setTime(start.getTime() - 29 * 24 * 60 * 60 * 1000)
      return [start, end]
    },
  },
]
const tempDate = ref([])
const handleCalendarChange = (val) => {
  tempDate.value = val
}
const disabledDate = (date) => {
  if (date.getTime() > new Date().getTime()) {
    return true
  }
  if (tempDate.value.length < 2) {
    return false
  }
  const firstDate = tempDate.value[0]
  const timeOffset = Math.abs(date.getTime() - firstDate.getTime())
  return timeOffset > 29 * 24 * 60 * 60 * 1000
}
const loadingChartData = ref(false)
const loadingStatusRecord = ref(false)
const onFilter = () => {
  loadingData()
}
const startTime = ref()
const endTime = ref()
provide('startTime', startTime)
provide('endTime', endTime)
// endregion

// region 选择的设备变化
const currentFacility = ref()
const onFacilityChange = (value) => {
  currentFacility.value = value
  nextTick(() => {
    loadingData()
  })
}
provide('currentFacility', currentFacility)
// endregion

// region 加载数据
const loadingData = () => {
  startTime.value = dayjs(date.value[0]).unix()
  endTime.value = dayjs(date.value[1]).unix()

  loadingChartData.value = true
  loadingStatusRecord.value = true
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.facilityStatus.stoppedData.stoppedData') }}
    </template>
    <template #default>
      <div class="content-container">
        <FacilityList @change="onFacilityChange"></FacilityList>
        <div class="filter-container">
          <ElDatePicker
            style="width: 30rem"
            type="daterange"
            v-model="date"
            :default-time="defaultTime"
            :shortcuts="dateShortcuts"
            :disabled-date="disabledDate"
            @calendar-change="handleCalendarChange"
          >
          </ElDatePicker>
          <ElButton type="primary" plain :loading="loading" @click="onFilter">
            {{ t('el.facilityStatus.stoppedData.filter') }}
          </ElButton>
        </div>
        <div
          class="content-container"
          v-if="currentFacility?.['id']"
          :class="breakPoint"
        >
          <ChartsComponent v-model="loadingChartData"></ChartsComponent>
          <RecordComponent
            v-model="loadingStatusRecord"
            :facility-id="currentFacility['id']"
            :start-time="dayjs(date[0]).unix()"
            :end-time="dayjs(date[1]).unix()"
          >
          </RecordComponent>
        </div>
      </div>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss">
.content-container {
  display: flex;
  flex-flow: column;
  height: 100%;

  > .filter-container {
    margin-bottom: 1rem;
    display: flex;
    grid-gap: 1rem;
    flex-flow: row;
    width: min-content;
  }

  > .content-container {
    display: flex;
    flex-flow: column;
    grid-gap: 1rem;
    flex: 1;
    overflow: auto;

    &.xl,
    &.xxl {
      flex-flow: row;
    }
  }
}
</style>
