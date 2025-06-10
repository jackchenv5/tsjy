<template>
  <div class="chart-container" ref="chartRef"></div>
</template>
<script setup>
import { ref, onMounted } from 'vue'
import * as echarts from 'echarts'
const chartRef = ref(null)
let chartInstance = null
const initChart = () => {
  chartInstance = echarts.init(chartRef.value)

  chartInstance.setOption({
    tooltip: {
      trigger: 'item',
    },
    legend: {
      bottom: '1%',
      left: 'center',
    },
    series: [
      {
        color: ['#5ab1ef', '#b6a2de', '#67e0e3', '#2ec7c9'],
        name: '时长',
        type: 'pie',
        radius: ['40%', '70%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 10,
          borderColor: '#fff',
          borderWidth: 2,
        },
        label: {
          show: false,
          position: 'center',
        },
        emphasis: {
          label: {
            show: true,
            fontSize: '12',
            fontWeight: 'bold',
          },
        },
        labelLine: {
          show: false,
        },
        data: [
          { value: 1048, name: '运行时长' },
          { value: 735, name: '待机时长' },
        ],
        animationType: 'scale',
        animationEasing: 'exponentialInOut',
        animationDelay: function () {
          return Math.random() * 100
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
.chart-container {
  width: 100%;
  height: 300px;
}
</style>
