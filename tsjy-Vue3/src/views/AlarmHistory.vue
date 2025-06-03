<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import { onMounted, provide, ref } from 'vue'
import { dayjs } from 'element-plus'
import AlarmHistoryTable from '@/components/AlarmHistoryTable.vue'
import AlarmCountChart from '@/components/AlarmCountChart.vue'

// region 组件
onMounted(() => {
  startTime.value = date.value[0]
  endTime.value = date.value[1]
})
// endregion

// region 时间
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
    text: '最近 7 天',
    value: () => {
      const end = new Date()
      const start = new Date()
      start.setTime(start.getTime() - 6 * 24 * 60 * 60 * 1000)
      return [start, end]
    },
  },
  {
    text: '最近 14 天',
    value: () => {
      const end = new Date()
      const start = new Date()
      start.setTime(start.getTime() - 13 * 24 * 60 * 60 * 1000)
      return [start, end]
    },
  },
  {
    text: '最近 30 天',
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

const startTime = ref()
const endTime = ref()
provide('startTime', startTime)
provide('endTime', endTime)
// endregion

// region 筛选
const loadChartData = ref(false)
const loadTableData = ref(false)

const filter = () => {
  startTime.value = date.value[0]
  endTime.value = date.value[1]
  loadChartData.value = true
  loadTableData.value = true
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>报警历史</template>
    <template #default>
      <div class="main">
        <div class="header">
          <ElDatePicker
            style="width: 24rem"
            type="daterange"
            v-model="date"
            :default-time="defaultTime"
            :shortcuts="dateShortcuts"
            @calendar-change="handleCalendarChange"
          >
          </ElDatePicker>
          <ElButton type="primary" plain @click="filter">筛选</ElButton>
        </div>
        <div class="content">
          <AlarmHistoryTable class="table" v-model="loadTableData">
          </AlarmHistoryTable>
          <AlarmCountChart class="chart" v-model="loadChartData">
          </AlarmCountChart>
        </div>
      </div>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss">
.main {
  flex: 1;
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;
  overflow: auto;

  > .header {
    display: flex;
    grid-gap: 1rem;
    flex-flow: row;
    width: min-content;
  }

  > .content {
    flex: 1;
    display: flex;
    flex-flow: row;
    overflow: auto;
    grid-gap: 1rem;

    > .table {
      flex: 1;
    }

    > .chart {
      flex: 1;
    }
  }
}
</style>
