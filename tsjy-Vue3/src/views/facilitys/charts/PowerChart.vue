<template>
  <div
    ref="chartContainer"
    class="echart-container"
    style="height: 300px"
  ></div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
import * as echarts from 'echarts'

const chartContainer = ref(null)
let chartInstance = null
let updateInterval = null

const initChart = () => {
  if (!chartContainer.value) return

  if (chartInstance) {
    chartInstance.dispose()
  }

  chartInstance = echarts.init(chartContainer.value)

  const option = {
    title: {
      text: '电机扭矩实时监控',
      left: 'center',
    },
    tooltip: {
      trigger: 'axis',
      formatter: '{b}<br/>{a}: {c} N·m',
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true,
    },
    xAxis: {
      type: 'time',
      boundaryGap: false,
      name: '时间',
      nameLocation: 'middle',
      nameGap: 30,
      axisLabel: {
        formatter: '{HH}:{mm}:{ss}',
      },
    },
    yAxis: {
      type: 'value',
      name: '扭矩 (N·m)',
      min: 0,
      max: 50,
      interval: 5,
      axisLabel: {
        formatter: '{value} N·m',
      },
    },
    series: [
      {
        name: '电机扭矩',
        type: 'line',
        showSymbol: true,
        smooth: true,
        lineStyle: {
          width: 3,
          color: '#5470c6',
        },
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: 'rgba(84, 112, 198, 0.5)' },
            { offset: 1, color: 'rgba(84, 112, 198, 0.1)' },
          ]),
        },
        data: [],
      },
    ],
  }

  chartInstance.setOption(option)
  return chartInstance
}

const initialData = () => {
  const now = new Date()
  const data = []
  for (let i = 9; i >= 0; i--) {
    const time = new Date(now - i * 1000)
    data.push({
      name: time.toLocaleTimeString('zh'),
      value: [time, 0],
    })
  }
  return data
}

const generateDataPoint = (lastValue) => {
  const currentTime = new Date()
  // 生成15-40N·m之间的随机扭矩，模拟实际负载变化
  const randomValue =
    lastValue === 0
      ? Math.random() * 25 + 15
      : Math.min(50, Math.max(15, lastValue + (Math.random() * 3 - 1.5)))

  return {
    name: currentTime.toLocaleTimeString('zh'),
    value: [currentTime, randomValue],
  }
}

const updateChartData = (chart) => {
  const option = chart.getOption()
  const currentData = option.series[0].data

  const lastValue = currentData.length
    ? currentData[currentData.length - 1].value[1]
    : 0

  const newPoint = generateDataPoint(lastValue)
  currentData.push(newPoint)

  if (currentData.length > 10) {
    currentData.shift()
  }

  chart.setOption({
    series: [
      {
        data: currentData,
      },
    ],
  })
}

onMounted(() => {
  const chart = initChart()
  chart.setOption({
    series: [
      {
        data: initialData(),
      },
    ],
  })

  updateInterval = setInterval(() => {
    updateChartData(chart)
  }, 1000)

  window.addEventListener('resize', () => chart.resize())
})

onBeforeUnmount(() => {
  if (updateInterval) clearInterval(updateInterval)
  if (chartInstance) chartInstance.dispose()
})
</script>
