<!-- src/components/PrintLengthRealtime.vue -->
<script setup>
import { ref, onMounted, onUnmounted, watch, nextTick } from 'vue'
import * as echarts from 'echarts'
import { useCssVar, useDark, toValue } from '@vueuse/core'
import { vElementSize } from '@vueuse/components'
import { ElSegmented, ElInputNumber } from 'element-plus'

defineProps(['facilityId'])

// 用户输入
const standardLength = ref(50) // 标准印刷长度 (毫米)
const batchSize = ref(10) // 批次产品数量

// 数据类型切换
const dataType = ref('realtime') // 'realtime' 或 'history'

// 图表引用
const lengthChartRef = ref(null)
const meterChartRef = ref(null)
const histogramChartRef = ref(null)
let lengthChart = null
let meterChart = null
let histogramChart = null

// 实时数据
const currentPrintLength = ref(50.2) // 当前印刷长度 (毫米)
const totalLength = ref(1234.5) // 累计长度 (米)
const lengthData = ref([]) // 实时印刷长度数据
const meterData = ref([]) // 计米数据
const printLengthData = ref([]) // 批次产品印刷长度

// 历史数据 (模拟，后续需替换为接口)
const historyLengthData = ref([])
const historyMeterData = ref([])
const historyPrintLengthData = ref([])

// 初始化模拟数据
const initSimulatedData = () => {
  const now = Date.now()
  for (let i = 0; i < 20; i++) {
    lengthData.value.push({
      time: now - (19 - i) * 1000,
      value: (Math.random() * 4 + 48).toFixed(1) // 48-52 毫米
    })
    meterData.value.push({
      time: now - (19 - i) * 1000,
      value: (Math.random() * 10 + 100 * i).toFixed(1) // 递增的计米数据
    })
  }
  for (let i = 0; i < batchSize.value; i++) {
    printLengthData.value.push((Math.random() * 4 + 48).toFixed(1))
  }
}

// 实时印刷长度折线图
const lengthChartOptions = {
  animation: false,
  textStyle: { color: toValue(useCssVar('--gray-9')) },
  tooltip: { trigger: 'axis', axisPointer: { animation: false } },
  grid: { left: '50', right: '50', top: '50', bottom: '50' },
  xAxis: { type: 'time', name: '当前数据', splitLine: { show: false } },
  yAxis: {
    type: 'value',
    name: '毫米',
    min: 0,
    max: 100,
    axisLine: { show: true, lineStyle: { color: toValue(useCssVar('--chart-color-1')) } },
    axisLabel: { textStyle: { color: toValue(useCssVar('--chart-color-1')) } },
    nameTextStyle: { color: toValue(useCssVar('--chart-color-1')) },
  },
  series: [
    {
      name: '印刷长度',
      type: 'line',
      showSymbol: true,
      symbol: 'circle',
      symbolSize: 8,
      data: [],
      lineStyle: { color: '#00CED1', width: 1 },
      itemStyle: { color: '#00CED1' },
    },
  ],
}

const initLengthChart = () => {
  lengthChart = echarts.init(lengthChartRef.value)
  lengthChart.setOption(lengthChartOptions)
}

const updateLengthChartData = () => {
  if (dataType.value === 'realtime') {
    lengthChartOptions.series[0].data = lengthData.value.map(item => [item.time, item.value])
  } else {
    lengthChartOptions.series[0].data = historyLengthData.value
  }
  if (lengthChart) lengthChart.setOption(lengthChartOptions, true)
}

// 计米数据曲线图
const meterChartOptions = {
  animation: false,
  textStyle: { color: toValue(useCssVar('--gray-9')) },
  tooltip: { trigger: 'axis', axisPointer: { animation: false } },
  grid: { left: '50', right: '50', top: '50', bottom: '50' },
  xAxis: { type: 'time', name: '当前数据', splitLine: { show: false } },
  yAxis: {
    type: 'value',
    name: '米',
    axisLine: { show: true, lineStyle: { color: toValue(useCssVar('--chart-color-1')) } },
    axisLabel: { textStyle: { color: toValue(useCssVar('--chart-color-1')) } },
    nameTextStyle: { color: toValue(useCssVar('--chart-color-1')) },
  },
  series: [
    {
      name: '计米数据',
      type: 'line',
      smooth: true,
      showSymbol: true,
      symbol: 'circle',
      symbolSize: 8,
      data: [],
      lineStyle: { color: '#00CED1', width: 1 },
      itemStyle: { color: '#00CED1' },
    },
  ],
}

