<script setup>
import { vElementSize } from '@vueuse/components'
import { nextTick, onMounted, ref, toValue, watch } from 'vue'
import * as echarts from 'echarts'
import { useCssVar, useDark } from '@vueuse/core'
import { getRollDiameterHistoryDataApi } from '@/apis/sawingMotor.js' // 替换为新的 API
import { dayjs } from 'element-plus'

// region 组件
const model = defineModel()
const props = defineProps(['motorId', 'startTime', 'endTime'])
onMounted(async () => {
  await nextTick(async () => {
    initChart()
    updateChartColor()
  })
})
watch(model, async (value) => {
  if (value) {
    await getRollDiameterHistoryData()
    model.value = false
  }
})
const emit = defineEmits(['data-loaded'])

// region 卷径数据
const loadingRollDiameterHistoryData = ref(false)
const rollDiameterHistoryData = ref({
  realRollDiameterData: [],
  reasonableRollDiameterMinData: [],
  reasonableRollDiameterMaxData: [],
})
const getRollDiameterHistoryData = async () => {
  if (loadingRollDiameterHistoryData.value) {
    return
  }
  loadingRollDiameterHistoryData.value = true
  try {
    const { data } = await getRollDiameterHistoryDataApi(
      props.motorId,
      dayjs(props.startTime).unix(),
      dayjs(props.endTime).unix(),
    )
    rollDiameterHistoryData.value = data
    updateChartData()
    emit('data-loaded', rollDiameterHistoryData.value)
  } catch (e) {
    console.error(e)
  } finally {
    loadingRollDiameterHistoryData.value = false
  }
}
// endregion

// region 图表
let chart
const chartRef = ref()
let chartOptions = {
  textStyle: {
    color: '',
  },
  color: [],
  tooltip: {
    trigger: 'axis',
    axisPointer: {
      animation: false,
    },
  },
  legend: {
    top: '20',
    textStyle: {
      color: 'inherit',
    },
  },
  grid: {
    left: '50',
    right: '50',
    top: '50',
    bottom: '70',
  },
  xAxis: {
    type: 'time',
    splitLine: {
      show: false,
    },
  },
  yAxis: [
    {
      type: 'value',
      name: '实际卷径',
      position: 'left',
      offset: 25,
      min: 0,
      max: 100, // 根据实际卷径范围调整
      axisLine: {
        show: true,
        lineStyle: {
          color: toValue(useCssVar('--chart-color-1')),
        },
      },
      axisLabel: {
        showMinLabel: true,
        textStyle: {
          color: toValue(useCssVar('--chart-color-1')),
        },
      },
      nameTextStyle: {
        color: toValue(useCssVar('--chart-color-1')),
      },
      alignTicks: true,
      boundaryGap: [0, '20%'],
    },
    {
      type: 'value',
      name: '合理卷径最小值',
      position: 'left',
      offset: -5,
      min: 0,
      max: 100, // 根据实际卷径范围调整
      axisLine: {
        show: true,
        lineStyle: {
          color: toValue(useCssVar('--chart-color-2')),
        },
      },
      axisLabel: {
        showMinLabel: true,
        textStyle: {
          color: toValue(useCssVar('--chart-color-2')),
        },
      },
      nameTextStyle: {
        color: toValue(useCssVar('--chart-color-2')),
      },
      alignTicks: true,
      boundaryGap: [0, '20%'],
    },
    {
      type: 'value',
      name: '合理卷径最大值',
      position: 'right',
      offset: -2,
      min: 0,
      max: 100, // 根据实际卷径范围调整
      axisLine: {
        show: true,
        lineStyle: {
          color: toValue(useCssVar('--chart-color-3')),
        },
      },
      axisLabel: {
        showMinLabel: true,
        textStyle: {
          color: toValue(useCssVar('--chart-color-3')),
        },
      },
      nameTextStyle: {
        color: toValue(useCssVar('--chart-color-3')),
      },
      alignTicks: true,
      boundaryGap: [0, '20%'],
    },
  ],
  series: [
    {
      name: '实际卷径',
      type: 'line',
      yAxisIndex: 0,
      showSymbol: false,
      data: [],
      itemStyle: {
        color: '',
      },
      symbol: 'none',
      connectNulls: false,
      sampling: 'lttb',
    },
    {
      name: '合理卷径最小值',
      type: 'line',
      yAxisIndex: 1,
      showSymbol: false,
      data: [],
      itemStyle: {
        color: '',
      },
      symbol: 'none',
      connectNulls: false,
      sampling: 'lttb',
    },
    {
      name: '合理卷径最大值',
      type: 'line',
      yAxisIndex: 2,
      showSymbol: false,
      data: [],
      itemStyle: {
        color: '',
      },
      symbol: 'none',
      connectNulls: false,
      sampling: 'lttb',
    },
  ],
  dataZoom: [
    {
      type: 'inside',
      start: 0,
      end: 20,
    },
    {
      start: 0,
      end: 20,
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
const updateChartColor = () => {
  chartOptions.textStyle.color = toValue(useCssVar('--gray-9'))
  chartOptions.color = [
    toValue(useCssVar('--chart-color-1')),
    toValue(useCssVar('--chart-color-2')),
    toValue(useCssVar('--chart-color-3')),
  ]
  updateChart()
}
useDark({
  onChanged() {
    if (chart) {
      updateChartColor()
    }
  },
})

const updateChartData = () => {
  let realRollDiameterData = []
  let reasonableRollDiameterMinData = []
  let reasonableRollDiameterMaxData = []

  for (let i = 0; i < rollDiameterHistoryData.value.realRollDiameterData.length; i++) {
    const data = rollDiameterHistoryData.value.realRollDiameterData[i]
    realRollDiameterData.push({
      time: data.time.toString(),
      value: [dayjs.unix(data.time).toDate(), data.value],
    })
  }
  for (let i = 0; i < rollDiameterHistoryData.value.reasonableRollDiameterMinData.length; i++) {
    const data = rollDiameterHistoryData.value.reasonableRollDiameterMinData[i]
    reasonableRollDiameterMinData.push({
      time: data.time.toString(),
      value: [dayjs.unix(data.time).toDate(), data.value],
    })
  }
  for (let i = 0; i < rollDiameterHistoryData.value.reasonableRollDiameterMaxData.length; i++) {
    const data = rollDiameterHistoryData.value.reasonableRollDiameterMaxData[i]
    reasonableRollDiameterMaxData.push({
      time: data.time.toString(),
      value: [dayjs.unix(data.time).toDate(), data.value],
    })
  }

  chartOptions.series[0].data = realRollDiameterData
  chartOptions.series[1].data = reasonableRollDiameterMinData
  chartOptions.series[2].data = reasonableRollDiameterMaxData
  updateChart()
}
let timerId
const onResize = () => {
  if (timerId) {
    clearTimeout(timerId)
  }
  timerId = setTimeout(() => {
    if (chart) {
      chart.resize()
    }
  }, 50)
}
// endregion
</script>

<template>
  <div class="chart" ref="chartRef" v-element-size="onResize"></div>
</template>

<style scoped lang="scss">
.chart {
  flex: 1;
  overflow: hidden;
}
</style>