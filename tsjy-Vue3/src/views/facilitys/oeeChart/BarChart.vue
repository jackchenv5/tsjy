<template>
  <div class="bar-chart-container" ref="chartRef"></div>
</template>
<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import * as echarts from 'echarts'
const chartRef = ref(null)
let chartInstance = null
// 生成连续日期数组
const generateDates = () => {
  const dates = []
  const startDate = new Date('2025-01-01')
  for (let i = 0; i < 20; i++) {
    const date = new Date(startDate)
    date.setDate(date.getDate() + i)
    const formatted = date.toISOString().split('T')[0]
    dates.push(formatted)
  }
  return dates
}
const initChart = () => {
  chartInstance = echarts.init(chartRef.value)

  const dates = generateDates() // 获取日期数组
  chartInstance.setOption({
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        lineStyle: {
          width: 1,
          color: '#019680',
        },
      },
    },
    xAxis: {
      type: 'category',
      boundaryGap: false,
      data: dates,
      splitLine: {
        show: true,
        lineStyle: {
          width: 1,
          type: 'solid',
          color: 'rgba(226,226,226,0.5)',
        },
      },
      axisTick: {
        show: false,
      },
    },
    yAxis: [
      {
        type: 'value',
        max: 80000,
        splitNumber: 4,
        axisTick: {
          show: false,
        },
        splitArea: {
          show: true,
          areaStyle: {
            color: ['rgba(255,255,255,0.2)', 'rgba(226,226,226,0.2)'],
          },
        },
      },
    ],
    grid: {
      left: '1%',
      right: '1%',
      top: '2  %',
      bottom: 0,
      containLabel: true,
    },
    series: [
      {
        smooth: true,
        data: [
          111, 222, 4000, 18000, 33333, 55555, 66666, 33333, 14000, 36000,
          66666, 44444, 22222, 11111, 4000, 2000, 500, 333, 223, 111,
        ],
        type: 'line',
        areaStyle: {},
        itemStyle: {
          color: '#5ab1ef',
        },
      },
      {
        smooth: true,
        data: [
          33, 66, 88, 333, 3333, 5000, 18000, 3000, 1200, 13000, 22000, 11000,
          2221, 1201, 390, 198, 60, 30, 22, 11,
        ],
        type: 'line',
        areaStyle: {},
        itemStyle: {
          color: '#019680',
        },
      },
    ],
  })
}
onMounted(() => {
  initChart()
})
</script>
<style scoped>
.bar-chart-container {
  width: 100%;
  height: 30vh;
}
</style>