const initMeterChart = () => {
  meterChart = echarts.init(meterChartRef.value)
  meterChart.setOption(meterChartOptions)
}

const updateMeterChartData = () => {
  if (dataType.value === 'realtime') {
    meterChartOptions.series[0].data = meterData.value.map(item => [item.time, item.value])
  } else {
    meterChartOptions.series[0].data = historyMeterData.value
  }
  if (meterChart) meterChart.setOption(meterChartOptions, true)
}

// 直方图
const histogramChartOptions = {
  animation: false,
  textStyle: { color: toValue(useCssVar('--gray-9')) },
  tooltip: { trigger: 'axis', axisPointer: { type: 'shadow' } },
  grid: { left: '50', right: '50', top: '50', bottom: '50' },
  xAxis: {
    type: 'category',
    name: '长度 (毫米)',
    axisTick: { show: false },
    axisLine: { show: false },
  },
  yAxis: {
    type: 'value',
    name: '数量',
  },
  series: [
    {
      name: '印刷长度分布',
      type: 'bar',
      barGap: '0%',
      barCategoryGap: '0%',
      itemStyle: { color: '#00CED1' },
      data: [],
    },
  ],
}

const initHistogramChart = () => {
  histogramChart = echarts.init(histogramChartRef.value)
  //const step = 0.5
  const bins = [
    standardLength.value - 1.5,
    standardLength.value - 1.0,
    standardLength.value - 0.5,
    standardLength.value,
    standardLength.value + 0.5,
    standardLength.value + 1.0,
    standardLength.value + 1.5,
  ]
  const histogramData = Array(bins.length - 1).fill(0)

  const dataToUse = dataType.value === 'realtime' ? printLengthData.value : historyPrintLengthData.value
  dataToUse.forEach(length => {
    for (let i = 0; i < bins.length - 1; i++) {
      if (length >= bins[i] && length < bins[i + 1]) {
        histogramData[i]++
        break
      }
    }
  })

  histogramChartOptions.xAxis.data = bins.slice(0, -1).map((bin, index) => `${bin}-${bins[index + 1]}`)
  histogramChartOptions.series[0].data = histogramData
  histogramChart.setOption(histogramChartOptions)
}

// 模拟实时数据更新
let intervalId
const startInterval = () => {
  intervalId = setInterval(() => {
    if (dataType.value === 'realtime') {
      const now = Date.now()
      currentPrintLength.value = (Math.random() * 4 + 48).toFixed(1)
      totalLength.value += parseFloat((Math.random() * 0.5).toFixed(1))
      lengthData.value.push({ time: now, value: currentPrintLength.value })
      meterData.value.push({ time: now, value: totalLength.value })
      printLengthData.value.push(parseFloat(currentPrintLength.value))

      if (lengthData.value.length > 20) lengthData.value.shift()
      if (meterData.value.length > 20) meterData.value.shift()
      if (printLengthData.value.length > batchSize.value) printLengthData.value.shift()

      updateLengthChartData()
      updateMeterChartData()
      initHistogramChart()
    }
  }, 2000)
}
const stopInterval = () => {
  if (intervalId) clearInterval(intervalId)
}

// 切换数据类型
watch(dataType, async () => {
  if (dataType.value === 'history') {
    historyLengthData.value = Array.from({ length: 20 }, (_, i) => ({
      time: new Date(Date.now() - (20 - i) * 3600 * 1000),
      value: (Math.random() * 10 + 45).toFixed(1),
    }))
    historyMeterData.value = Array.from({ length: 20 }, (_, i) => ({
      time: new Date(Date.now() - (20 - i) * 3600 * 1000),
      value: (Math.random() * 10 + 100 * i).toFixed(1),
    }))
    historyPrintLengthData.value = Array.from({ length: batchSize.value }, () => (Math.random() * 4 + 48).toFixed(1))
  }
  updateLengthChartData()
  updateMeterChartData()
  initHistogramChart()
})

// 监听用户输入变化
watch([standardLength, batchSize], () => {
  if (dataType.value === 'realtime') {
    printLengthData.value = Array.from({ length: batchSize.value }, () => (Math.random() * 4 + 48).toFixed(1))
  } else {
    historyPrintLengthData.value = Array.from({ length: batchSize.value }, () => (Math.random() * 4 + 48).toFixed(1))
  }
  initHistogramChart()
})

