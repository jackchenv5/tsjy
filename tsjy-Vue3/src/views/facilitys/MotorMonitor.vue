<template>
  <div class="container">
    <div class="motor-label">
      <el-card style="height: 120px" header="电机名称：卷绕">
        电机型号：22554
      </el-card>
      <el-card style="height: 120px" header="电机名称：拖引">
        电机型号：22554
      </el-card>
    </div>
    <div class="motor-card">
      <el-card style="height: 150px" header="电机转速">
        <div style="display: flex; justify-content: space-between">
          <div>
            <div>额定:1200</div>
            <div>最大:2400</div>
          </div>
          <div>0.0 RPM</div>
        </div>
        <el-progress :text-inside="true" :stroke-width="16" :percentage="50" />
      </el-card>
      <el-card style="height: 150px" header="电机电流">
        <div style="display: flex; justify-content: space-between">
          <div>
            <div>额定:80</div>
            <div>最大:120</div>
          </div>
          <div>0.0 A</div>
        </div>
        <el-progress :text-inside="true" :stroke-width="16" :percentage="50" />
      </el-card>
      <el-card style="height: 150px" header="电机扭矩">
        <div style="display: flex; justify-content: space-between">
          <div>
            <div>额定:250</div>
            <div>最大:500</div>
          </div>
          <div>0.0 N.m</div>
        </div>
        <el-progress :text-inside="true" :stroke-width="16" :percentage="50" />
      </el-card>
      <el-card style="height: 150px" header="电机温度">
        <div style="display: flex; justify-content: space-between">
          <div>
            <div>额定:80</div>
            <div>最大:150</div>
          </div>
          <div>0.0 ℃</div>
        </div>
        <el-progress :text-inside="true" :stroke-width="16" :percentage="50" />
      </el-card>
      <el-card style="height: 150px" header="电机振幅">
        <div style="display: flex; justify-content: space-between">
          <div>
            <div>额定:80</div>
            <div>最大:150</div>
          </div>
          <div>0.0 mm</div>
        </div>
        <el-progress :text-inside="true" :stroke-width="16" :percentage="50" />
      </el-card>
    </div>
    <div class="montor-chart">
      <el-card>
        <RpmChart />
        <EleChart />
        <PowerChart />
        <TemChart />
        <AChart />
      </el-card>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
import * as echarts from 'echarts'

import RpmChart from './charts/RpmChart.vue'
import EleChart from './charts/EleChart.vue'
import PowerChart from './charts/PowerChart.vue'
import TemChart from './charts/TemChart.vue'
import AChart from './charts/AChart.vue'
// 创建图表的容器引用
const chartContainer = ref(null)
let chartInstance = null
let updateInterval = null

// 初始化图表
const initChart = () => {
  if (!chartContainer.value) return

  // 销毁现有实例（如果存在）
  if (chartInstance) {
    chartInstance.dispose()
  }

  // 初始化图表实例
  chartInstance = echarts.init(chartContainer.value)

  // 设置图表配置
  const option = {
    title: {
      text: '电机转速实时监控',
      left: 'center',
    },
    tooltip: {
      trigger: 'axis',
      formatter: '{b}<br/>{a}: {c} RPM',
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
      name: '转速 (RPM)',
      min: 0,
      max: 2400,
      interval: 300,
      axisLabel: {
        formatter: '{value} RPM',
      },
    },
    series: [
      {
        name: '电机转速',
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

// 初始数据（当前时间前9秒到当前时间）
const initialData = () => {
  const now = new Date()
  const data = []
  for (let i = 9; i >= 0; i--) {
    const time = new Date(now - i * 1000)
    // 初始值设为0
    data.push({
      name: time.toLocaleTimeString('zh'),
      value: [time, 0],
    })
  }
  return data
}

// 生成随机数据点（在合理范围内波动）
const generateDataPoint = (lastValue) => {
  const currentTime = new Date()
  // 生成1700-2300之间的随机转速，更符合实际设备运行情况
  const randomValue =
    lastValue === 0
      ? Math.floor(Math.random() * 600) + 1700
      : Math.min(2400, Math.max(1700, lastValue + (Math.random() * 100 - 50)))

  return {
    name: currentTime.toLocaleTimeString('zh'),
    value: [currentTime, randomValue],
  }
}

// 更新图表数据
const updateChartData = (chart) => {
  const option = chart.getOption()
  const currentData = option.series[0].data

  // 获取最后的值用于连续变化
  const lastValue = currentData.length
    ? currentData[currentData.length - 1].value[1]
    : 0

  // 添加新数据点
  const newPoint = generateDataPoint(lastValue)
  currentData.push(newPoint)

  // 保持最多10个点
  if (currentData.length > 10) {
    currentData.shift()
  }

  // 更新图表
  chart.setOption({
    series: [
      {
        data: currentData,
      },
    ],
  })
}

// 组件挂载时初始化
onMounted(() => {
  const chart = initChart()
  // 设置初始数据
  chart.setOption({
    series: [
      {
        data: initialData(),
      },
    ],
  })

  // 每秒更新数据
  updateInterval = setInterval(() => {
    updateChartData(chart)
  }, 1000)

  // 响应窗口大小变化
  window.addEventListener('resize', () => chart.resize())
})

// 组件卸载前清理
onBeforeUnmount(() => {
  if (updateInterval) clearInterval(updateInterval)
  if (chartInstance) chartInstance.dispose()
})
</script>

<style scoped lang="scss">
* {
  box-sizing: border-box;
}
.container {
  width: 100%;
  height: 100%;
  display: flex;
  gap: 10px;
}
.motor-label {
  width: 20%;
  height: 100%;
  background-color: #f5f5f5;
  display: flex;
  flex-direction: column;
  gap: 10px;
}
.motor-card {
  width: 20%;
  height: 100%;
  background-color: #f5f5f5;
  display: flex;
  flex-direction: column;
  gap: 10px;
}
.montor-chart {
  width: 60%;
  height: 100%;
  background-color: #f5f5f5;
  flex-direction: column;
  overflow: auto;
}
</style>
