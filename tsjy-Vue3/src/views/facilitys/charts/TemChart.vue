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
      text: '电机温度实时监控',
      left: 'center',
    },
    tooltip: {
      trigger: 'axis',
      formatter: '{b}<br/>{a}: {c} °C',
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
      name: '温度 (°C)',
      min: 20,
      max: 100,
      interval: 10,
      axisLabel: {
        formatter: '{value} °C',
      },
    },
    series: [
      {
        name: '电机温度',
        type: 'line',
        showSymbol: true,
        smooth: true,
        lineStyle: {
          width: 3,
          color: '#ee6666', // Changed to red for temperature indication
        },
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: 'rgba(238, 102, 102, 0.5)' },
            { offset: 1, color: 'rgba(238, 102, 102, 0.1)' },
          ]),
        },
        markArea: {
          silent: true,
          itemStyle: {
            color: 'rgba(255, 173, 177, 0.2)',
          },
          data: [
            [
              {
                yAxis: 80,
                itemStyle: {
                  color: 'rgba(255, 0, 0, 0.1)',
                },
              },
              {
                yAxis: 100,
              },
            ],
          ],
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
      value: [time, 30], // Starting at ambient temperature
    })
  }
  return data
}

const generateDataPoint = (lastValue) => {
  const currentTime = new Date()
  // 生成30-85°C之间的随机温度，模拟电机工作温度
  let randomValue
  if (lastValue < 40) {
    // Cold start phase - faster heating
    randomValue = lastValue + Math.random() * 3
  } else if (lastValue > 80) {
    // High temperature phase - slower changes
    randomValue = lastValue + (Math.random() * 2 - 1)
  } else {
    // Normal operation - moderate changes
    randomValue = lastValue + (Math.random() * 2.5 - 1.25)
  }

  // Ensure temperature stays within bounds
  randomValue = Math.min(95, Math.max(25, randomValue))

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
    : 30

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