// 图表自适应
const onResize = () => {
  if (lengthChart) lengthChart.resize()
  if (meterChart) meterChart.resize()
  if (histogramChart) histogramChart.resize()
}

// 生命周期
onMounted(async () => {
  initSimulatedData()
  await nextTick(() => {
    initLengthChart()
    initMeterChart()
    initHistogramChart()
    startInterval()
  })
})
onUnmounted(() => {
  stopInterval()
  if (lengthChart) lengthChart.dispose()
  if (meterChart) meterChart.dispose()
  if (histogramChart) histogramChart.dispose()
})
useDark({ onChanged: () => {
  if (lengthChart) updateLengthChartData()
  if (meterChart) updateMeterChartData()
  if (histogramChart) initHistogramChart()
}})
</script>

<template>
  <div class="realtime-container">
    <!-- 切换按钮 -->
    <div class="switch-container">
      <ElSegmented
        :options="[
          { label: '实时数据', value: 'realtime' },
          { label: '历史数据', value: 'history' }
        ]"
        v-model="dataType"
      />
    </div>

    <!-- 上方区域：印刷长度折线图 -->
    <div class="top-section">
      <div class="data-display">
        <div class="data-item">
          <span class="label">当前印刷长度:</span>
          <span class="value">{{ currentPrintLength }} 毫米</span>
        </div>
        <div class="data-item">
          <span class="label">累计长度:</span>
          <span class="value">{{ totalLength }} 米</span>
        </div>
      </div>
      <div class="chart" ref="lengthChartRef" v-element-size="onResize"></div>
    </div>

    <!-- 下方区域：分为左下和右下 -->
    <div class="bottom-section">
      <!-- 左下区域：计米数据曲线图 -->
      <div class="left-bottom">
        <h3>计米数据</h3>
        <div class="chart" ref="meterChartRef" v-element-size="onResize"></div>
      </div>

      <!-- 右下区域：直方图 -->
      <div class="right-bottom">
        <h3>最近产品印刷长度分布</h3>
        <div class="chart-setting">
          <div class="input-item">
            <label>标准印刷长度 (毫米):</label>
            <ElInputNumber v-model="standardLength" :min="0" :step="0.1" size="small" />
          </div>
          <div class="input-item">
            <label>批次产品数量:</label>
            <ElInputNumber v-model="batchSize" :min="1" :max="100" size="small" />
          </div>
        </div>
        <div class="chart" ref="histogramChartRef" v-element-size="onResize"></div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.realtime-container {
  display: flex;
  flex-direction: column;
  height: 100%;
  padding: 1rem;

  .switch-container {
    margin-bottom: 1rem;
  }

  .top-section {
    flex: 0 0 300px;
    padding: 1rem;
    background-color: #f5f7fa;
    border-radius: 8px;
    margin-bottom: 1rem;
    display: flex;
    flex-direction: column;

    .data-display {
      display: flex;
      flex-flow: row;
      gap: 2rem;
      padding: 1rem;
      background: #f5f5f5;
      border-bottom: 1px solid #e8e8e8;

      .data-item {
        display: flex;
        align-items: center;

        .label {
          font-weight: bold;
          margin-right: 0.5rem;
        }

        .value {
          font-size: 1.2rem;
          color: #333;
        }
      }
    }

    .chart {
      flex: 1;
      width: 100%;
    }
  }

  .bottom-section {
    flex: 1;
    display: flex;
    gap: 1rem;

    .left-bottom {
      flex: 1;
      padding: 1rem;
      background-color: #f5f7fa;
      border-radius: 8px;
      display: flex;
      flex-direction: column;
      align-items: center;

      .chart {
        width: 100%;
        height: 100%;
      }
    }

    .right-bottom {
      flex: 2;
      padding: 1rem;
      background-color: #f5f7fa;
      border-radius: 8px;
      display: flex;
      flex-direction: column;
      align-items: center;

      .chart-setting {
        display: flex;
        gap: 1rem;
        padding: 1rem 0;

        .input-item {
          display: flex;
          align-items: center;
          gap: 0.5rem;
        }
      }

      .chart {
        width: 100%;
        height: 100%;
      }
    }
  }
}
</style>