<template>
    <div>
      <h3>历史数据</h3>
      <p>设备 ID: {{ facilityId }}</p>
      <div class="query-container">
        <ElDatePicker
          style="width: 24rem"
          type="daterange"
          v-model="date"
          :default-time="defaultTime"
          :shortcuts="dateShortcuts"
        ></ElDatePicker>
        <ElButton
          type="primary"
          plain
          @click="onQueryButtonClick"
          :disabled="loadingHistoryData"
        >
          查询
        </ElButton>
        <ElButton
          type="primary"
          plain
          @click="exportHistoryData"
          :disabled="!historyData4Echarts"
        >
          导出历史数据
        </ElButton>
        <ElIcon class="is-loading" v-show="loadingHistoryData" :size="24">
          <Loading></Loading>
        </ElIcon>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue'
  import { dayjs } from 'element-plus'
  import { Loading } from '@element-plus/icons-vue'
  
  defineProps(['facilityId'])
  
  const date = ref([
    new Date(dayjs().subtract(6, 'day').hour(0).minute(0).second(0).millisecond(0)),
    new Date(new Date().setHours(23, 59, 59, 999)),
  ])
  const defaultTime = [
    new Date(2000, 1, 1, 0, 0, 0),
    new Date(2000, 1, 1, 23, 59, 59),
  ]
  const dateShortcuts = [
    { text: '最近 7 天', value: () => [new Date(new Date().getTime() - 6 * 24 * 60 * 60 * 1000), new Date()] },
    { text: '最近 14 天', value: () => [new Date(new Date().getTime() - 13 * 24 * 60 * 60 * 1000), new Date()] },
    { text: '最近 30 天', value: () => [new Date(new Date().getTime() - 29 * 24 * 60 * 60 * 1000), new Date()] },
  ]
  
  const loadingHistoryData = ref(false)
  const historyData4Echarts = ref(null)
  
  const onQueryButtonClick = () => {
    loadingHistoryData.value = true
    // 这里需要添加实际的查询逻辑，完成后设置 loadingHistoryData.value = false
  }
  
//   const handleDataLoaded = (data) => {
//     historyData4Echarts.value = data
//     loadingHistoryData.value = false
//   }
  
  function convertToCSV(data) {
    const headers = ['Time', 'PrintLength']
    let csvContent = headers.join(',') + '\n'
  
    if (!data || !data.printLengthData) return csvContent
  
    data.printLengthData.forEach((item) => {
      const row = [
        dayjs.unix(item.time).format('YYYY-MM-DD HH:mm:ss'),
        item.value
      ]
      csvContent += row.join(',') + '\n'
    })
    return csvContent
  }
  
  function exportHistoryData() {
    if (!historyData4Echarts.value) {
      console.warn('没有可导出的历史数据')
      return
    }
    const csvContent = convertToCSV(historyData4Echarts.value)
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = dayjs().format('YYYY-MM-DD HH:mm') + '印刷长度历史数据.csv'
    link.style.visibility = 'hidden'
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
  }
  </script>
  
  <style scoped lang="scss">
  .query-container {
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;
    align-items: center;
  
    > .is-loading {
      color: var(--el-color-primary);
    }
  }
  </style>