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
      text: '电机振动幅度实时监控',
      left: 'center',
    },
    tooltip: {
      trigger: 'axis',
      formatter: (params) => {
        const value = params[0].value
        return `振动频率: ${value[0].toLocaleTimeString('zh')}<br/>
                  振幅: ${value[1].toFixed(3)} mm<br/>
                  ${value[1] > 0.2 ? '⚠️ 异常振动' : '✅ 正常状态'}`
      },
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
      name: '振幅 (mm)',
      min: 0,
      max: 0.5,
      interval: 0.05,
      axisLabel: {
        formatter: (value) => value.toFixed(2) + ' mm',
      },
    },
    series: [
      {
        name: '振动幅度',
        type: 'line',
        showSymbol: false, // 振动数据通常不需要显示点
        smooth: false, // 振动数据需要原始波形
        lineStyle: {
          width: 1,
          color: '#37A2FF',
        },
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: 'rgba(55, 162, 255, 0.6)' },
            { offset: 1, color: 'rgba(55, 162, 255, 0.1)' },
          ]),
        },
        markLine: {
          silent: true,
          data: [
            {
              yAxis: 0.2, // 振动报警阈值
              lineStyle: {
                color: '#FF0000',
                type: 'dashed',
              },
              label: {
                position: 'end',
                formatter: '安全阈值 0.2mm',
              },
            },
          ],
        },
        data: [],
      },
    ],
    graphic: [
      {
        type: 'text',
        right: 10,
        top: 10,
        style: {
          text: '安全阈值: 0.2mm',
          fill: '#666',
          fontSize: 12,
        },
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
    data.push([time, 0]) // 初始振幅为0
  }
  return data
}

// 生成模拟振动数据（包含正常波动和偶尔的异常峰值）
const generateVibrationData = (lastData) => {
  const now = new Date()
  let amplitude

  // 10%概率生成异常振动
  if (Math.random() < 0.1) {
    amplitude = 0.15 + Math.random() * 0.35 // 异常振动范围：0.15-0.5mm
  } else {
    // 正常振动范围：0-0.15mm
    amplitude = Math.min(
      0.15,
      Math.max(
        0,
        (lastData.length ? lastData[lastData.length - 1][1] : 0) +
          (Math.random() * 0.02 - 0.01), // 微小波动
      ),
    )
  }

  return [now, amplitude]
}

const updateChartData = (chart) => {
  const option = chart.getOption()
  const currentData = option.series[0].data

  const newPoint = generateVibrationData(currentData)
  currentData.push(newPoint)

  // 保持50个数据点（振动监测需要更高密度）
  if (currentData.length > 50) {
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

  // 振动数据更新更快（500ms）
  updateInterval = setInterval(() => {
    updateChartData(chart)
  }, 500)

  window.addEventListener('resize', () => chart.resize())
})

onBeforeUnmount(() => {
  if (updateInterval) clearInterval(updateInterval)
  if (chartInstance) chartInstance.dispose()
})
</script>

<style>
.echart-container {
  background: #f8f8f8;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}
</style>
