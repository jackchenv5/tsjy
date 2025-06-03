<script setup>
import { onMounted, ref } from 'vue'
import * as echarts from 'echarts'

// region 组件
onMounted(() => {
  initChart()
  updateChart()
})
// endregion

// region 图表
const chartRef = ref()
let chart
let chartOptions = {
  textStyle: {
    color: '',
  },
  color: [],
  tooltip: {
    trigger: 'axis',
  },
  grid: {
    left: '40',
    right: '50',
    top: '50',
    bottom: '10',
    containLabel: true,
  },
  xAxis: {
    type: 'value',
    data: [],
    axisPointer: {
      type: 'shadow',
    },
    name: '日期',
    nameLocation: 'end',
  },
  yAxis: {
    type: 'category',
    name: '停机时长/分',
    min: 0,
    alignTicks: true,
    axisLine: {
      show: true,
    },
    axisLabel: {
      showMinLabel: true,
      // formatter: yAxisLabelFormatter,
    },
  },
  series: [
    {
      name: '待机',
      type: 'bar',
      stack: 'total',
      barWidth: '20%',
      data: [120, 200, 150],
      itemStyle: {
        color: '',
      },
      tooltip: {
        // valueFormatter: valueFormatter,
      },
    },
    {
      name: '停机',
      type: 'bar',
      stack: 'total',
      barWidth: '20%',
      data: [120, 200, 150],
      itemStyle: {
        color: '',
      },
      tooltip: {
        // valueFormatter: valueFormatter,
      },
    },
    {
      name: '故障',
      type: 'bar',
      stack: 'total',
      barWidth: '20%',
      data: [120, 200, 150],
      itemStyle: {
        color: '',
      },
      tooltip: {
        // valueFormatter: valueFormatter,
      },
    },
  ],
}
const initChart = () => {
  chart = echarts.init(chartRef.value)
}
const updateChart = () => {
  if (chart) {
    chart.setOption(chartOptions, true)
  }
}

// endregion
</script>

<template>
  <div class="record-chart-root">
    <div class="header"></div>
    <div class="chart" ref="chartRef"></div>
  </div>
</template>

<style scoped lang="scss">
.record-chart-root {
  flex: 1;
  display: flex;
  flex-flow: column;

  > .header {
  }

  > .chart {
    flex: 1;
    background-color: #ffbb96;
  }
}
</style>
